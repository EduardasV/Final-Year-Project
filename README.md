# Final-Year-Project  
## Purpose of the project 
The purpose of this project was to create an AI which would follow simple navigation instructions and drive around the town while maintaining the position in the middle of the lane. When the AI is given the direction that it needs to turn on the next junction it would analyse the road, when it sees an opportunity to turn it would then turn the car and position itself back to the center of the lane.
## Technology used

- Unity3D: C#
- Python3: OpenCV, Keras backend Tensorflow

## How it works

At the time of making this project, Unity didn't have a lot of machine learning tools. The python plugin wasn't enough because you couldn't import required modules; my other approach was to make this project purely in C# but unity OpenCV wasn't updated and had a lot of errors which would require a lot of time to fix. I decided to go back to using C# and Python, but the new approach I decided to try was to send information over through the network.

Sending information through the network, I was required to have a server and a client. In my case, I decided to have my Unity App to run as a server, and python to be the client. For the network I used tcp_sockets, this was the best solution I found which made python send information to unity, because unity app needed only 2 values from the python app which was car steering and car speed.

How did python know what information to send to Unity? The way python was able to send the correct data is because it used images saved by unity, analysed it and sent information over the network. The images are saved 30 times per second from the camera that is at the front of the car. This image is overwritten after every save; making it space efficient but the bad thing about this was python would miss a few frames if the image took a while to analyse. The way python would analyse the images is by loading it and sending it through the CNN architecture which was inspired by the nvidia CNN architecture.

The AI consists of 4 different neural networks (turn left, turn right, left lane and right lane), I decided to do it like this, because it wasn't possible to have conditions inside one NN (neural network). The way each NN was selected was with the navigation system, the navigation system would see it needs to turn then it would send the AI information on the type of NN it has to use. Each NN was train with 1000 sample images which were generated by manually driving around in the Unity App and the images were saved inside a folder with the reference to the image saved in a CSV file, the file had the image name, location, the speed and wheel angle of the vehicle on that frame.

## How to run it

To run this application you will need to have python 3 and Unity3D 2017.* installed.
Unfortunately, you will have to run the Unity App inside Unity as the .exe had performance issues when saving images. 
1. clone the repository
1. open the project in unity
1. run the unity project by pressing the play button  
1. make sure LB.h5, RB.h5, TLB.h5, TRB.h5 files are inside the python folder
1. run the network.py file in python folder inside IDLE for python 3 


---
[Example of the project in a YouTube video](https://www.youtube.com/watch?v=SCyNrJgGW8w)
