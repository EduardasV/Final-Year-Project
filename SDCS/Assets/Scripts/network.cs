using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;



public class network : MonoBehaviour
{
    public GameObject car;

    public String host = "localhost";
    public Int32 port = 50000;

    public float targetSpeed;
    public float targetSteer;

    public string[] receivedTextSplit;

    internal Boolean socket_ready = false;
    internal String input_buffer = "";
    TcpClient tcp_socket;
    NetworkStream net_stream;

    StreamWriter socket_writer;
    StreamReader socket_reader;

    public float testingcar;
    CarEngine CEFunction;

    int i = 0;
    public string netMessage;

    void FixedUpdate()
    {
        netMessage = string.Format("{0}|{1}",
            car.GetComponent<CarEngine>().carBrain,
            car.GetComponent<CarEngine>().targetSteer);
        car.GetComponent<CarEngine>().CarSpeed(targetSpeed);
        car.GetComponent<CarEngine>().CarSteering(targetSteer);
        writeSocket(netMessage);
        string received_data = readSocket();
        if (received_data != "")
        {
            // Do something with the received data,
            // print it in the log for now
            //Debug.Log(received_data);
            receivedTextSplit = received_data.Split('|');
            car.GetComponent<CarEngine>().CarSpeed(float.Parse(receivedTextSplit[1]));
            car.GetComponent<CarEngine>().CarSteering(float.Parse(receivedTextSplit[0]));
        }
    }

    void Awake()
    {
        setupSocket();
    }
    void OnApplicationQuit()
    {
        closeSocket();
    }
    public void setupSocket()
    {
        try
        {
            tcp_socket = new TcpClient(host, port);

            net_stream = tcp_socket.GetStream();
            socket_writer = new StreamWriter(net_stream);
            socket_reader = new StreamReader(net_stream);

            socket_ready = true;
        }
        catch (Exception e)
        {
            // Something went wrong
            Debug.Log("Socket error: " + e);
        }
    }
    public void writeSocket(string line)
    {
        if (!socket_ready)
            return;

        line = line + "\r\n";
        socket_writer.Write(line);
        socket_writer.Flush();
    }
    public String readSocket()
    {
        if (!socket_ready)
            return "!socket_ready";

        if (net_stream.DataAvailable)
            return socket_reader.ReadLine();

        return "";
    }
    public void closeSocket()
    {
        if (!socket_ready)
            return;

        socket_writer.Close();
        socket_reader.Close();
        tcp_socket.Close();
        socket_ready = false;
    }
}