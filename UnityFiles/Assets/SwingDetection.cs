using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class SwingDetection : MonoBehaviour
{
    public enum swingDir
    {
        Horizontal = 0,
        Downwards = 1,
        Upwards = 2,
        DiagonalDownwards = 3,
        DiagonalUpwards = 4,
        Thrust = 5,
        Null = 6,
    }
    private enum posLocation
    {
        Tip = 0,
        TrueEdge = 1,
        FalseEdge = 2,
        Crossguard = 3,
        Null = 4,

    }

    [FormerlySerializedAs("swordPositionData")] [FormerlySerializedAs("SwordPosition")] public SwordGuardData swordGuardData;
    public Camera playerCam;
    private posLocation currentLocation = posLocation.Null;
    public swingDir tipSwingDirection = swingDir.Null;
    public swingDir trueEdgeSwingDirection= swingDir.Null;
    public swingDir falseEdgeSwingDirection= swingDir.Null;
    public swingDir crossguardSwingDirection= swingDir.Null;
    public float minimumVerticalDisplacement;
    public float minimumHorizontalDisplacement;
    public float minimumForwardsDisplacement;
    

    public GameObject swordTip;
    public GameObject trueEdge;
    public GameObject falseEdge;
    public GameObject crossguard;
    public DataTracker tipData;
    public DataTracker trueEdgeData;
    public DataTracker falseEdgeData;
    public DataTracker crossguardData;
    public List<GameObject> trackedPoints = new List<GameObject>();
    

    // Start is called before the first frame update
    void Start()
    {
        tipData = swordTip.GetComponent<DataTracker>();
        trueEdgeData = trueEdge.GetComponent<DataTracker>();
        falseEdgeData = falseEdge.GetComponent<DataTracker>();
        crossguardData = crossguard.GetComponent<DataTracker>();
        trackedPoints = GameObject.FindGameObjectsWithTag("TrackedPoint").ToList();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool CheckPosDataFull(DataTracker dataTracker)
    {
        bool valid = true;
        foreach (var position in dataTracker.positionLog)
        {
            if (position == Vector3.zero)
            {
                valid = false;
            }

        }
        return valid;
    }

    void SetSwingDirection(DataTracker dataTracker)
    {
        swingDir swingDirection = CalculateSwingDirection(dataTracker);
        
        switch (currentLocation)
        {
            case posLocation.Tip:
                //SET DIRECTION OF TIP
                tipSwingDirection = swingDirection;
                break;
            case posLocation.TrueEdge:
                //SET DIRECTION OF TRUE EDGE
                trueEdgeSwingDirection = swingDirection;
                break;
            case posLocation.FalseEdge:
                //SET DIRECTION OF FALSE EDGE
                falseEdgeSwingDirection = swingDirection;
                break;
            case posLocation.Crossguard:
                //SET DIRECTION OF CROSSGUARD
                crossguardSwingDirection = swingDirection;
                break;
        }
        
    }

    swingDir CalculateSwingDirection(DataTracker dataTracker)
    {
        swingDir newSwingDir = swingDir.Null;
        bool verticalValid = false;
        bool upwards = false;
        bool horizontalValid = false;
        bool thrust = false;
        Vector3 startPos = dataTracker.positionLog[0];
        
        float xDisplacement = 0;
        float zDisplacement = 0;
        float verticalDisplacement = 0;
        
        int targetX = 0;
        int targetY = 0;
        int targetZ = 0;
        
        for (int i = 1; i < dataTracker.positionLog.Length; i++)
        {
            float newXDisplacement = Mathf.Abs(startPos.x - dataTracker.positionLog[i].x);
            float newYDisplacement = Mathf.Abs(startPos.y - dataTracker.positionLog[i].y);
            float newZDisplacement = Mathf.Abs(startPos.z - dataTracker.positionLog[i].z);
            
            if (newXDisplacement > xDisplacement && newXDisplacement > minimumHorizontalDisplacement)
            {
                xDisplacement = newXDisplacement;
                targetX = i;

            }
            if (newYDisplacement > verticalDisplacement && newYDisplacement > minimumVerticalDisplacement)
            {
                verticalDisplacement = newYDisplacement;
                verticalValid = true;
                targetY = i;
            } 
            if (newZDisplacement > zDisplacement && newZDisplacement > minimumHorizontalDisplacement)
            {
                zDisplacement = newZDisplacement;
                targetZ = i;
            }
        }
        
        //VERTICAL
        //CHECK IF GREATEST DISTANCE CHANGE IS OVER MINIMUM AND GET GREATEST CHANGE
        //CHECK (0,ychange,0) direction vs vector up, dot product (IF 1, UP else if -1, DOWN)
        Vector3 startY = new Vector3(0,startPos.y,0);
        Vector3 endY = new Vector3(0,dataTracker.positionLog[targetY].y);
        Vector3 direction = Vector3.Normalize(endY - startY);
        if (Vector3.Dot(direction, Vector3.up) > 0.5f)
        {
            upwards = true;
        }
        else if (Vector3.Dot(direction, Vector3.up) < -0.5f)
        {
            upwards = false;
        }

        
        //HORIZONTAL
        
        // USE VECTOR3.ANGLE FOR FORWARDS or Dot vs camera forwards > 0.7F
        //NEED GREATEST DISTANCE IN HORIZONTAL TO CALCULATE DIRECTION OF MOVEMENT OVER THE LAST X AMNT OF TIME
        
        
        
        Vector3 startHorizontal = new Vector3(startPos.x,0,startPos.z);
        Vector3 endHorizontal = new Vector3(dataTracker.positionLog[targetX].x,0,dataTracker.positionLog[targetZ].z);
        direction = Vector3.Normalize(endHorizontal - startHorizontal);
      

        float directionDot = Vector3.Dot(direction, playerCam.transform.forward);
        if (directionDot < 0.4 && directionDot > -0.4 && (targetX!= 0 || targetZ != 0))
        {
            horizontalValid = true;
        }
        else if (Vector3.Dot(direction, playerCam.transform.forward) > 0.9f && swordGuardData.currentDirection == StanceObject.generalDirection.Forwards)
        {
            thrust = true;
        }


        if (verticalValid && !horizontalValid)
        {
            newSwingDir = upwards ? swingDir.Upwards : swingDir.Downwards;
        }
        else if (thrust)
        {
            newSwingDir = swingDir.Thrust;
        }
        else if (horizontalValid)
        {
            if (verticalValid)
            {
                newSwingDir = upwards ? swingDir.DiagonalUpwards : swingDir.DiagonalDownwards;
            }
            else
            {
                newSwingDir =  swingDir.Horizontal;
            }
        }
       
        


        return newSwingDir;
    }
}
