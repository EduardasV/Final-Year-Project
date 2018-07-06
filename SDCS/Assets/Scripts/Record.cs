using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;


public class Record : MonoBehaviour {
    public GameObject car;

    public int frameWidth = 150;
    public int frameHeight = 100;

    private string brain;

    private bool RB = false;
    private bool LB = false;
    private bool TRB = false;
    private bool TLB = false;
    private int fileCounter = 0;

    public float interval = 0.1f;
    float elapsed = 0f;

    public float CEspeed;
    public float CEbrake;
    public float CEmotor;
    public float CEwheel;
    public string labelType;
    
    void Update()
    {
        SaveOneFrame();

        CEspeed = car.GetComponent<CarEngine>().currentSpeed;
        CEbrake = car.GetComponent<CarEngine>().maxBrakeToruqe;
        CEmotor = car.GetComponent<CarEngine>().maxMotorTorque;
        CEwheel = car.GetComponent<CarEngine>().wheelAngle;

        TLB |= Input.GetKeyDown("q");
        LB |= Input.GetKeyDown("w");
        RB |= Input.GetKeyDown("e");
        TRB |= Input.GetKeyDown("r");
        if (Input.GetKeyDown("s"))
        {
            RB = LB = TRB = TLB = false;
            fileCounter = 0;
        }
        elapsed += Time.deltaTime;
        if (elapsed >= interval)
        {
            elapsed = 0;
            TakeShot();
        }
    }
    void TakeShot()
    {
        if (TLB)
        {
            DirectoryInfo p = new DirectoryInfo("./Assets/Python/TLB/");
            FileInfo[] files = p.GetFiles();
            saveFrame("TLB", fileCounter);
            saveFile("TLB", CEspeed, CEwheel, fileCounter);
            fileCounter = files.Length + 1;
        }
        else if (LB)
        {
            DirectoryInfo p = new DirectoryInfo("./Assets/Python/LB/");
            FileInfo[] files = p.GetFiles();
            saveFrame("LB", fileCounter);
            saveFile("LB", CEspeed, CEwheel, fileCounter);
            fileCounter = files.Length + 1;
        }
        else if (RB)
        {
            DirectoryInfo p = new DirectoryInfo("./Assets/Python/RB/");
            FileInfo[] files = p.GetFiles();
            saveFrame("RB", fileCounter);
            saveFile("RB", CEspeed, CEwheel, fileCounter);
            fileCounter = files.Length + 1;
        }
        else if (TRB)
        {
            DirectoryInfo p = new DirectoryInfo("./Assets/Python/TRB/");
            FileInfo[] files = p.GetFiles();
            saveFrame("TRB", fileCounter);
            saveFile("TRB", CEspeed, CEwheel, fileCounter);
            fileCounter = files.Length + 1;
        }
    }
    void SaveOneFrame()
    {
        try
        { 
            RenderTexture rt = new RenderTexture(frameWidth, frameHeight, 24);
            GetComponentInChildren<Camera>().targetTexture = rt;
            Texture2D frame = new Texture2D(frameWidth, frameHeight, TextureFormat.RGB24, false);
            GetComponentInChildren<Camera>().Render();
            RenderTexture.active = rt;
            frame.ReadPixels(new Rect(0, 0, frameWidth, frameHeight), 0, 0);
            GetComponentInChildren<Camera>().targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            byte[] bytes = frame.EncodeToPNG();
            File.WriteAllBytes("./MainFrame.png", bytes);
        }
        catch {}
    }
    void saveFile(string brain, float Cspeed, float Cwheel, int counter)
    {
        string csvLocation = string.Format("./Assets/Python/{0}/Data{0}.csv", brain);
        string row = string.Format("{0},{1},{2}\n", frameName(brain, counter),
            Cspeed, Cwheel);
        File.AppendAllText(csvLocation, row);
    }
    void saveFrame(string brainType, int counter)
    {
        RenderTexture rt = new RenderTexture(frameWidth, frameHeight, 24);
        GetComponentInChildren<Camera>().targetTexture = rt;
        Texture2D frame = new Texture2D(frameWidth, frameHeight, TextureFormat.RGB24, false);
        GetComponentInChildren<Camera>().Render();
        RenderTexture.active = rt;
        frame.ReadPixels(new Rect(0, 0, frameWidth, frameHeight), 0, 0);
        GetComponentInChildren<Camera>().targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        byte[] bytes = frame.EncodeToPNG();
        string filename = "./Assets/Python/" + brainType + "/" + 
            "/" + frameName(brainType, counter);
        File.WriteAllBytes(filename, bytes);
    }
    static string frameName(string brain, int counter)
    {
        string framePath = string.Format("{0}{2}_{1}.png",
                             brain,
                             DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                             counter);
        return framePath;
    }
    static int CountLinesInFile(string path)
    {
        int count = 0;
        using (StreamReader r = new StreamReader(path))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                count++;
            }
        }
        return count;
    }
}
