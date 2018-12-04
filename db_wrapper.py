import argparse
import json
import paho.mqtt.client as mqtt
import mysql.connector

#servers
client = None
mydb = None
mycursor = None

#Other globals 
CONNECTED = False
topic = 'database'

# The callback for when the client receives a CONNACK response from the server.
def on_connect(client, userdata, flags, rc):
    print("Connected with result code "+str(rc))

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    #client.subscribe("$SYS/#")
    client.subscribe(topic, qos=0)

# The callback for when a PUBLISH message is received from the server.
# First word MUST be UPDATE, SELECT, or INSERT
def on_message(client, userdata, msg):
    content = str(msg.payload)
    print(msg.topic+" "+str(content))
    
    cmd_type = content.upper().split()[0]
    
    if cmd_type == 'DISCONNECT':
        print('Disconnecting!')
        client.disconnect()
        return
        
    mycursor.execute(content)
    
    try:
        if cmd_type == 'SELECT':
            result = mycursor.fetchall()
            result_dict = [dict(zip([key[0] for key in mycursor.description], row)) for row in result]
            json_str = json.dumps({'count': len(result_dict), 'items': result_dict})  
            
            print(json_str)
            
            rc = client.publish(topic + '/result', payload= (json_str), qos =0, retain=False)
            print(rc)
        else:
            print("Committing to db w/ " + cmd_type + " command")
            mydb.commit()
    except Exception as e:
        print(e)

def connect_to_db(ip):
    global mydb, mycursor
    
    mydb = mysql.connector.connect(
      host = ip,
      user = "root",
      passwd = "password",
      database = "Synchro"
    )

    mycursor = mydb.cursor()
    
     
def connect_to_server(ip): 
    global client

    client = mqtt.Client(client_id = 'db_wrapper.py')
    client.on_connect = on_connect
    client.on_message = on_message

    client.connect(ip, 1883, 60)
    
def main():

    parser = argparse.ArgumentParser(description='Wrapper for mysql commands from unity')
    parser.add_argument('--ip', type=str, action = 'store', default = 'localhost', help='IP address of machine running Mosquitto server')
    parser.add_argument('--dbip', type=str, action = 'store', default = 'localhost', help='IP address of machine running MySQL server')
    parser.add_argument('--standalone', '-s', action = 'store_true', help='Run script withou') 
    args = parser.parse_args()
    
    if args.standalone:
        return
    
    connect_to_server(args.ip)
    connect_to_db(args.ip)
    
    client.loop_forever()
   
 
if __name__ == '__main__':
    main()