import os
import sys
import subprocess
import time
import signal

gestures_dir = sys.path[0] + '/'
print(gestures_dir)
single_player_openpose_file = "singleplayer-openpose.py"
multiplayer_openpose_file = "multiplayer-openpose.py"
print(gestures_dir + single_player_openpose_file)

FNULL = open(os.devnull, 'w')
single_process = subprocess.Popen(["python", single_player_openpose_file], stdout=FNULL)#gesture_dir + single_player_openpose_file)
print("still me")

time.sleep(10)
single_process.kill()