# Mosquitto on MacOS
This guide assumes homebrew is already installed. If not, please install it before proceeding.

## Installing mosquitto through homebrew
mosquitto is an open source  mqtt server and client. It has a server/client version as well as just a client version.

1. Open *Terminal* and run
```
brew install mosquitto
```

2. a. To start the server with the default config, first try
```
brew services start mosquitto
```
   and then use `brew services list` to see if mosquitto is running.
If it is running, skip to step 3.

2. b. If it does not work try, run the following command to start the server (fill in the \*.plist with the actual file name, don’t leave it as an \*)
```
launchctl load /usr/local/opt/mosquitto/*.plist
```

3. Now, test the installation and ensure the server is running successfully.  Open a new *Terminal* window, and subscribe to a topic with
```
mosquitto_sub -t topic/state
```
   In another *Terminal* window, send a message to the same topic with
```
mosquitto_pub -t topic/state -m "Hello World"
```
   The message of “Hello World” should appear in the terminal window running the mosquitto_sub process.

## Adjusting configs
The default mosquitto binary command file is `/usr/local/sbin/mosquitto`. It may be symlinked.  
The default `mosquitto.conf` server configuration file used by the mosquitto brew service is `/usr/local/etc/mosquitto/mosquitto.conf`  

The conifg file for this project is `ECE180D/mosquitto/mosquitto.conf`. See that file for descriptions of the options used.

## PC to PC connections on the same LAN
1. Add the following line to the .conf, where `[port#]` is the arbitrary port where the server will listen for incoming connections.
```
listener [port#]
```
Also add this line to the .conf to change the default port for the `localhost`. This port will need to be used when sending messages to the server from the same machine that is hosting the server.
```
port [localport#]
```

2. Start the server on one computer.
```
brew services start mosquitto
```
OR
```
[/path/to/mosquitto/executable] -c [/path/to/mosquito.conf]
```

3. To send and receive messages to the server from the host laptop, first subscribe to a topic with
```
mosquitto_sub -h localhost -p [localport#] -t topic
```
where `[localport#]` is the same as in step 1, and then publish a message to the same topic with
```
mosquitto_pub -h localhost -p [localport#] -t topic -m "Sending from localhost..."
```

4. To send and receive messages on a remote laptop, first subscribe to the same topic as in step 3 with
```
mosquitto_sub -h [IP_of_server_machine] -p [port#] -t topic
```
where` [IP_of_server_machine]` is the IP of the computer running the mosquitto server process and `[port#]` is the same port used in step 1. To send a message to the same topic, use
```
mosquitto_pub -h [IP_of_server_machine] -p [port#] -t topic -m "Message send from remote machine"
```

**_begin not tested_**
## This section has NOT been tested yet
###Installing the Python Libraries
To create the link between Python and MQTT we need to install the Python Eclipse MQTT library.  Visit here for the latest downloads and follow the link to download the required version.  Specifically, I downloaded these Python Libraries.

Once downloaded, unpack the tar file and install the library
```
tar -xvf org.eclipse.pho.mqtt.python-1.1.tar
cd org.eclipse.pho.mqtt.python-1.1
sudo python setup.py install
```
**_end not tested_**

# MQTT Unity Client Setup
1. Download the github repo https://github.com/vovacooper/Unity3d_MQTT .
2. Unzip the .zip
3. Load the package into Unity Project  
  a. From the menu bar, choose `Assets -> Import Package -> Custom Package…`  
  b. Navigate to Unity3d_MQTT-master/Packages/unity3d_mqtt.unitypackage
4. Import All
5. Open mqttTest.cs
6. Edit the following line to match the mosquitto server network info
```
client = new MqttClient(IPAddress.Parse("127.0.0.1"),[localport#] , false , null );
```
In this case, the loopback IP of 127.0.0.1 and the [localport#] were used because the mosquitto server is on the same computer running unity. Replace the [localport#] with the same port # that was set for localhost in the .conf file in step 1 of **PC to PC connections on the same LAN**.
7. Make sure the topics in the mqttTest.cs file are correct, as well. 
```
client.Subscribe(new string[] { "topic" }, new byte[] {MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
```
   And 
```
client.Publish("topic", System.Text.Encoding.UTF8.GetBytes("Sending from Unity3D!!!"),MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
```
The unity console may print out the sent and received messages.

## To test on macOS
1. In Terminal
```
brew services start mosquitto
mosquitto_sub -h localhost -p port# -t topic 
```
2. In unity
  a. **Skip this step unless unity throws an error about an object being undeclared, assigned, or referenced, or something** Run mqttTest.Start()
  b. Play the mqtt_test/MQTT scene
  c. Click over to the unity console to see console messages
  d. Click the button “level 1”
  e. Unity console should say it sent and received a message
3. In Terminal
  a. `Sending from Unity3D!!!` should have appeared in the window that was running mosquitto_sub

mqtt Unity sources
* http://blog.jorand.io/2017/08/02/MQTT-on-Unity/
* https://github.com/vovacooper/Unity3d_MQTT


# Mosquitto FAQ
### Q. How to start server?
A. https://mosquitto.org/man/mosquitto-8.html
   * How to start server with certain configuration?
      `[/path/to/mosquitto/executable] -c [/path/to/mosquitto.conf]`
### Q. What’s the default port of the mosquitto server?
A. 1883
### Q. How to adjust the mosquitto.conf server configuration
A. https://mosquitto.org/man/mosquitto-conf-5.html
* Format: All lines with a # as the very first character are treated as a comment. Configuration lines start with a variable name. The variable value is separated from the name by a single space.
* Defaults
   * No authentication
   * Port: 1883
### Q. `brew services …` does not work. What do I do?
A. If you don’t have “brew services” installed, run `brew services` in *Terminal*.
Additional `brew services` info: https://github.com/Homebrew/homebrew-services

#### mosquitto on macOS sources used to make this readme:
* https://simplifiedthinking.co.uk/2015/10/03/install-mqtt-server/
* https://mosquitto.org/download/
* https://mosquitto.org/man/mosquitto-conf-5.html
* http://blog.argot-sdk.org/2013/06/dummies-guide-to-installing-mosquitto.html

