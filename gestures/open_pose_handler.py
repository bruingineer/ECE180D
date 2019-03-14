import os
import sys
import subprocess
import paho.mqtt.client as mqtt

gestures_dir = sys.path[0] + '/'
single_player_openpose_file = "singleplayer-openpose.py"
multiplayer_openpose_file = "multiplayer-openpose.py"
FNULL = open(os.devnull, 'w')
single_player = 'single_player'
multiplayer = 'multiplayer'
open_pose_topic = 'openpose/change'

class controller:

    def __init__(self):
        self.state = single_player
        self.mqtt_client = mqtt.Client(client_id = 'openpose_handler')
        self.mqtt_client.on_message = self.on_message
        self.mqtt_client.connect('127.0.0.1', '1883', 60)
        self.mqtt_client.subscribe([(open_pose_topic, 0)])
        self.mqtt_client.loop_start()
        self.proc = None
        self.start_proc()

    def on_message(self, client, _controller, msg):
        payload = msg.payload
        topic = msg.topic
        print(payload)
        # request to change script
        if mqtt.topic_matches_sub(open_pose_topic, topic):
            if payload in [single_player, multiplayer]:
                self.handle_state(payload)
                return
    
        return
    
    
    def handle_state(self, state):
        print('Cur State: {}'.format(self.state))
        print('Requesting to change to {}'.format(state))
        print(self.state != state)
        if state != self.state:
            self.state = state
            self.start_proc()
            return
    
    def start_proc(self):
        if self.proc:
            print('Killing process...')
            self.proc.kill()
        
        file = ''
        if self.state == single_player:
            file = single_player_openpose_file
        else:
            file = multiplayer_openpose_file

        print("Starting {}".format(file))

        self.proc = subprocess.Popen(["python", file], stdout=FNULL)

    def start_loop(self):
        while True:
            continue

_controller = controller()
_controller.start_loop()

