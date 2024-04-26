using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabHandPose : MonoBehaviour
{
    public GameObject playerLeftHand;
    public GameObject playerRightHand;
    public GameObject upperLeftHand;
    public GameObject upperRightHand;
    public GameObject lowerLeftHand;
    public GameObject lowerRightHand;
    public HandData rightHandUpperPose;
    public HandData leftHandUpperPose; 
    public HandData rightHandLowerPose;
    public HandData leftHandLowerPose;
    public GameObject xrRig;

    private Vector3 startingHandPositionUpper;
    private Vector3 finalHandPositionUpper;
    private Quaternion startingHandRotationUpper;
    private Quaternion finalHandRotationUpper;
    private Quaternion[] startingFingerRotationsUpper;
    private Quaternion[] finalFingerRotationsUpper;
    
    private Vector3 startingHandPositionLower;
    private Vector3 finalHandPositionLower;
    private Quaternion startingHandRotationLower;
    private Quaternion finalHandRotationLower;
    private Quaternion[] startingFingerRotationsLower;
    private Quaternion[] finalFingerRotationsLower;
    private int grabState = 0;
    private GameObject firstGrabHand;
    private GameObject secondGrabHand;
    
    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(setupPose);
        grabInteractable.selectExited.AddListener(unsetPose);
       rightHandUpperPose.gameObject.SetActive(false);
        rightHandLowerPose.gameObject.SetActive(false);
        leftHandUpperPose.gameObject.SetActive(false);
        leftHandLowerPose.gameObject.SetActive(false);
        grabState = 0;
    }

    // Update is called once per frame
    public void setupPose(BaseInteractionEventArgs arg)
    {
        if (arg.interactorObject is XRDirectInteractor)
        {
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = false;
            
            if (handData.GetComponent<HandData>().handType == HandData.HandModelType.Left && grabState == 0)
            {
                transform.parent = xrRig.transform;
                SetHandDataValuesUpper(handData,leftHandUpperPose);
                leftHandUpperPose.gameObject.SetActive(true);
                playerLeftHand.SetActive(false);
                firstGrabHand = playerLeftHand;
                grabState = 1;
            }
            else if (handData.GetComponent<HandData>().handType == HandData.HandModelType.Right && grabState == 0)
            {
                transform.parent = xrRig.transform;
                SetHandDataValuesUpper(handData,rightHandUpperPose);
                rightHandUpperPose.gameObject.SetActive(true);
                playerRightHand.SetActive(false);
                firstGrabHand = playerRightHand;
                grabState = 1;
            }
            else if (handData.GetComponent<HandData>().handType == HandData.HandModelType.Left && grabState >= 1)
            {
                SetHandDataValuesLower(handData,leftHandLowerPose);
                leftHandLowerPose.gameObject.SetActive(true);
                playerLeftHand.SetActive(false);
                secondGrabHand = playerLeftHand;
                grabState = 2;
            }
            else if (handData.GetComponent<HandData>().handType == HandData.HandModelType.Right && grabState >= 1)
            {
                SetHandDataValuesLower(handData,rightHandLowerPose);
                rightHandLowerPose.gameObject.SetActive(true);
                playerRightHand.SetActive(false);
                secondGrabHand = playerRightHand;
                grabState = 2;
            }


            if (grabState == 1)
            {
             //   SetHandData(handData,finalHandPositionUpper,finalHandRotationUpper,finalFingerRotationsUpper);
            }
            else if (grabState == 2)
            {
            //    SetHandData(handData,finalHandPositionLower,finalHandRotationLower,finalFingerRotationsLower);
            }
            
            
        }
        
    }

    public void unsetPose(BaseInteractionEventArgs arg)
    {
        if (arg.interactorObject is XRDirectInteractor)
        {
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = true;


            switch (grabState)
            {
                case 1:
                    //SetHandData(handData,startingHandPositionUpper,startingHandRotationUpper,startingFingerRotationsUpper);
                    if (leftHandUpperPose.gameObject.activeInHierarchy)
                    {
                        leftHandUpperPose.gameObject.SetActive(false);
                        playerLeftHand.SetActive(true);
                    }
                    else if (rightHandUpperPose.gameObject.activeInHierarchy)
                    {
                        rightHandUpperPose.gameObject.SetActive(false);
                        playerRightHand.SetActive(true);
                    }
                    //SET WHICHEVER UPPER TO NOT VISIBLE
                    //SET THAT SIDE OF PLAYER TO VISIBLE
                    grabState = 0;
                    transform.parent = null;
                    break;
                case 2:
                  //  SetHandData(handData,startingHandPositionLower,startingHandRotationLower,startingFingerRotationsLower);

                  if (leftHandLowerPose.gameObject.activeInHierarchy)
                  {
                      if (handData.GetComponent<HandData>().handType != HandData.HandModelType.Left)
                      {
                          leftHandUpperPose.gameObject.SetActive(true);
                          leftHandLowerPose.gameObject.SetActive(false);
                          rightHandUpperPose.gameObject.SetActive(false);
                          playerRightHand.SetActive(true);
                      }
                      else
                      {
                          leftHandLowerPose.gameObject.SetActive(false);
                          playerLeftHand.SetActive(true);
                      }
                  }
                  else if (rightHandLowerPose.gameObject.activeInHierarchy)
                  {
                      if (handData.GetComponent<HandData>().handType != HandData.HandModelType.Right)
                      {
                          rightHandUpperPose.gameObject.SetActive(true);
                          rightHandLowerPose.gameObject.SetActive(false);
                          leftHandUpperPose.gameObject.SetActive(false);
                          playerLeftHand.SetActive(true);
                      }
                      else
                      {
                          rightHandLowerPose.gameObject.SetActive(false);
                          playerRightHand.SetActive(true);
                      }
                  }

                  //SET WHICHEVER LOWER TO NOT VISIBLE
                    //SET THAT SIDE OF PLAYER TO VISIBLE
                    grabState = 1;
                    break;
            }
        }
        
    }

    public void SetHandDataValuesUpper(HandData h1, HandData h2)
    {
        startingHandPositionUpper = h1.root.localPosition;
        finalHandPositionUpper = h2.root.localPosition;

        startingHandRotationUpper = h1.root.localRotation;
        finalHandRotationUpper = h2.root.localRotation;
        
        startingFingerRotationsUpper = new Quaternion[h1.fingerBones.Length];
        finalFingerRotationsUpper = new Quaternion[h1.fingerBones.Length];

        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            startingFingerRotationsUpper[i] = h1.fingerBones[i].localRotation;
            finalFingerRotationsUpper[i] = h2.fingerBones[i].localRotation;
        }
    }

    public void SetHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;

        for (int i = 0; i < newBonesRotation.Length; i++)
        {
            h.fingerBones[i].localRotation = newBonesRotation[i];
        }
        
    }
    public void SetHandDataValuesLower(HandData h1, HandData h2)
    {
        startingHandPositionLower = h1.root.localPosition;
        finalHandPositionLower = h2.root.localPosition;

        startingHandRotationLower = h1.root.localRotation;
        finalHandRotationLower = h2.root.localRotation;
        
        startingFingerRotationsLower = new Quaternion[h1.fingerBones.Length];
        finalFingerRotationsLower = new Quaternion[h1.fingerBones.Length];

        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            startingFingerRotationsLower[i] = h1.fingerBones[i].localRotation;
            finalFingerRotationsLower[i] = h2.fingerBones[i].localRotation;
        }
    }

    
        
}

