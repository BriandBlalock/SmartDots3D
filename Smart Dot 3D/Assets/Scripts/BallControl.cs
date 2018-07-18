using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharpNeat.Phenomes;


public class BallControl : UnitController {
    public float Speed = 5f;
    
    bool MovingForward = true;
    bool IsRunning = true ;
    public float SensorRange = 200;
    public GameObject goal;
    IBlackBox box;
    int steps = 0;
    float averageSpeed =  0;
    // Use this for initialization
    void Start()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //grab the input axes
        //var steer = Input.GetAxis("Horizontal");
        //var gas = Input.GetAxis("Vertical");

        ////if they're hittin' the gas...
        //if (gas != 0)
        //{
        //    //take the throttle level (with keyboard, generally +1 if up, -1 if down)
        //    //  and multiply by speed and the timestep to get the distance moved this frame
        //    var moveDist = gas * speed * Time.deltaTime;

        //    //now the turn amount, similar drill, just turnSpeed instead of speed
        //    //   we multiply in gas as well, which properly reverses the steering when going 
        //    //   backwards, and scales the turn amount with the speed
        //    var turnAngle = steer * turnSpeed * Time.deltaTime * gas;

        //    //now apply 'em, starting with the turn           
        //    transform.Rotate(0, turnAngle, 0);

        //    //and now move forward by moveVect
        //    transform.Translate(Vector3.forward * moveDist);
        //}

       

        if (IsRunning)
        {
            float frontSensor = 0;
            float leftSensor = 0;
            float rightSensor = 0;
            float upSensor = 0;
            float downSensor = 0;
            float leftUpSensor = 0;
            float rightUpSensor = 0;
            float leftDownSensor = 0;
            float rightDownSensor = 0;
           
            // Front sensor
            RaycastHit hit;
           
           
                
            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(0, 0, 1).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("obstacle"))
                {
                    frontSensor = 1 - hit.distance / SensorRange;

                }
                Debug.DrawLine(transform.position, hit.point);
            }
           
            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(0.5f, 0.5f, 1).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("obstacle"))
                {
                    rightUpSensor = 1 - hit.distance / SensorRange;
                }
                Debug.DrawLine(transform.position, hit.point);
            }
           
            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(1, 0, 0).normalized) ,out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("obstacle"))
                {
                    rightSensor = 1 - hit.distance / SensorRange;
                }
                Debug.DrawLine(transform.position, hit.point);
            }
            
            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(-0.5f, 0.5f, 1).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("obstacle"))
                {
                    leftUpSensor = 1 - hit.distance / SensorRange;
                }
                Debug.DrawLine(transform.position, hit.point);
            }
           
            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(-1, 0, 0).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("obstacle"))
                {
                    leftSensor = 1 - hit.distance / SensorRange;
                }
                Debug.DrawLine(transform.position, hit.point);
            }

          
            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(0, 1, 0).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("obstacle"))
                {
                    upSensor = 1 - hit.distance / SensorRange;
                }
                Debug.DrawLine(transform.position, hit.point);
            }
           
            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(0, -1, 0).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("obstacle"))
                {
                    downSensor = 1 - hit.distance / SensorRange;
                }
                Debug.DrawLine(transform.position, hit.point);
            }
         
            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(-0.5f, -0.5f, 1).normalized),out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("obstacle"))
                {
                    leftDownSensor = 1 - hit.distance / SensorRange;
                }
                Debug.DrawLine(transform.position, hit.point);
            }
            
            if (Physics.Raycast(transform.position + transform.forward * 1.1f, transform.TransformDirection(new Vector3(0.5f, -0.5f, 1).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("obstacle"))
                {
                    leftUpSensor = 1 - hit.distance / SensorRange;
                }
                Debug.DrawLine(transform.position, hit.point);
            }

            ISignalArray inputArr = box.InputSignalArray;
            inputArr[0] = frontSensor;
            inputArr[1] = leftUpSensor;
            inputArr[2] = leftSensor;
            inputArr[3] = rightUpSensor;
            inputArr[4] = rightSensor;
            inputArr[5] = upSensor;
            inputArr[6] = downSensor;
            inputArr[7] = leftDownSensor;
            inputArr[8] = rightDownSensor;

            box.Activate();

            ISignalArray outputArr = box.OutputSignalArray;

            Vector3 moveDir = new Vector3((float)outputArr[0], (float)outputArr[1], (float)outputArr[2]).normalized;
            ++steps;
            averageSpeed = (averageSpeed + (float)outputArr[3]) / steps;

            transform.Translate(moveDir * (Speed * (float)outputArr[3]));
            
        }
    }

    public override void Stop()
    {
        this.IsRunning = false;
    }

    public override void Activate(IBlackBox box)
    {
        this.box = box;
        this.IsRunning = true;
    }



    public override float GetFitness()
    {
       
        float fit =  1/(goal.transform.position.z - transform.position.z) * 100  ;
       
        if (fit > 0)
        {
            return fit;
        }
        return 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.tag.Equals("obstacle"))
        {
            IsRunning = false;
        }
    }

}
