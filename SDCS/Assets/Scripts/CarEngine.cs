using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour {

    [Header("Prefab values")]
    public Transform path;
    public WheelCollider WFL;
    public WheelCollider WFR;
    public WheelCollider WRR;
    public WheelCollider WRL;
    public Vector3 centerOfMass;
    public Texture2D textureDefault;
    public Texture2D textureBraking;
    public Renderer renderCar;

    [Header("Vehicle values")]
    public float wheelAngle;
    public float maxMotorTorque;
    public float maxBrakeToruqe = 150f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public float initSpeed;
    public float targetSpeed;
    public bool avoiding = false;
    public string carBrain;
    public float targetSteer;
    public int i;

    [Header("Sensors")]
    public float sensorLength = 12f;
    public Vector3 frontSensorPos = new Vector3(0, 0.5f, 1.8f);
    public float FSensorPos = 0.7f;
    public float FASensorPos = 30f;

    private bool isBraking = false;
    private float maxSteerAngle = 35f;

    private List<Transform> nodes;
    private int currentNode = 0;
    private bool manualDrive = false;
    
	private void Start ()
    {
        carBrain = "LB";

        WFL.motorTorque = 0;
        WFR.motorTorque = 0;
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != path.transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }
    }
    //Manual Drive
    private void Brake(float val)
    {
        WRL.brakeTorque = val;
        WRR.brakeTorque = val;
    }
    private void Drive(float val)
    {
        WFL.motorTorque += val;
        WFR.motorTorque += val;
        if (WFL.motorTorque > 100)
        {
            WFL.motorTorque = 100;
            WFR.motorTorque = 100;
        }
        else if (WFL.motorTorque < 0)
        {
            WFL.motorTorque = 0;
            WFR.motorTorque = 0;
        }
    }
    private void Steering(float val)
    {
        WFL.steerAngle += val;
        WFR.steerAngle += val;
        if (WFL.steerAngle < -30)
        {
            WFL.steerAngle = -30;
            WFR.steerAngle = -30;
        }
        else if (WFL.steerAngle > 30)
        {
            WFL.steerAngle = 30;
            WFR.steerAngle = 30;
        }
        if(val == 0 && WFL.steerAngle < 0)
        {
            WFL.steerAngle++;
            WFR.steerAngle++;
        }
        else if (val == 0 && WFL.steerAngle > 0)
        {
            WFL.steerAngle--;
            WFR.steerAngle--;
        }
    }


    //Auto Drive
    public void CarSteering(float val)
    {
        if (!manualDrive)
        {
            WFL.steerAngle = val;
            WFR.steerAngle = val;
            if (WFL.steerAngle < -30)
            {
                WFL.steerAngle = -30;
                WFR.steerAngle = -30;
            }
            else if (WFL.steerAngle > 30)
            {
                WFL.steerAngle = 30;
                WFR.steerAngle = 30;
            }
            if (val == 0 && WFL.steerAngle < 0)
            {
                WFL.steerAngle++;
                WFR.steerAngle++;
            }
            else if (val == 0 && WFL.steerAngle > 0)
            {
                WFL.steerAngle--;
                WFR.steerAngle--;
            }
        }
    }
    public void CarSpeed(float tSpeed)
    {
        if (!manualDrive)
        {
            if (tSpeed > currentSpeed)
            {
                WFL.motorTorque = 150;
                WFR.motorTorque = 150;
                WRL.brakeTorque = 0;
                WRR.brakeTorque = 0;
            }
            else if (tSpeed < currentSpeed)
            {
                WFL.motorTorque = 0;
                WFR.motorTorque = 0;
                WRL.brakeTorque = 200;
                WRR.brakeTorque = 200;
            }
        }
    }
    public void BrainType()
    {

        if (!manualDrive)
        {
            Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
            float val = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
            if (val < -20 && !avoiding)
                carBrain = "TLB";
            else if (val > 20 && !avoiding)
                carBrain = "TRB";
            if (carBrain == "TLB" && val > -5)
                carBrain = "LB";
            else if (carBrain == "TRB" && val < 5)
                carBrain = "RB";
        }
    }

    private void FixedUpdate()
    {
        try
        { 
           // BrainType();
        }
        catch { }

        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        targetSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;


        if (Input.GetKey("z"))
            carBrain = "TLB";
        else if (Input.GetKey("x"))
            carBrain = "LB";
        else if (Input.GetKey("c"))
            carBrain = "RB";
        else if (Input.GetKey("v"))
            carBrain = "TRB";

        manualDrive |= Input.GetKey("m");
        if (manualDrive)
        {
            if (Input.GetKey("up") && !isBraking)
            {
                isBraking = false;
                Brake(0);
            }
            else if (Input.GetKey("down"))
            {
                isBraking = true;
            }
            else
            {
                isBraking = false;
                Brake(maxMotorTorque + 20);
            }

            if (Input.GetKey("left"))
            {
                Steering(-1);
                WFL.motorTorque = 0;
                WFR.motorTorque = 0;
            }
            else if (Input.GetKey("right"))
            {
                Steering(1);
                WFL.motorTorque = 0;
                WFR.motorTorque = 0;
            }
            else
            {
                Steering(0);
            }
            if (Input.GetKey("space"))
                Brake(0);
            if (Input.GetKey("n")) manualDrive = false;
        }
        else
        { 
            for (i = 0; i < 100; i++)
            CarSpeed(i++);
        }

        maxMotorTorque = WFL.motorTorque;
        maxBrakeToruqe = WRL.brakeTorque;
        currentSpeed = (((((2 * Mathf.PI * WFL.radius) * WFL.rpm) * 60)*0.62f)/1000)/3;
        wheelAngle = WFL.steerAngle;
        initSpeed = WFL.rpm;



        Sensors();
        
        CheckWaypointDistance();
        Braking();
	}

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPos.z;
        sensorStartPos += transform.up * frontSensorPos.y;
        float avoidMultiplier = 0;
        avoiding = false;

        //front right
        sensorStartPos -= transform.right * FSensorPos / 20;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength*2))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                manualDrive = true;
                WRL.brakeTorque = 7000;
                WRR.brakeTorque = 7000;
                WFL.motorTorque = 0;
                WFR.motorTorque = 0;
                avoiding = true;
                if (carBrain == "RB")
                    carBrain = "LB";
            }
        }

        //front angled right
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(FASensorPos, transform.up) * transform.forward , out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                manualDrive = true;
                WRL.brakeTorque = 7000;
                WRR.brakeTorque = 7000;
                WFL.motorTorque = 0;
                WFR.motorTorque = 0;
                avoiding = true;
                carBrain = "LB";
            }
        }

        //front angled left
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-FASensorPos, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                manualDrive = true;
                WRL.brakeTorque = 7000;
                WRR.brakeTorque = 7000;
                WFL.motorTorque = 0;
                WFR.motorTorque = 0;
                avoiding = true;
                carBrain = "RB";
            }
        }

        else if (avoiding)
        {
            WFL.steerAngle = maxSteerAngle * avoidMultiplier;
            WFR.steerAngle = maxSteerAngle * avoidMultiplier;
        }

    }

    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 3f)
        {
            if(currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }
    private void Braking()
    {
        if (isBraking)
        {
            renderCar.material.mainTexture = textureBraking;
            WRL.brakeTorque = 700;
            WRR.brakeTorque = 700;
            WFL.motorTorque = 0;
            WFR.motorTorque = 0;
        }
        else if(manualDrive)
        {
            renderCar.material.mainTexture = textureDefault;
            WFL.motorTorque = 150;
            WFR.motorTorque = 150;
        }
    }
}


