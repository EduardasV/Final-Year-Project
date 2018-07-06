import numpy as np
import cv2 as cv
import socket
import json
import tensorflow as tf
from keras.models import load_model
import utils
import argparse
import matplotlib.image as mpimg

#server information     data.decode('utf-8')
host = '' 
port = 50000 
backlog = 5 
size = 1024 
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
s.bind((host,port)) 
s.listen(backlog)

#1122

config = tf.ConfigProto()
config.gpu_options.allow_growth = True
session = tf.Session(config=config)
session

modelRB = load_model('RB.h5')
modelTRB = load_model('RB.h5')
modelLB = load_model('LB.h5')
modelTLB = load_model('TLB.h5')

while 1:
    #connect to server
    client, address = s.accept() 
    print ("Client connected.")

    while 1: 
        data = client.recv(size).rstrip(b'\r\n')
        if data:
            data_array = data.decode('utf-8').split("|")
            #removed speed
            global target_steering
            target_steering = data_array[1]
            image = cv.imread('MainFrame.png', 0)
            edges = cv.Canny(image,50,190)
            try:
                images = np.empty([1, 100, 150])
                image = np.asarray(image)
                images[0] = utils.preprocess(image)
                current_target = float(target_steering)
                print(data_array[0])
                if data_array[0] == "LB":
                    steering_angle = float(modelLB.predict(images, batch_size=1))
                elif data_array[0] == "RB":
                    steering_angle = float(modelRB.predict(images, batch_size=1))
                elif data_array[0] == "TRB":
                    steering_angle = float(modelTRB.predict(images, batch_size=1))
                elif data_array[0] == "TLB":
                    steering_angle = float(modelTLB.predict(images, batch_size=1))
            
                global speed_limit
                if current_target > 5 or current_target < -5:
                    speed_limit = 13
                elif current_target > 20 or current_target < -20:
                    speed_limit = 5
                else:
                    speed_limit = 20
                    
                    #print('{}|{}'.format(steering_angle, speed))
                sendtounity = "{}|{}\n".format(steering_angle, speed_limit)
                sendtounity = bytes(sendtounity, 'utf-8')
                print(sendtounity)
                client.send(sendtounity)

                
            except Exception:
                pass



    
