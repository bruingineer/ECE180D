"""
module to define gestures and poses

"""

import numpy as np
from time import time

DEBUG_PROCESS_KEYPOINTS = False

# dictionary to convert body part names to the body_25 indexes
body25 = {
        "Nose":      0,
        "Neck":      1,
        "RShoulder": 2,
        "RElbow":    3,
        "RWrist":    4,
        "LShoulder": 5,
        "LElbow":    6,
        "LWrist":    7,
        "MidHip":    8,
        "RHip":      9,
        "RKnee":    10,
        "RAnkle":   11,
        "LHip":     12,
        "LKnee":    13,
        "LAnkle":   14,
        "REye":     15,
        "LEye":     16,
        "REar":     17,
        "LEar":     18,
        "LBigToe":  19,
        "LSmallToe":20,
        "LHeel":    21,
        "RBigToe":  22,
        "RSmallToe":23,
        "RHeel":    24,
        "Background":25
    }

class keypointFrames:
    """
    reference for coordinates given in keypoints np array
    (WIDTH,0)---------(0,0)
          |             |
          |             |
    (WIDTH,HEIGHT)----(0,HEIGHT)

    keypoints shape [people x parts x index] == (1L, 25L, 3L)
    index[] = [x , y , confidence]
    """

    def __init__(self):
        self.last_3_frames = []

    def add(self, _keypoints, inputWIDTH, inputHEIGHT):
        self.keypoints = np.array(_keypoints)
        # print(self.keypoints)
        # print('*********')
        if len(self.last_3_frames) < 3:
            self.last_3_frames.append((time(),self.keypoints))
            #print(self.last_3_frames)
        else:
            self.last_3_frames.pop(0)
            self.last_3_frames.append((time(),self.keypoints))

        #np.array(keypoints)
        # self.num_people = self.keypoints.shape[1]
        self.WIDTH = inputWIDTH
        self.HEIGHT = inputHEIGHT

    def checkFor(self, _target_gesture):
        #print ("CheckFor " + _target_gesture)
        _target_gesture = _target_gesture.lower()
        if _target_gesture == "tpose":
            return self.isTPose()
        elif _target_gesture == "fieldgoal":
            return self.isFieldGoal()
        elif _target_gesture == "righthandwave":
            return self.isRightHandRightToLeftWave()
        elif _target_gesture == "lefthandwave":
            return self.isLeftHandLeftToRightWave()
        elif _target_gesture == "lefthandraise":
            return self.isRaiseLeftHand()
        elif _target_gesture == "righthandraise":
            return self.isRaiseRightHand()
        elif _target_gesture == "righthanddab":
            return self.isDab()
        else:
            print("gesture "+_target_gesture+" not recognized.")

    def isDab(self):
        parts = [body25[x] for x in ['RWrist', 'Neck', 'RElbow', 'RShoulder','LElbow', 'LShoulder', 'LWrist']]
        if not np.all(self.keypoints[0,parts,:]):
            print('not enough keypoints for dab!')
            return False

        wristAndNeckCorrect = False
        # RWrist X is close to neck X
        if (self.WIDTH * 0.1 > abs(self.keypoints[0,body25['RWrist'],0] - self.keypoints[0,body25['Neck'],0])):
            # RWrist above neck
            print('RWrist / Neck X pass')
            if (self.keypoints[0,body25['RWrist'],1] < self.keypoints[0,body25['Neck'],1]):
                print('RWrist / Neck Y pass')
                wristAndNeckCorrect = True

        rightElbowCorrect = False
        # right elbow placement 
        if (self.keypoints[0,body25['RElbow'],0] < self.keypoints[0,body25['RShoulder'],0]):
            print('RElbow / RShoulder X pass')
            rightElbowCorrect = True

        # left arm check is straight and up a bit
        leftArmCorrect = False
        leftArmX = self.keypoints[0,[body25[x] for x in ['LShoulder', 'LElbow', 'LWrist']],0]
        leftArmY = self.keypoints[0,[body25[x] for x in ['LShoulder', 'LElbow', 'LWrist']],1]
        if (np.array_equal(leftArmX, np.sort(leftArmX, axis=None)[::])):
            print('left arm is in order to the left')
            # check goes up
            if(np.array_equal(leftArmY, np.sort(leftArmY, axis=None)[::-1])):
                print('left arm goes up')
                if(self.HEIGHT * 0.15 < (leftArmY.max() - leftArmY.min())):
                    print('left arm goes up Enough')
                    leftArmCorrect = True

        return wristAndNeckCorrect and rightElbowCorrect and leftArmCorrect

    def isRightHandRightToLeftWave(self):
        x = []
        y = []
        if DEBUG_PROCESS_KEYPOINTS:
            print("Wave - last3frames length: {}".format(len(self.last_3_frames)))
        
        # get the right wrist x-y coordinates from the last 3 frames array
        for i in range(len(self.last_3_frames)):
            frame = self.last_3_frames[i][1]
            x.append(frame[0,body25['RWrist'],0])
            y.append(frame[0,body25['RWrist'],1])

        if DEBUG_PROCESS_KEYPOINTS:
            print("Wave - processed x: {0}".format(x))
        
        npx = np.array([z for z in x if z>0])
        npy = np.array([z for z in y if z>0])

        # TODO: normalize threshold to skeleton size
        if len(npy) > 1:
            # check for minimal y movement
            if ( ((npy.max() - npy.min())/self.HEIGHT) < 0.3 ):
                # check for x coordinates are in order which means movement in one directin
                # check for movement across .4 of the screen width
                # TODO normalize sizes
                if ( np.array_equal(np.sort(npx, axis=None)[::-1], npx) ) and ( ((npx.max() - npx.min())/self.WIDTH) > 0.4 ):
                    return True
        return False

    def isLeftHandLeftToRightWave(self):
        x = []
        y = []
        if DEBUG_PROCESS_KEYPOINTS:
            print("Wave - last3frames length: {}".format(len(self.last_3_frames)))
        
        # get the right wrist x-y coordinates from the last 3 frames array
        for i in range(len(self.last_3_frames)):
            frame = self.last_3_frames[i][1]
            x.append(frame[0,body25['LWrist'],0])
            y.append(frame[0,body25['LWrist'],1])

        if DEBUG_PROCESS_KEYPOINTS:
            print("Wave - processed x: {0}".format(x))
        
        npx = np.array([z for z in x if z>0])
        npy = np.array([z for z in y if z>0])

        # TODO: normalize threshold to skeleton size
        if len(npy) > 1:
            # check for minimal y movement
            if ( ((npy.max() - npy.min())/self.HEIGHT) < 0.3 ):
                # check for x coordinates are in order which means movement in one directin
                # check for movement across .4 of the screen width
                # TODO normalize sizes
                if ( np.array_equal(np.sort(npx, axis=None)[::], npx) ) and ( ((npx.max() - npx.min())/self.WIDTH) > 0.4 ):
                    return True
        return False

    # checks for arms straight out from sides, using passed in keypoints to class
    # returns true if in TPose
    def isTPose(self):
        #print ("checking for tpose")
        # 1D array: y of [body25 1 thru 7]
        parts = [body25[x] for x in ['RWrist','RElbow','RShoulder','Neck','LShoulder','LElbow','LWrist']]

        # get y
        a = self.keypoints[0, parts, 1].flat
        TPoseKeypoints_y = a[a > 0]
        # get x
        a = self.keypoints[0, parts, 0].flat
        TPoseKeypoints_x = a[a > 0]
        
        if DEBUG_PROCESS_KEYPOINTS:
            print('tpose - y: {}'.format(self.TPoseKeypoints_y))
            print('tpose - x: {}'.format(self.TPoseKeypoints_y1))

        # check for at least 6 of the 8 points are detected and their y variation is within threshold
        threshold = 0.1
        if (TPoseKeypoints_y.size >= 6) and (
            (((TPoseKeypoints_y.max() - TPoseKeypoints_y.min()) / self.HEIGHT)) < threshold):
            if DEBUG_PROCESS_KEYPOINTS:
                print("tpose - testing for x order")
            if (np.array_equal(np.sort(TPoseKeypoints_x, axis=None), TPoseKeypoints_x)):
                return True
        else:
            return False

    def isFieldGoal(self):
        # get parts needed for this pose
        elbow2elbow_indexes = [body25[x] for x in ['Neck','RShoulder','RElbow','LElbow','LShoulder']]
        a = self.keypoints[0,elbow2elbow_indexes,1].flat
        self.elbowToElbow_y = a[a>0]
        
        # check that elbows and shoulders are horizontal within threshold
        threshold = 0.2
        isElbowsCorrect = False
        if (self.elbowToElbow_y.size >= 4) and (abs(((self.elbowToElbow_y.max() - self.elbowToElbow_y.min()) / self.HEIGHT)) < threshold):
            isElbowsCorrect = True

        isHandsCorrect = False
        # check if wrist and elbox x coords are within in threshold
        if np.count_nonzero(self.keypoints[0,[body25[x] for x in ['RElbow','RWrist','LWrist','LElbow']],0]) == 4:
            if DEBUG_PROCESS_KEYPOINTS:
                print('field goal - non zero check')
            if (abs(self.keypoints[0,body25['RWrist'],0]-self.keypoints[0,body25['RElbow'],0]) / self.WIDTH) < threshold:
                if DEBUG_PROCESS_KEYPOINTS:
                    print("field goal - Right pass")
                if (abs(self.keypoints[0,body25['LWrist'],0]-self.keypoints[0,body25['LElbow'],0]) / self.WIDTH) < threshold:
                    if DEBUG_PROCESS_KEYPOINTS:
                        print('field goal - left pass')
                    # check if wrists are above nose
                    if (self.keypoints[0,body25['RWrist'],1] < self.keypoints[0,body25['Nose'],1]):
                        if (self.keypoints[0,body25['LWrist'],1] < self.keypoints[0,body25['Nose'],1]):
                            isHandsCorrect = True

        return isHandsCorrect and isElbowsCorrect

    #TODO normalize and add checks for other arm is down
    # left hand raised
    def isRaiseLeftHand(self):
        indexes = [body25[x] for x in ['LElbow','LShoulder','LWrist']]
        a = self.keypoints[0,indexes,1].flat
        y = a[a>0]
        a = self.keypoints[0,indexes,0].flat
        x = a[a>0]
        
        #checking for low change in x ==> arm is straight up
        threshold = 0.2
        if ( y.size == 3 ) and (abs(((x.max() - x.min()) / self.HEIGHT)) < threshold):
            # check if wrists are above nose
            if (self.keypoints[0,body25['LWrist'],1] < self.keypoints[0,body25['Nose'],1]):
                return True

        return False

    # right hand raised
    def isRaiseRightHand(self):
        indexes = [body25[x] for x in ['RElbow','RShoulder','RWrist']]
        # print(self.keypoints.size)
        a = self.keypoints[0,indexes,1].flat
        y = a[a>0]
        a = self.keypoints[0,indexes,0].flat
        x = a[a>0]
        
        #checking for low change in x ==> arm is straight up
        threshold = 0.2
        if ( y.size == 3 ) and (abs(((x.max() - x.min()) / self.HEIGHT)) < threshold):
            # check if wrists are above nose
            if (self.keypoints[0,body25['RWrist'],1] < self.keypoints[0,body25['Nose'],1]):
                return True

        return False