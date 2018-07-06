from keras.layers import Lambda, Conv2D, MaxPooling2D, Dropout, Dense, Flatten
from sklearn.model_selection import train_test_split
from utils import INPUT_SHAPE, batch_generator
from keras.callbacks import ModelCheckpoint
from keras.models import Sequential
from keras.optimizers import Adam
import tensorflow as tf
import pandas as pd
import numpy as np
import cv2 as cv
import argparse
import socket
import time
import json
import csv
import os



filename = "DataTRB.csv"

ifile = open(filename, "rU")
reader = csv.reader(ifile, delimiter=",")

rownum = 0	
a = []	
b = []

for row in reader:
    a.append (os.path.join('../TRB/{0}/{1}'.format(row[1], row[0])))
    b.append (row[1])
    
ifile.close()

#print("\n".join(a))
#print("\n".join(b))
print(a[1])

