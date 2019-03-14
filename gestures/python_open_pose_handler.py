import os
import sys
gestures_dir = sys.path[0] + '/'
single_player_openpose_file = "singleplayer-openpose.py"
multiplayer_openpose_file = "multiplayer-openpose.py"
print(gestures_dir + single_player_openpose_file)
os.system(gestures_dir + single_player_openpose_file)