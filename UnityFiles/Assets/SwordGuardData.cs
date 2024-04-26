using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using static StanceObject;

public class SwordGuardData : MonoBehaviour
{
    public GameObject sword;
    public GameObject point;
    public GameObject trueEdge;
    public GameObject falseEdge;
    public GameObject crossguard;
    public Vector3[] pointPositionLog;
    public Vector3[] edgePositionLog;
    [FormerlySerializedAs("guardPositionLog")] public Vector3[] crossguardPositionLog;
    public GameObject head;
    private float edgesDistance;
    public bool isGuarding;
    private float guardTimer = 2f;
    public float guardCount = 0f;
    private float guardTimeout = 2f;
    public float guardTimeoutCount = 0f;
    public bool useWeightingSystem;
    float alberWeight = 0;
    float vomtagWeight = 0;
    float oxWeight = 0;
    float pflugWeight = 0;
    public float standardAngleVariance = 65;
    public float forwardsAngleVariance = 45;
    public float belowHeadUpwardsAngleVariance = 80;
    public List<StanceObject> stances;
    public StanceObject activeStanceObject;
    public StanceObject previousStanceObject;
    public StanceObject currentStanceObject;
    
    //POINT DIRECTION
    public enum pointDirection
    {
        TiltUp = 0,
        TiltDown = 1,
        NoTilt = 2,
        Null = 3,
    }
    // PLANE OF THE SWORD
    public enum plane
    {
        Horizontal = 0,
        Vertical = 1,
        Null = 2,
    }
    //RELATIVITY TO HEAD
    public enum relativityToHead
    {
        AboveHead = 0,
        BelowHead = 1,
        Null = 2,
    }
    // GENERAL DIRECTION
    public enum generalDirection
    {
        Forwards = 0,
        Upwards = 1,
        Downwards = 2,
        Null = 3,
    }
    public enum guardPose
    {
        Vomtag = 0,
        Alber = 1,
        Ox = 2,
        Pflug = 3,
        Null = 4,
    }
    
    
    public StanceObject.generalDirection currentDirection = StanceObject.generalDirection.Null;
    public pointDirection currentPointDirection = pointDirection.Null;
    public StanceObject.relativityToHead currentRelativityToHead = StanceObject.relativityToHead.Null;
    public StanceObject.plane currentPlane = StanceObject.plane.Null;
    public StanceObject.guardPose currentguard = StanceObject.guardPose.Null;
    public StanceObject.guardPose trackedGuard = StanceObject.guardPose.Null;
    public StanceObject.guardPose previousGuard = StanceObject.guardPose.Null;
    private float previousguardChange = 0.75f;
    public float previousguardChangeCount = 0f;
    private bool guardsDiff = false;
    private DataTracker crossguardData;
    private DataTracker pointData;
    private DataTracker trueEdgeData;
    
    
    // Start is called before the first frame update
    void Start()
    {
        edgesDistance = Mathf.Abs(falseEdge.transform.position.x) + Mathf.Abs(trueEdge.transform.position.x);
        Debug.Log("Edge distance is: " + edgesDistance);
        pointPositionLog = new Vector3[5];
    edgePositionLog = new Vector3[5];
    crossguardPositionLog = new Vector3[5];
    crossguardData = crossguard.GetComponent<DataTracker>();
    pointData = point.GetComponent<DataTracker>();
    trueEdgeData = trueEdge.GetComponent<DataTracker>();

    }

    // Update is called once per frame
    void Update()
    {
        crossguardPositionLog = crossguardData.positionLog;
        pointPositionLog = pointData.positionLog;
        edgePositionLog = trueEdgeData.positionLog;

        
        CalcRelativePosition();
        CalcGeneralDirection();
        CalcPlane();
        CalcPointDirection();
        CalcGuard();


        if (currentguard != previousGuard && currentguard != StanceObject.guardPose.Null)
        {
            previousGuard = currentguard;
            previousStanceObject = currentStanceObject;
            guardsDiff = true;
            previousguardChangeCount = 0;
        }
        else if (guardsDiff)
        {
            previousguardChangeCount += Time.deltaTime;
        }


        if (previousguardChangeCount >= previousguardChange)
        {
            guardsDiff = false;
            trackedGuard = previousGuard;
            activeStanceObject = previousStanceObject;
            previousguardChangeCount = 0;
        }




        bool guardDropped = currentguard == StanceObject.guardPose.Null;

        if (guardTimeoutCount >= guardTimeout)
        {
            guardCount = 0;
            trackedGuard = StanceObject.guardPose.Null;
            previousGuard = StanceObject.guardPose.Null;
            isGuarding = false;
            guardTimeoutCount = 0;
        }
        else if (guardCount >= guardTimer)
        {
            isGuarding = true;
        }


        if (guardDropped)
        {
            guardTimeoutCount += Time.deltaTime;
        }
        else if (currentguard != StanceObject.guardPose.Null)
        {
            guardCount += Time.deltaTime;
            guardTimeoutCount = 0;
        }



    }

    public void UpdateGenDirWeight()
    {
        if (currentDirection == StanceObject.generalDirection.Upwards)
        {
            pflugWeight += 0.5f;
            vomtagWeight += 0.5f;
        }
        else if (currentDirection == StanceObject.generalDirection.Downwards)
        {
            alberWeight += 1;
        }
        else if (currentDirection == StanceObject.generalDirection.Forwards)
        {
            alberWeight += 0.15f;
            pflugWeight += 0.15f;
            oxWeight += 0.7f;
        }

    }
    public void UpdateRelativeWeight()
    {
        //Increase the weighted values for each guard if trait matches the guard
        if (currentRelativityToHead == StanceObject.relativityToHead.AboveHead)
        {
            oxWeight += 0.5f;
            vomtagWeight += 0.5f;
        }
        else if (currentRelativityToHead == StanceObject.relativityToHead.BelowHead)
        {
            alberWeight += 0.5f;
            pflugWeight += 0.5f;
        }


    }
    public void UpdatePlaneWeight()
    {
        if (currentPlane == StanceObject.plane.Horizontal)
        {
            oxWeight += 1;
        }
        else if (currentPlane == StanceObject.plane.Vertical)
        {
            alberWeight += 0.33f;
            pflugWeight += 0.33f;
            vomtagWeight += 0.33f;
        }
        
    }

    private void CalcGuard()
    {
        bool stanceFound = false;
        //Loop through all stances applied to the weapon.
        //Check the direction, plane and position against the values in each stance's template.
        foreach (var stance in stances.Where(stance =>
            currentDirection == stance.requiredGeneralDirection && currentPlane == stance.requiredPlane &&
            currentRelativityToHead == stance.requiredRelativityToHead))
        {
            //Valid stance was found, set as the current guard and store the Stance Object to be referenced on damage calculation.
            currentguard = stance.stanceID;
            stanceFound = true;
            currentStanceObject = stance;
        }

        // If no stances were found, clear the current guard.
        if (stanceFound) return;
        currentguard = StanceObject.guardPose.Null;
        currentStanceObject = null;
    }

    private void CalcGuardOld()
    {
        /*if (!useWeightingSystem)
        {
            //Check for exact criteria for Ox Stance
            if (currentPlane == plane.Horizontal &&
                currentDirection == generalDirection.Forwards &&
                currentRelativityToHead == relativityToHead.AboveHead)
            {
                currentguard = StanceObject.guardPose.Ox;
            }
            //Check for exact criteria for Vom Tag Stance
            else if (currentPlane == StanceObject.plane.Vertical &&
                     currentDirection == StanceObject.generalDirection.Upwards &&
                     currentRelativityToHead == StanceObject.relativityToHead.AboveHead)
            {
                currentguard = StanceObject.guardPose.Vomtag;
            }
            //Check for exact criteria for Alber Stance
            else if (currentPlane == StanceObject.plane.Vertical &&
                     currentDirection == StanceObject.generalDirection.Downwards &&
                     currentRelativityToHead == StanceObject.relativityToHead.BelowHead)
            {
                currentguard = StanceObject.guardPose.Alber;
            }
            //Check for exact criteria for Pflug Stance
            else if (currentPlane == StanceObject.plane.Vertical &&
                     currentDirection == StanceObject.generalDirection.Upwards &&
                     currentRelativityToHead == StanceObject.relativityToHead.BelowHead)
            {
                currentguard = StanceObject.guardPose.Pflug;
            }
            //If no valid stances were found, set stance to Null
            else
            {
                currentguard = StanceObject.guardPose.Null;
            }
        }
        else if (useWeightingSystem)
        {
            alberWeight = 0;
            pflugWeight = 0;
            oxWeight = 0;
            vomtagWeight = 0;
            
            
            UpdatePlaneWeight();
            UpdateRelativeWeight();
            UpdateGenDirWeight();


            float currentWeight = 0;
            if (pflugWeight >= 1.33f)
            {
                currentWeight = pflugWeight;
                currentguard = StanceObject.guardPose.Pflug;
            }

            if (alberWeight >= 1.33f && alberWeight > currentWeight)
            {
                currentWeight = alberWeight;
                currentguard = StanceObject.guardPose.Alber;
            }

            if (vomtagWeight >= 1.33f && vomtagWeight > currentWeight)
            {
                currentWeight = vomtagWeight;
                currentguard = StanceObject.guardPose.Vomtag;
            }

            if (oxWeight >= 1.75f && oxWeight > currentWeight)
            {
                currentWeight = oxWeight;
                currentguard = StanceObject.guardPose.Ox;
            }

            if (currentWeight < 1.33f)
            {
                currentguard = StanceObject.guardPose.Null;
            }
            
        }*/


        // DETERMINE A GUARD BASE ON THE INPUT ENUMS

    }

    
    //UPDATES THE PLANE OF THE SWORD
    private void CalcPlane()
    {
        //Calculate vector between sword edges
        Vector3 edgesVector = new Vector3();
        edgesVector = Vector3.Normalize(falseEdge.transform.position - trueEdge.transform.position);

        //If the sword is pointing up or down, the edge vector is compared against the head forwards transform.
        if (currentDirection == StanceObject.generalDirection.Upwards )
        {
            if (Vector3.Dot(edgesVector, head.transform.forward) > -0.5 && Vector3.Dot(edgesVector, head.transform.forward) < 0.5)
            {
                currentPlane = StanceObject.plane.Horizontal; 
            }
            else
            {
                currentPlane = StanceObject.plane.Vertical;
            }
        }
        //For any other weapon position, the world up vector is compared.
        else if (Vector3.Dot(edgesVector, Vector3.up) > -0.5  && Vector3.Dot(edgesVector, Vector3.up) < 0.5)
        {
            currentPlane = StanceObject.plane.Horizontal;
        }
        else
        {
            currentPlane = StanceObject.plane.Vertical;
        }
    }
    
    
    
    //Determines the weapon position relative to the head of the user.
    private void CalcRelativePosition()
    {
        //Determine if the crossguard is currently above or below the headset.
        if (crossguard.transform.position.y >= head.transform.position.y)
        {
            currentRelativityToHead = StanceObject.relativityToHead.AboveHead;
        }
        else if (crossguard.transform.position.y < head.transform.position.y)
        {
            currentRelativityToHead = StanceObject.relativityToHead.BelowHead;
        }
    }


    //UPDATES THE DIRECTION OF THE SWORDS POINT RELATIVE TO THE HANDS
    private void CalcPointDirection()
    {
        // CALCULATE THE GENERAL DIRECTION OF THE SWORD POINT AND SET THE ENUM
        //TILT UP
        //TILT DOWN
        //NONE
        float tiltDistance = 20;

        if (currentDirection == StanceObject.generalDirection.Upwards || currentDirection == StanceObject.generalDirection.Downwards)
        {
        }
    }
    
    
    //Calculates the general direction of the weapon. E.g Upwards or Forwards.
    private void CalcGeneralDirection()
    {
        //Sword Pointing Downwards
        if (Vector3.Angle(crossguard.transform.up, -Vector3.up) < standardAngleVariance)
        {
            currentDirection = StanceObject.generalDirection.Downwards;
        }
        //Sword Below Head Pointing Upwards
        else if (Vector3.Angle(crossguard.transform.up, Vector3.up) < belowHeadUpwardsAngleVariance &&
                 currentRelativityToHead == StanceObject.relativityToHead.BelowHead)
        {
            currentDirection = StanceObject.generalDirection.Upwards;
        }
        //Sword Above Head Pointing Upwards
        else if (Vector3.Angle(crossguard.transform.up, Vector3.up) < standardAngleVariance &&
                 currentRelativityToHead == StanceObject.relativityToHead.AboveHead)
        {
            currentDirection = StanceObject.generalDirection.Upwards;
        }
        //Sword Pointing Forwards
        else if (Vector3.Angle(crossguard.transform.up, head.transform.forward) < forwardsAngleVariance)
        {
            currentDirection = StanceObject.generalDirection.Forwards;
        }
        //No Valid Direction Determined
        else
        {
            currentDirection = StanceObject.generalDirection.Null;
        }
    }
}
