"""
Multiplayer server for Synchro
"""

import paho.mqtt.client as mqtt
import random
import logging
import argparse
from time import sleep
from time import time
import json

# create logger
log = logging.getLogger(__name__)
log.setLevel(logging.DEBUG)

# create console handler and set level to debug
ch = logging.StreamHandler()
ch.setLevel(logging.DEBUG)

# create formatter
formatter = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')

# add formatter to ch
ch.setFormatter(formatter)

# add ch to logger
log.addHandler(ch)

"""
TOPIC SUMMARY

server should init all topics with retain messages when started and after game is finished
init values listed first

server:
game/state = {waiting for players, start, running, pause, game over, finish}

clients send their client id, the server responds on server/'client_id' with their player number
server/player_connected = {"", client id}
server/'client_id' = {player1, player2}

player 1:
player1/challenge = {"", challenge_type + challenge_data}
player1/request_challenge = {"", requested, fulfilled}
player1/position = {"",0,1,...10}
player1/connection_status = {"",ready,not ready,I WON,disconnected}
player1/winner_notification = {'', winner, loser, you suck}

player 2: 
same as player1

"""

def on_connect(client, userdata, flags, rc):
    log.info("Connection returned result: {}".format(connack_string(rc)))
    return

# userdata is controller class object
def on_message(client, _controller, msg):
    log.info("on_message - topic: "+msg.topic+" - message: "+str(msg.payload))
    unrecognized_message = False
    
    if msg.payload == "":
        log.info("empty string message")
        return

    # request to join
    if mqtt.topic_matches_sub("server/player_connected", msg.topic):
        _controller.addGameClient(str(msg.payload))

    # status of game clients
    elif mqtt.topic_matches_sub("+/connection_status", msg.topic):
        #disconnect or connect    
        if str(msg.topic).find('player1') != -1:
            _controller.game_clients[0].connection_status = str(msg.payload)
        elif str(msg.topic).find('player2') != -1:
            _controller.game_clients[1].connection_status = str(msg.payload)
        else:
            unrecognized_message = True

    # # position updates
    # elif mqtt.topic_matches_sub('+/position', msg.topic):
    #     if str(msg.topic).find('player1') != -1:
    #         _controller.game_clients[0].position = int(msg.payload)
    #     elif str(msg.topic).find('player2') != -1:
    #         _controller.game_clients[1].position = int(msg.payload)
    #     else:
    #         unrecognized_message = True

    # challenge request
    elif mqtt.topic_matches_sub('+/request_event', msg.topic):
        if msg.payload == 'requested':
            if str(msg.topic).find('player1') != -1:
                # send next challenge
                _controller.sendChallengeTo(1, 'event')

            elif str(msg.topic).find('player2') != -1:
                # send next challenge
                _controller.sendChallengeTo(2, 'event') 

        else:
            unrecognized_message = True

    elif mqtt.topic_matches_sub('+/request_obstacle', msg.topic):
        if msg.payload == 'requested':
            if str(msg.topic).find('player1') != -1:
                # send next challenge
                _controller.sendChallengeTo(1, 'obstacle')

            elif str(msg.topic).find('player2') != -1:
                # send next challenge
                _controller.sendChallengeTo(2, 'obstacle') 

        else:
            unrecognized_message = True

    else:
        log.warning('on_message - unexpected topic')

    if unrecognized_message:
        log.warning('on_message - unrecognized_message on \'{}\''.format(msg.topic))
    return

# def connect_to_server(ip, port, client_id):
#   log.info('*connect_to_server')
#   client = mqtt.Client(client_id = client_id)
#   client.on_connect = on_connect
#   client.on_message = on_message
#   client.connect(ip, port, 60)
#   return client


class challengeGenerator:
    EVENTS_INDEX = ['word', 'gesture', 'trivia']
    GESTURES = ['tpose', 'rightHandRaise', 'leftHandRaise', 'fieldGoal', 'rightHandWave']
    OBSTACLES_INDEX = ['big_lasers', 'small_lasers']
    NUMBER_OF_LANES = 10
    NUMBER_OF_WORD_CHALLENGES = 16
    NUMBER_OF_TRIVIA_CHALLENGES = 9

    CHALLENGE_INDEX = EVENTS_INDEX+OBSTACLES_INDEX

    def __init__(self):
        self.challenges = []
        return
    
    """
    %challengeType% = {big_lasers, small_lasers, word, gesture}
    """
    def createChallenge(self, challengeType):
        challenge_data = None
        log.info('chal: {}'.format(challengeType))

        # pick from events or obstacles
        if challengeType == 'event':
            challengeType = challengeGenerator.EVENTS_INDEX[random.randint(0,len(challengeGenerator.EVENTS_INDEX)-1)]
        elif challengeType == 'obstacle':
            challengeType = challengeGenerator.OBSTACLES_INDEX[random.randint(0,len(challengeGenerator.OBSTACLES_INDEX)-1)]
        log.info('chal type: {}'.format(challengeType))

        if challengeType == 'gesture':
            gesture = -1
            gesture = challengeGenerator.GESTURES[random.randint(0, len(challengeGenerator.GESTURES)-1)]
            challenge_data = gesture 

        elif challengeType == 'big_lasers':
            # lanes to populate with lasers
            number_of_laser_lanes = random.randint(6,9)
            lasers = []
            lasers = random.sample(xrange(0,challengeGenerator.NUMBER_OF_LANES),number_of_laser_lanes)
            challenge_data = lasers 

        elif challengeType == 'small_lasers':
            # number_of_small_lasers_to_fire = 15
            number_of_small_lasers_to_fire = 12
            lasers = []
            for x in range(number_of_small_lasers_to_fire):
                lasers.append(random.randint(0,challengeGenerator.NUMBER_OF_LANES-1))
            challenge_data = lasers

        elif challengeType == 'word':
            word_index = -1
            word_index = random.randint(0,challengeGenerator.NUMBER_OF_WORD_CHALLENGES-1)
            challenge_data = word_index

        elif challengeType == 'trivia':
            trivia_index = -1
            trivia_index = random.randint(0,challengeGenerator.NUMBER_OF_TRIVIA_CHALLENGES-1)
            challenge_data = trivia_index

        else:
            log.error("unrecognized challenge type.")

        log.info('{}, {}'.format(challengeType, challenge_data))
        return (challengeType, challenge_data)

    def generate(self, number_of_challenges = 25):
        for i in range(0, number_of_challenges):
            challenge = self.createChallenge(challengeGenerator.CHALLENGE_INDEX[random.randint(0,len(challengeGenerator.CHALLENGE_INDEX))-1])
            self.challenges.append(challenge)
        return self.challenges

class game_client:
    def __init__(self, id):
        self.player_id = id
        self.current_challenge = 0
        self.position = 0
        self.connection_status = ''
        return

    def getNextChallengeNumber(self):
        self.current_challenge += 1
        return self.current_challenge

class controller:

    def __init__(self, ip):
        self.challenge_generator = challengeGenerator()

        self.game_clients = []
        self.player_id_to_client_id = {}

        self.state = 'waiting for players'

        self.mqtt_client = mqtt.Client(client_id = 'game_server')
        self.mqtt_client.on_connect = on_connect
        self.mqtt_client.on_message = on_message
        self.mqtt_client.connect(ip, '1883', 60)
        self.subscribeToGameTopics()
        self.initTopics()
        self.mqtt_client.user_data_set(self)
        self.mqtt_client.loop_start()

    def subscribeToGameTopics(self):
        topics = [('test', 0), ('server/player_connected', 0), ('player1/#', 0), ('player2/#', 0)]
        self.mqtt_client.subscribe(topics)
        return 

    def createChallenges(self, number):
        self.challenge_generator.generate(90)
        return

    def addGameClient(self, client_id):
        payload = ''
        # add game client if there is room, else reject request
        if len(self.game_clients) < 2:
            # player1 = game_clients[0], player2 = game_clients[1]
            player_id = 1+len(self.game_clients)
            payload = 'player{}'.format(player_id)
            new_game_client = game_client(player_id)
            self.game_clients.append(new_game_client)
        else:
            payload = '2 players connected already'
        self.mqtt_client.subscribe(topic='server/{}'.format(client_id), qos=0)
        self.mqtt_client.publish(topic='server/{}'.format(client_id), payload=payload)
        return

    def sendChallengeTo(self, player_id, challengeType):
        # challenge_inx = self.game_clients[player_id-1].getNextChallengeNumber()
        # challenge_type, challenge_data = self.challenge_generator.challenges[challenge_inx]

        challenge_type, challenge_data = self.challenge_generator.createChallenge(challengeType)
        payload = json.dumps({"challenge" : challenge_type, "data" : challenge_data})

        self.mqtt_client.publish(topic='player{}/{}'.format(player_id, challengeType), payload=payload)
        self.mqtt_client.publish(topic='player{}/request_{}}'.format(player_id, challengeType), payload='fulfilled')

    def readyToStart(self):
        for gc in self.game_clients:
            log.info("readyToStart - player{} is {}".format(gc.player_id, gc.connection_status))
        if len(self.game_clients) != 2:
            return False
        return (self.game_clients[0].connection_status == 'ready' and self.game_clients[1].connection_status == 'ready')

    def startGame(self):
        self.mqtt_client.publish(topic='game/state', payload='start')

    def initTopics(self):
        """
        INIT VALUES 

        server:
        game/state = 'waiting for players'
        server/player_connected = ""

        player 1:
        player1/challenge = ""
        player1/challenge_status = ""
        player1/request_challenge = ""
        player1/position = ""
        player1/connection_status = ""

        player 2: 
        same as player1

        """
        self.mqtt_client.publish(topic='game/state', payload='', retain=True)
        self.mqtt_client.publish(topic='server/player_connected', payload='', retain=True)

        self.mqtt_client.publish(topic='player1/challenge', payload='', retain=True)
        self.mqtt_client.publish(topic='player1/challenge_status', payload='', retain=True)
        self.mqtt_client.publish(topic='player1/request_challenge', payload='', retain=True)
        self.mqtt_client.publish(topic='player1/position', payload='', retain=True)
        self.mqtt_client.publish(topic='player1/connection_status', payload='', retain=True)

        self.mqtt_client.publish(topic='player2/challenge', payload='', retain=True)
        self.mqtt_client.publish(topic='player2/challenge_status', payload='', retain=True)
        self.mqtt_client.publish(topic='player2/request_challenge', payload='', retain=True)
        self.mqtt_client.publish(topic='player2/position', payload='', retain=True)
        self.mqtt_client.publish(topic='player2/connection_status', payload='', retain=True)
        return

    def cleanUp(self):
        self.game_clients = []
        self.challenge_generator.challenges = []
        self.initTopics()

def main(args):
    c = controller(args[0].ip)
    # c.createChallenges(50)


    # game/state = {waiting for players, ready, start, running, pause, game over, finish}
    previous_state = c.state

    while(1):
        sleep(0.1)
        # log.info("length of game clients: {}".format(len(c.game_clients)))
        if c.state != previous_state:
            c.mqtt_client.publish(topic='game/state', payload=c.state, retain=True)

        previous_state = c.state

        # waiting for players
        if c.state == 'waiting for players':
            #code
            # if 2 clients connected, start game
            if c.readyToStart():
                c.state = 'start'

        elif c.state == 'start':
            c.startGame()
            c.state = 'running'

        elif c.state == 'running':
            #more more code
            for i in range(1,len(c.game_clients)+1):
                if c.game_clients[i-1].connection_status == 'I WON':
                    c.mqtt_client.publish(topic='player{}/winner_notification'.format(i), payload='winner')
                    c.mqtt_client.publish(topic='player{}/winner_notification'.format((i+1)%2), payload='loser')
                    c.state = 'game over'

        elif c.state == 'pause':
            #code 
            pass
        elif c.state == 'game over':
            #more code
            # clients should send message
            for i in range(1, 3):
                c.mqtt_client.publish(topic='player{}/connection_status'.format(i), payload='not ready')

            c.state = 'waiting for players'
        else:
            log.error('invalid state!')


        # if both clients disconnected, initialize topics

        # if position of player is the last spot, send winner message / loser message / game over

        # rest of logic is in message callback

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--ip", default=None, help="Set the ip of the Mqtt server.")

    args = parser.parse_known_args()
    main(args)