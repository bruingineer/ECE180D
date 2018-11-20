import numpy as np
import argparse
import cv2
import paho.mqtt.client as mqtt



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
    

def localize(nregions):
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
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        
        #only compute once
        if not lines:
            height, width = frame.shape[:2]
            sep = width/nregions
            lines = [sep*i for i in range(1,nregions)]
            
        
        #Draw lines for player to see which region he is in
        for line in lines:
            frame = cv2.line(frame,(line,0),(line, height),(255,0,0),2)
        
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
            
            ##################### ADAPT to 3 tracks ################
            # if region-1 < float(nregions)/3:
                # region = 'Top'
            # elif region-1 < 2*float(nregions)/3:
                # region = 'Middle'
            # else:
                # region = 'Bottom'
            #########################################################
            
            if CONNECTED:
                rc = client.publish(topic, payload= (int(region)), qos =0, retain=False)
                print(rc)
               
            break 
            
            #TODO: May need some stability control to account for random false positive
            #      face detection 
            
        
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
    parser.add_argument('--ip', type=str, action = 'store', default = None, help='IP address of machine running Unity')
    parser.add_argument('--nregions', type=int, action = 'store', default = 10, help='Number of regions to split frame with')
    parser.add_argument('--standalone', '-s', action = 'store_true', help='Run script without MQTT publishing') 
    args = parser.parse_args()
    
    if args.standalone:
        localize(args.nregions)
        return
        
    elif args.ip:
        connect_to_server(args.ip)
    else:
        connect_to_server('localhost')
    
    print("CONNECTING")
    
    global CONNECTED
    CONNECTED = True
    localize(args.nregions)
    
if __name__ == '__main__':
    main()
