import numpy as np
import argparse
import cv2
import paho.mqtt.client as mqtt

connected = False

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
    topic = 'localization'

    client = mqtt.Client(client_id = 'localization.py')
    client.on_connect = on_connect
    client.on_message = on_message

    client.connect(ip, 1883, 60)
    

def localize():
    #load Haar Classifier for face recognition
    faceCascade = cv2.CascadeClassifier('.\\data\\haarcascade_frontalface_default.xml')

    cap = cv2.VideoCapture(0)
    #cap.set(3,640)  #set width of frame
    #cap.set(4,480)  #set height of frame

    sF = 1.05

    track = ''
    cur_track = ''
    just_changed = False
    while True:
        ret, frame = cap.read() # Capture frame-by-frame
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        
        height, width = frame.shape[:2]
        
        line1_x = width/3
        line2_x = 2*line1_x
        
        #Draw lines for player to see which region he is in
        frame = cv2.line(frame,(line1_x,0),(line1_x, height),(255,0,0),5)
        frame = cv2.line(frame,(line2_x,0),(line2_x, height),(255,0,0),5)
        
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
            
            #Determine current track
            if x + w/2 <= line1_x:
                track = 'Top'
            elif x + w/2 <= line2_x:
                track = 'Middle'
            else:
                track = 'Bottom'
                
            #Attempt to add some stability by preventing changes in subsequent frames
            #TODO: currently waits for one frame, should we wait more ?
            if track != cur_track and not just_changed:
                cur_track = track
                
                #TODO: Integrate MQTT to trigger track change in Unity here
                print('Moved to ' + track + ' Track')
                
                if connected:
                    rc = client.publish(topic, payload= (track), qos =0, retain=False)
                    print rc
                    
                just_changed = True
     
            else:
                just_changed = False 
                
                
        #cv2.cv.Flip(frame, None, 1)
        
        frame = cv2.flip(frame,1) #vertical flip to create mirror image
        if track:
            cv2.putText(frame, track, (10, 30), cv2.FONT_HERSHEY_SIMPLEX,
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
    parser.add_argument('--standalone', '-s', action = 'store_true', help='Run Script without MQTT publishing') 
    args = parser.parse_args()
    
    if args.standalone:
        localize()
        return
        
    elif args.ip:
        connect_to_server(args.ip)
    else:
        connect_to_server('localhost')
    
    connected = True
    localize()
    
if __name__ == '__main__':
    main()
