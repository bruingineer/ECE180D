"""
Multiplayer server for Synchro
"""

import paho.mqtt.client as mqtt
import random
import logging

log = logging.getLogger(__name__)

def connect_to_server(ip, port, client_id):
	client = mqtt.Client(client_id = client_id)
    client.on_connect = on_connect
    client.on_message = on_message
    client.connect(ip, port, 60)
    return client

def on_connect(client, userdata, flags, rc):
	log.info("Connection returned result: {}".format(connack_string(rc)))
	return

def on_message(client, userdata, msg):
    log.info("mqtt - topic: "+msg.topic+" - message: "+str(msg.payload))
    return

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
		self.game_client_id = id
		self.currentChallenge = 0
		self.position = 0
		return

	def getNextChallengeNumber(self):
		self.currentChallenge += 1
		return self.currentChallenge

	def incrementPosition(self):
		self.position += 1

class controller:
	def __init__(self):
		self.mqtt_client = connect_to_server(ip="IP", port="port", client_id="game_server")
		# self.mqtt_client.subscribe(topic, qos=0)
		self.mqtt_client.loop_start()

	def sendChallengeTo(self, player_id):
		# get next challenge of player id
		# package
		# publish to topic

	# def correctGestureReceived(self, player_id):
	# 	# 
	def challengeRequestReceived(self, )

def main():

if __name__ == '__main__':
	main()