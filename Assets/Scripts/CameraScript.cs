using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public GameObject targetObject;
    public float GottaGoFast;
    //private float targetAngle = 0;
    //const float rotationAmount = 10f;
    //public float rDistance = 1.0f;
    //public float rSpeed = 1.0f;

    Vector3 offset0 = new Vector3(5.0f, 10.0f, -10.0f);
    Vector3 offset3 = new Vector3(-9.0f, 10.0f, -6.0f);
    Vector3 offset2 = new Vector3(-5.0f, 10.0f, 8.0f);
    Vector3 offset1 = new Vector3(9.0f, 10.0f, 4.0f);
    private int status = 0;


    void Start()
    {
        //offset = transform.position - targetObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        // Trigger functions if Rotate is requested
        if (Input.GetKeyDown(KeyCode.I))
        {
            Rotate(90);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            Rotate(-90);
        }

        
       

        // transform.LookAt(targetObject);
    }

    void LateUpdate()
    {

        int statusRemain = status % 4; 

        switch (statusRemain)
        {
            case 0:
                Vector3 desiredPosition0 = targetObject.transform.position + offset0;
                Vector3 position0 = Vector3.Lerp(transform.position, desiredPosition0, Time.deltaTime * GottaGoFast);
                transform.LookAt(targetObject.transform);
                transform.position = position0;
                break;
            case 1:
                Vector3 desiredPosition1 = targetObject.transform.position + offset1;
                Vector3 position1 = Vector3.Lerp(transform.position, desiredPosition1, Time.deltaTime * GottaGoFast);
                transform.LookAt(targetObject.transform);
                transform.position = position1;
                break;
            case 2:
                Vector3 desiredPosition2 = targetObject.transform.position + offset2;
                Vector3 position2 = Vector3.Lerp(transform.position, desiredPosition2, Time.deltaTime * GottaGoFast);
                transform.LookAt(targetObject.transform);
                transform.position = position2;
                break;
            case 3:
                Vector3 desiredPosition3 = targetObject.transform.position + offset3;
                Vector3 position3 = Vector3.Lerp(transform.position, desiredPosition3, Time.deltaTime * GottaGoFast);
                transform.LookAt(targetObject.transform);
                transform.position = position3;
                break;
            default:
                break;
        }
        
    }


        protected void Rotate(float angle)
    {

        if (angle > 0)
        {
            status++;

            /*
            while (angle != 0)
            {
                transform.RotateAround(targetObject.transform.position, Vector3.up, -rotationAmount);
                angle -= rotationAmount;
            }
            */
        }
        else if (angle < 0)
        {

            status--;
            if (status < 0)
                status = 3;

            /*
            while (angle != 0)
            {
                transform.RotateAround(targetObject.transform.position, Vector3.up, rotationAmount);
                angle += rotationAmount;
            }
            */
        }

    }

   
}
