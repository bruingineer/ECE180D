import numpy as np
import argparse
import cv2
import paho.mqtt.client as mqtt
import imutils



CONNECTED = False
client = None
topic = 'localization'

# The callback for when the client receives a CONNACK response from the server.
def on_connect(client, userdata, flags, rc):
    print("Connected with result code "+str(rc))

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    client.subscribe("$SYS/#")

# The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, msg):
    print(msg.topic+" "+str(msg.payload))

def connect_to_server(ip): 
    global client

    client = mqtt.Client(client_id = 'localization.py')
    client.on_connect = on_connect
    client.on_message = on_message

    client.connect(ip, 1883, 60)
    

def localize(nregions, method):
    # define the lower and upper boundaries of the "green"
    # ball in the HSV color space
    greenLower =  (29, 86, 6)
    greenUpper =  (64, 255, 255)    
    orangeLower = (10, 100, 20)
    orangeUpper = (25, 255, 255)
    yellowLower = (20, 100, 100)
    yellowUpper = (30, 255, 255)
    
    #load Haar Classifier for face recognition
    faceCascade = cv2.CascadeClassifier('.\\data\\haarcascade_frontalface_default.xml')

    cap = cv2.VideoCapture(0)
    #cap.set(3,640)  #set width of frame
    #cap.set(4,480)  #set height of frame

    sF = 1.05
    region = None
    lines = []
    
    
    while True:
        ret, frame = cap.read() # Capture frame-by-frame
        
        #only compute once
        if not lines:
            height, width = frame.shape[:2]
            sep = width/nregions
            lines = [sep*i for i in range(1,nregions)]
            
        #Draw lines for player to see which region he is in
        for line in lines:
            frame = cv2.line(frame,(line,0),(line, height),(255,0,0),2)
            
        
        if method == 'face-recognition':
            gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)     
            
            faces = faceCascade.detectMultiScale(
                gray,
                scaleFactor= sF,
                minNeighbors=8,
                minSize=(60, 60),
                #flags=cv2.CV_HAAR_SCALE_IMAGE
            )
            
            # ---- Draw a rectangle around the faces
            for (x, y, w, h) in faces:
                cv2.rectangle(frame, (x, y), (x+w, y+h), (0, 0, 255), 2)
                roi_color = frame[y:y+h, x:x+w]
                
                region = nregions - (x + w/2)/sep
                
                if CONNECTED:
                    rc = client.publish(topic, payload= (int(region)), qos =0, retain=False)
                    print(rc)
                   
                break 
                
                #TODO: May need some stability control to account for random false positive
                #      face detection 
        
        elif method == 'color':
            blurred = cv2.GaussianBlur(frame, (11, 11), 0)
            hsv = cv2.cvtColor(blurred, cv2.COLOR_BGR2HSV)
            
            # construct a mask for the color "green", then perform
            # a series of dilations and erosions to remove any small
            # blobs left in the mask
            mask = cv2.inRange(hsv, orangeLower, orangeUpper)
            bitmask = cv2.bitwise_and(frame, frame, mask=mask)           
            mask = cv2.erode(mask, None, iterations=2)
            mask = cv2.dilate(mask, None, iterations=2)
            
            cv2.imshow('image', bitmask)
            cv2.imshow('mask', mask)
            
            # find contours in the mask and initialize the current
            # (x, y) center of the ball
            cnts = cv2.findContours(mask.copy(), cv2.RETR_EXTERNAL,
                cv2.CHAIN_APPROX_SIMPLE)
            cnts = cnts[0] if imutils.is_cv2() else cnts[1]
            center = None

                
            # only proceed if at least one contour was found
            if len(cnts) > 0:
                # find the largest contour in the mask, then use
                # it to compute the minimum enclosing circle and
                # centroid
                c = max(cnts, key=cv2.contourArea)
                ((x, y), radius) = cv2.minEnclosingCircle(c)
                M = cv2.moments(c)
                center = (int(M["m10"] / M["m00"]), int(M["m01"] / M["m00"]))

                # only proceed if the radius meets a minimum size
                if radius > 5 :
                    # draw the circle and centroid on the frame,
                    # then update the list of tracked points
                    cv2.circle(frame, (int(x), int(y)), int(radius),
                        (0, 255, 255), 2)
                    cv2.circle(frame, center, 5, (0, 0, 255), -1)
                    
                    region = nregions - (center[0])/sep
                    
                    if CONNECTED:
                        rc = client.publish(topic, payload= (int(region)), qos =0, retain=False)
                        print(rc)
        
        frame = cv2.flip(frame,1) #vertical flip to create mirror image
        if region is not None:
            cv2.putText(frame, str(region), (10, 30), cv2.FONT_HERSHEY_SIMPLEX,
                        0.65, (0, 0, 255), 3)
                    
        cv2.imshow('Localization', frame)
        c = cv2.waitKey(7) % 0x100
        if c == 27:
            break           
            
    cap.release()
    cv2.destroyAllWindows()

def main():
    
    parser = argparse.ArgumentParser(description='Localization Script for Synchro')
    parser.add_argument('--ip', type=str, action = 'store', default = 'localhost', help='IP address of machine running Unity')
    parser.add_argument('--nregions', type=int, action = 'store', default = 10, help='Number of regions to split frame with')
    parser.add_argument('--method' , type = str, action = 'store', default = 'color', help = 'color or face-recognition as method for localization')   
    args = parser.parse_args()
   
    try:
        connect_to_server(args.ip)
    except:
        print("Unable to connect to MQTT server ... defaulting to Standalone Mode")
        localize(args.nregions, args.method)
        return
    
    global CONNECTED
    CONNECTED = True
    localize(args.nregions, args.method)
    
if __name__ == '__main__':
    main()
