import cv2, os
import numpy as np
import matplotlib.image as mpimg


IMAGE_HEIGHT, IMAGE_WIDTH = 100, 150
INPUT_SHAPE = (IMAGE_HEIGHT, IMAGE_WIDTH)


def load_image(data_dir, image_file):
    imgage = cv2.imread(os.path.join(data_dir, image_file[0]), 0)
    return imgage

def preprocess(image):
    image = cv2.Canny(image,50,190)
    return image

def batch_generator(data_dir, image_paths, steering_angles, batch_size, is_training):
    images = np.empty([batch_size, IMAGE_HEIGHT, IMAGE_WIDTH])
    steers = np.empty(batch_size)
    while True:
        i = 0
        for index in np.random.permutation(image_paths.shape[0]):
            camera = image_paths[index]
            steering_angle = steering_angles[index]
            
            image = load_image(data_dir, camera)                       
            images[i] = preprocess(image)
            steers[i] = steering_angle
            i += 1
            if i == batch_size:
                break
        yield images, steers
