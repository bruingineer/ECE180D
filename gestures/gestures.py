import argparse
import cv2
import time
import paho.mqtt.client as mqtt


CONNECTED = False
client = None
topic = 'movement'

# The callback for when the client receives a CONNACK response from the server.
def on_connect(client, userdata, flags, rc):
    print("Connected with result code "+str(rc))

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    #client.subscribe("$SYS/#")

# The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, msg):
    print(msg.topic+" "+str(msg.payload))

    
def gesture1():
    pass
    
def gesture2():
    pass
    
def connect_to_server(ip): 
    global client

    client = mqtt.Client(client_id = 'gesture.py')
    client.on_connect = on_connect
    client.on_message = on_message

    client.connect(ip, 1883, 60)
    
    client.subscribe(topic, qos=0)
    

def test():
    while True:
        time.sleep(5)
        rc = client.publish(topic, payload= ('correct'), qos =0, retain=False)
        print(rc)
        
def main():

    parser = argparse.ArgumentParser(description='Localization Script for Synchro')
    parser.add_argument('--ip', type=str, action = 'store', default = None, help='IP address of machine running Unity')
    parser.add_argument('--standalone', '-s', action = 'store_true', help='Run script without MQTT publishing') 
    args = parser.parse_args()
    
    if args.standalone:
        return
    
    elif args.ip:
        connect_to_server(args.ip)
    else:
        connect_to_server('localhost')
    
    #test()
    client.loop_start()
    test()
   
 
if __name__ == '__main__':
    main()