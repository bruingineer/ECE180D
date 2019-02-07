"""
Multiplayer server for Synchro
"""

import paho.mqtt.client as mqtt
import random
import logging
from time import sleep
from time import time

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
game/state = {waiting for players, ready, start, running, pause, game over, finish}

clients send their client id, the server responds on server/'client_id' with their player number
server/player_connected = {"", client id}
server/'client_id' = {player1, player2}

player 1:
player1/challenge = {"", challenge_type + challenge_data}
player1/challenge_status = {"", requested, sent, in progress}
player1/request_challenge = {"", requested, fulfilled}
player1/position = {"",0,1,...10}
player1/connection_status = {"",connected,disconnected}

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
    
    # request to join
    if mqtt.topic_matches_sub("server/player_connected", msg.topic):
   		_controller.addGameClient(str(msg.payload))

    # status of game clients
    if mqtt.topic_matches_sub("+/connection_status", msg.topic):
    	if str(msg.topic).find('player1') != -1:
    		_controller.player1.connection_status = str(msg.payload)
    	elif str(msg.topic).find('player2') != -1:
    		_controller.player2.connection_status = str(msg.payload)
    	else:
    		unrecognized_message = True

    # position updates
    elif mqtt.topic_matches_sub('+/position', msg.topic):
    	if str(msg.topic).find('player1') != -1:
    		_controller.game_clients[0].position = int(msg.payload)
    	elif str(msg.topic).find('player2') != -1:
    		_controller.game_clients[1].position = int(msg.payload)
    	else:
    		unrecognized_message = True

    # challenge request
    elif mqtt.topic_matches_sub('+/request_challenge', msg.topic):
    	if str(msg.topic).find('player1') != -1:
    		# send next challenge
    	elif str(msg.topic).find('player2') != -1:
    		# send next challenge
    	else:
    		unrecognized_message = True

    # # challenge status
    # elif mqtt.topic_matches_sub('+/position', msg.topic):
    # 	if str(msg.topic).find('player1') != -1:
    # 		_controller.game_clients[0].position = int(msg.payload)
    # 	elif str(msg.topic).find('player2') != -1:
    # 		_controller.game_clients[1].position = int(msg.payload)
    # 	else:
    # 		unrecognized_message = True

    else:
    	log.warning('on_message - message on unexpected topic')
    return

def connect_to_server(ip, port, client_id):
	log.info('*connect_to_server')
	client = mqtt.Client(client_id = client_id)
	client.on_connect = on_connect
	client.on_message = on_message
	client.connect(ip, port, 60)
	return client


class challengeGenerator:
	CHALLENGE_INDEX = ['big_lasers', 'small_lasers', 'word', 'gesture']
	GESTURES = ['tpose', 'rightHandRaise', 'leftHandRaise', 'fieldGoal', 'rightHandWave']
	NUMBER_OF_LANES = 10
	NUMBER_OF_WORD_CHALLENGES = 10


	def __init__(self):
		self.challenges = []
		return
	
	"""
	%challengeType% = {big_lasers, small_lasers, word, gesture}
	"""
	def createChallenge(self, challengeType):
		challenge_data = None
		if challengeType == 'gesture':
			gesture = -1
			gesture = challengeGenerator.GESTURES[random.randint(0, len(challengeGenerator.GESTURES)-1)]
			challenge_data = gesture 

		elif challengeType == 'big_lasers':
			# lanes to populate with lasers
			number_of_laser_lanes = 6
			lasers = []
			lasers = random.sample(xrange(0,challengeGenerator.NUMBER_OF_LANES),number_of_laser_lanes)
			challenge_data = lasers 

		elif challengeType == 'small_lasers':
			# number_of_small_lasers_to_fire = 15
			number_of_small_lasers_to_fire = random.randint(10,15)
			lasers = []
			for x in range(number_of_small_lasers_to_fire):
				lasers.append(random.randint(0,challengeGenerator.NUMBER_OF_LANES-1))
			challenge_data = lasers

		elif challengeType == 'word':
			word_index = -1
			word_index = random.randint(0,challengeGenerator.NUMBER_OF_WORD_CHALLENGES-1)
			challenge_data = word_index

		else:
			log.error("unrecognized challenge type.")

		return (challengeType, challenge_data)

	def generate(self, number_of_challenges = 25):
		for i in range(0, number_of_challenges):
			challenge = self.createChallenge(challengeGenerator.CHALLENGE_INDEX[random.randint(0,len(challengeGenerator.CHALLENGE_INDEX))-1])
			self.challenges.append(challenge)
		return

class game_client:
	def __init__(self, id):
		self.player_id = id
		self.current_challenge = 0
		self.position = 0
		return

	def getNextChallengeNumber(self):
		self.current_challenge += 1
		return self.current_challenge

	def incrementPosition(self):
		self.position += 1

class controller:

	def __init__(self):
		self.game_clients = []
		self.player_id_to_client_id = {}

		self.mqtt_client = mqtt.Client(client_id = 'game_server')
		self.mqtt_client.on_connect = on_connect
		self.mqtt_client.on_message = on_message
		self.mqtt_client.connect('localhost', '1883', 60)
		self.subscribeToGameTopics()
		self.mqtt_client.user_data_set(self)
		self.mqtt_client.loop_start()


	def subscribeToGameTopics(self):
		topics = [('test', 0), ('server/player_connected', 0), ('player1/#', 0), ('player2/#', 0)]
		self.mqtt_client.subscribe(topics)

	def addGameClient(self, client_id):
		payload = ''
    	if len(self.game_clients) < 2:
			player_id = 1+len(self.game_clients)
			payload = 'player{}'.format(player_id)
			new_game_client = game_client(player_id)
			self.game_clients.append(new_game_client)
		else:
			payload = '2 players connected already'

		self.mqtt_client.publish('server/{}'.format(client_id), payload)


	def sendChallengeTo(self, player_id):
		pass
		# get next challenge of player id
		# package
		# publish to topic

	# def correctGestureReceived(self, player_id):
	# 	# 
	def challengeRequestReceived(self):
		pass

def main():
	c = controller()

	while(1):
		print("{}: {}".format(time(), c._user))
		# if 2 clients connected, start game
		# if both clients disconnected, initialize topics

		# if position of player is the last spot, send winner message / loser message / game over

		# rest of logic is in message callback

if __name__ == '__main__':
	main()