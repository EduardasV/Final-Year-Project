from keras.layers import Lambda, Conv1D, MaxPooling2D, Dropout, Dense, Flatten
from sklearn.model_selection import train_test_split
from utils import INPUT_SHAPE, batch_generator
from keras.callbacks import ModelCheckpoint, TensorBoard
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

np.random.seed(0)


def load_data(args):
    data_df = pd.read_csv(os.path.join(os.getcwd(), args.data_dir,'DataTRB.csv'), names=['camera', 'speed', 'steering'])
    
    X = data_df[['camera']].values
    y = data_df['steering'].values

    X_train, X_valid, y_train, y_valid = train_test_split(X, y, test_size=args.test_size, random_state=0)
    return X_train, X_valid, y_train, y_valid


def build_model(args):
    #CNN 9 Layer model    Exponential Linear Unit
    model = Sequential()
    model.add(Lambda(lambda x: x/127.5-1.0, input_shape=INPUT_SHAPE))
    model.add(Conv1D(24, (5), activation="elu", strides=(2)))
    model.add(Conv1D(36, (5), activation="elu", strides=(2)))
    model.add(Conv1D(48, (5), activation="elu", strides=(2)))
    model.add(Conv1D(64, (3), activation='elu'))
    model.add(Conv1D(64, (3), activation="elu"))
    model.add(Dropout(args.keep_prob))
    model.add(Flatten())
    model.add(Dense(100, activation='elu'))
    model.add(Dense(50, activation='elu'))
    model.add(Dense(10, activation='elu'))
    model.add(Dense(1))
    model.summary()

    return model


def train_model(model, args, X_train, X_valid, y_train, y_valid):
    #Saves the model after every epoch.
    checkpoint = ModelCheckpoint('model-{epoch:03d}.h5',
                                 monitor='val_loss',
                                 verbose=0,
                                 save_best_only=args.save_best_only,
                                 mode='auto')

    #calculates difference between predicted steering angle and actual
    model.compile(loss='mean_squared_error', optimizer=Adam(lr=args.learning_rate))

    #tensorboard setup
    tensorboard = TensorBoard(log_dir='./logs', write_images=True)

    #Fits the model on data generated batch-by-batch and trains the model simulatenously
    model.fit_generator(batch_generator(args.data_dir, X_train, y_train, args.batch_size, True),
                        args.samples_per_epoch,
                        args.epochs,
                        max_queue_size=1,
                        validation_data=batch_generator(args.data_dir, X_valid, y_valid, args.batch_size, False),
                        validation_steps=len(X_valid),
                        callbacks=[tensorboard, checkpoint],
                        verbose=1)

#for command line args
def s2b(s):
    #Converts a string to boolean value
    s = s.lower()
    return s == 'true' or s == 'yes' or s == 'y' or s == '1'


def main():
    parser = argparse.ArgumentParser(description='Behavioral Cloning Training Program')
    parser.add_argument('-d', help='brain type directory',  dest='data_dir',          type=str,   default='TRB')
    parser.add_argument('-t', help='test size',             dest='test_size',         type=float, default=0.5)
    parser.add_argument('-k', help='dropout prob',          dest='keep_prob',         type=float, default=0.2)
    parser.add_argument('-n', help='No.epochs',             dest='epochs',            type=int,   default=10)
    parser.add_argument('-s', help='samples per epoch',     dest='samples_per_epoch', type=int,   default=20000)
    parser.add_argument('-b', help='batch size',            dest='batch_size',        type=int,   default=40)
    parser.add_argument('-o', help='save best models only', dest='save_best_only',    type=s2b,   default='true')
    parser.add_argument('-l', help='learning rate',         dest='learning_rate',     type=float, default=0.0001)
    args = parser.parse_args()

    #print parameters
    print('-' * 30)
    print('Parameters')
    print('-' * 30)
    for key, value in vars(args).items():
        print('{:<20} := {}'.format(key, value))
    print('-' * 30)

    config = tf.ConfigProto()
    config.gpu_options.allow_growth = True
    session = tf.Session(config=config)
    session
    #load data
    data = load_data(args)
    #build model
    model = build_model(args)
    #train model on data, it saves as model.h5 
    train_model(model, args, *data)


if __name__ == '__main__':
    main()

