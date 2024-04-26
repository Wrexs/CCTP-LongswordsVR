using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class StrikeDetection : MonoBehaviour
{
    public GameObject swordData;
    public GameObject sword;
    public int currentGuard = 0;
    public Material VomtagMaterial;
    public Material AlberMat;
    public Material OxMaterial;
    public Material PflugMat;
    private Renderer renderer;
    public SwordGuardData swordPos;
    public float hitpoints = 100f;
    public GameObject floatingDamage;
    public bool requireGuard = false;
    private Vector3 collisionPoint;
    public GameObject playerCam;
    private DataTracker.pointNames closestPoint;
    public bool trainingTarget = false;
    private float striketimer = 0;
    private float strikeMax = 0.3f;

    public StanceObject.guardPose targetGuard;

    // Start is called before the first frame update
    void Start()
    {
        if (!trainingTarget)
        {
            currentGuard = Random.Range(0, 4);
        }

        SetGuard(currentGuard);
       
        renderer = GetComponent<Renderer>();
        swordPos = swordData.GetComponent<SwordGuardData>();
        playerCam = GameObject.FindWithTag("MainCamera");
        sword = GameObject.FindWithTag("Sword");


    }

    // Update is called once per frame
    void Update()
    {
        if (sword.GetComponent<SwordGuardData>().trackedGuard == targetGuard || !requireGuard)
        {
            gameObject.layer = 7;
        }
        else if (requireGuard)
        {
            gameObject.layer = 3;
        }

        if (striketimer > 0)
        {
            striketimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword") && striketimer <= 0)
        {
            striketimer = strikeMax;
            bool newGuardSet = false;
            if (!trainingTarget)
            {
                while (!newGuardSet)
                {
                    int newGuard = Random.Range(0, 4);
                    if (newGuard != currentGuard)
                    {
                        currentGuard = newGuard;
                        SetGuard(currentGuard);
                        newGuardSet = true;
                    }
                }
            }


            // Determine the closest point in worldspace between the weapon and the collision target
            collisionPoint = other.ClosestPoint(transform.position);
            var pointsList = other.GetComponent<SwingDetection>().trackedPoints;
            GameObject targetPoint = null;
            float lowestDist = Mathf.Infinity;
            
            // Find the closest tracked point on the sword against the collision point
            for (int i = 0; i < pointsList.Count; i++)
            {
                float dist = Vector3.Distance(pointsList[i].transform.position, collisionPoint);

                if (dist < lowestDist)
                {
                    lowestDist = dist;
                    targetPoint = pointsList[i];
                }
            }

            
            //Calculate the direction of the sword using the tracked position data from the closest point to the collision
            closestPoint = targetPoint.GetComponent<DataTracker>().pointName;
            SwingDetection.swingDir strikeType = SwingDetection.swingDir.Null;
            switch (closestPoint)
            {
                case DataTracker.pointNames.Point:
                    strikeType = CalcSwingDirection(sword.GetComponent<SwingDetection>().tipData);
                    break;
                case DataTracker.pointNames.TrueEdge:
                    strikeType = CalcSwingDirection(sword.GetComponent<SwingDetection>().trueEdgeData);
                    break;
                case DataTracker.pointNames.FalseEdge:
                    strikeType = CalcSwingDirection(sword.GetComponent<SwingDetection>().falseEdgeData);
                    break;
                case DataTracker.pointNames.Crossguard:
                    strikeType = CalcSwingDirection(sword.GetComponent<SwingDetection>().crossguardData);
                    break;
                case DataTracker.pointNames.Null:
                    break;
            }
            
            bool appropriateStrike = false;
            bool appropriateContactPoint = false;
            //Check strike direction against the activeStance's favoured strikes for damage calculations
            if (strikeType != SwingDetection.swingDir.Null)
            {
                switch (strikeType)
                {
                    //Check the strike direction against the active stance's favoured strike directions
                    case SwingDetection.swingDir.Downwards:
                        if (swordPos.activeStanceObject.favourDownwardsStrike)
                        {
                            //If the strike is a valid direction, was an appropriate part of the weapon used
                            appropriateStrike = true;
                            if (closestPoint == DataTracker.pointNames.TrueEdge ||
                                closestPoint == DataTracker.pointNames.FalseEdge)
                            {
                                appropriateContactPoint = true;
                            }
                        }

                        break;

                    case SwingDetection.swingDir.Horizontal:
                        if (swordPos.activeStanceObject.favourHorizontalStrike)
                        {
                            appropriateStrike = true;
                            if (closestPoint == DataTracker.pointNames.TrueEdge ||
                                closestPoint == DataTracker.pointNames.FalseEdge)
                            {
                                appropriateContactPoint = true;
                            }
                        }

                        break;

                    case SwingDetection.swingDir.Thrust:
                        if (swordPos.activeStanceObject.favourThrust)
                        {
                            appropriateStrike = true;
                            if (closestPoint == DataTracker.pointNames.Point)
                            {
                                appropriateContactPoint = true;
                            }
                        }

                        break;

                    case SwingDetection.swingDir.Upwards:
                        if (swordPos.activeStanceObject.favourUpwardsStrike)
                        {
                            appropriateStrike = true;
                            if (closestPoint == DataTracker.pointNames.TrueEdge ||
                                closestPoint == DataTracker.pointNames.FalseEdge)
                            {
                                appropriateContactPoint = true;
                            }
                        }

                        break;

                    case SwingDetection.swingDir.DiagonalDownwards:
                        if (swordPos.activeStanceObject.favourDownwardsStrike ||
                            swordPos.activeStanceObject.favourHorizontalStrike)
                        {
                            appropriateStrike = true;
                            if (closestPoint == DataTracker.pointNames.TrueEdge ||
                                closestPoint == DataTracker.pointNames.FalseEdge)
                            {
                                appropriateContactPoint = true;
                            }
                        }

                        break;

                    case SwingDetection.swingDir.DiagonalUpwards:
                        if (swordPos.activeStanceObject.favourUpwardsStrike ||
                            swordPos.activeStanceObject.favourHorizontalStrike)
                        {
                            appropriateStrike = true;
                            if (closestPoint == DataTracker.pointNames.TrueEdge ||
                                closestPoint == DataTracker.pointNames.FalseEdge)
                            {
                                appropriateContactPoint = true;
                            }
                        }

                        break;
                }

                var damage = CalculateDamage(appropriateStrike, appropriateContactPoint);
                hitpoints -= damage;
                GameObject floatingText = Instantiate(floatingDamage, collisionPoint, transform.rotation, null);
                floatingText.GetComponent<DamageText>()
                    .SetDamageText("Strike type is: " + strikeType+ "Damage: "+ damage);
                
            }
        }
    }

    float CalculateDamage(bool appropriateStrike, bool appropriateContactPoint)
        {
            float baseDamage = 25;
            float strikeDirectionMulti = 0.5f;
            float contactPointMulti = 0.5f;
            
            
            
            //Increase damage if correct direction and/or contact point were used
            float damage = baseDamage;
            if (appropriateStrike)
            {
                damage += baseDamage * strikeDirectionMulti;
            }
            if (appropriateContactPoint)
            {
                damage += baseDamage * contactPointMulti;
            }
            return damage;
        }

        void SetGuard(int guardNum)
        {
            switch (guardNum)
            {
                case 0:
                    targetGuard = StanceObject.guardPose.Alber;
                    GetComponent<Renderer>().material = AlberMat;
                    break;
                case 1:
                    targetGuard = StanceObject.guardPose.Ox;
                    GetComponent<Renderer>().material = OxMaterial;
                    break;
                case 2:
                    targetGuard = StanceObject.guardPose.Pflug;
                    GetComponent<Renderer>().material = PflugMat;
                    break;
                case 3:
                    targetGuard = StanceObject.guardPose.Vomtag;
                    GetComponent<Renderer>().material = VomtagMaterial;
                    break;
            }
        }

        SwingDetection.swingDir CalcSwingDirection(DataTracker dataTracker)
        {
            SwingDetection.swingDir newSwingDir = SwingDetection.swingDir.Null;
            var swingData = sword.GetComponent<SwingDetection>();
            bool verticalValid = false;
            bool upwards = false;
            bool horizontalValid = false;
            bool thrust = false;
            Vector3 startPosVertical = dataTracker.positionLog[0];
            Vector3 startPosHorizontal = dataTracker.positionLog[0];
            float verticalDistance = 0;
            float horizontalDistance = 0;
            
            
            foreach (var pos in dataTracker.positionLog)
            {
                // GREATEST Y DISTANCE -> FOR VERTICAL
                //GREATEST X,Z DISTANCE -> FOR HORIZONTAL
                Vector3 newVertPos = new Vector3(0, pos.y, 0);
                Vector3 collisionVertPos = new Vector3(0, collisionPoint.y, 0);
                Vector3 newHorizPos = new Vector3(pos.x, 0, pos.z);
                Vector3 collisionHorizPos = new Vector3(collisionPoint.x, 0, collisionPoint.z);

                float newDistance = Vector3.Distance(newVertPos, collisionVertPos);
                if (newDistance > verticalDistance)
                {
                    verticalDistance = newDistance;
                    startPosVertical = pos;
                }

                newDistance = Vector3.Distance(newHorizPos, collisionHorizPos);
                if (newDistance > horizontalDistance)
                {
                    horizontalDistance = newDistance;
                    startPosHorizontal = pos;
                }
            }

            //VERTICAL
            Vector3 startY = new Vector3(0, startPosVertical.y, 0);
            Vector3 endY = new Vector3(0, collisionPoint.y, 0);
            Vector3 direction = Vector3.Normalize(endY - startY);
            if (Vector3.Dot(direction, Vector3.up) > 0.6f && verticalDistance >= swingData.minimumVerticalDisplacement)
            {
                upwards = true;
                verticalValid = true;
            }
            else if (Vector3.Dot(direction, Vector3.up) < -0.6f &&
                     verticalDistance >= swingData.minimumVerticalDisplacement)
            {
                upwards = false;
                verticalValid = true;
            }


            //HORIZONTAL
            Vector3 startHorizontal = new Vector3(startPosHorizontal.x, 0, startPosHorizontal.z);
            Vector3 endHorizontal = new Vector3(collisionPoint.x, 0, collisionPoint.z);
            float distanceHorizontal = Vector3.Distance(startHorizontal, endHorizontal);
            direction = Vector3.Normalize(endHorizontal - startHorizontal);

            float directionDot = Vector3.Dot(direction, playerCam.transform.forward);
            if (directionDot < 0.8 && directionDot > -0.8 &&
                distanceHorizontal >= swingData.minimumHorizontalDisplacement)
            {
                horizontalValid = true;
            }
            else if (Vector3.Dot(direction, sword.transform.forward) > 0.7f &&
                     swordPos.currentDirection == StanceObject.generalDirection.Forwards &&
                     distanceHorizontal >= swingData.minimumForwardsDisplacement)
            {
                thrust = true;
            }


            if (verticalValid && !horizontalValid)
            {
                newSwingDir = upwards ? SwingDetection.swingDir.Upwards : SwingDetection.swingDir.Downwards;
            }
           
            else if (horizontalValid)
            {
                if (verticalValid)
                {
                    newSwingDir = upwards
                        ? SwingDetection.swingDir.DiagonalUpwards
                        : SwingDetection.swingDir.DiagonalDownwards;
                }
                else
                {
                    newSwingDir = SwingDetection.swingDir.Horizontal;
                }
                
            }
            else if (thrust)
            {
                newSwingDir = SwingDetection.swingDir.Thrust;
            }




            return newSwingDir;



            return SwingDetection.swingDir.Null;
        }

}
