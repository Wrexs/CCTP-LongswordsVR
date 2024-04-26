using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SwitchCharacters : MonoBehaviour
{
    public GameObject vrPlayer;
    public List<GameObject> littleGuys;

    private bool VR_Mode = true;
    // Start is called before the first frame update
    void Start()
    {
        VR_Mode = true;
        vrPlayer.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
        for (int i = 0; i < littleGuys.Count; i++)
        {
            littleGuys[i].GetComponent<ThirdPersonController>().MoveSpeed = 0;
            littleGuys[i].GetComponent<ThirdPersonController>().SprintSpeed = 0;
            littleGuys[i].GetComponent<ThirdPersonController>().canMove = false;
            littleGuys[i].GetComponent<XRGrabInteractable>().enabled = true;
        }
       


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchPlayers()
    {
        if (VR_Mode)
        {
            vrPlayer.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
           // littleGuy.GetComponent<ThirdPersonController>().MoveSpeed = 2.0f;
          //  littleGuy.GetComponent<ThirdPersonController>().SprintSpeed = 5.335f;
         //  littleGuy.GetComponent<ThirdPersonController>().canMove = true;
                        //littleGuy.GetComponent<XRGrabInteractable>().enabled = false;
                        VR_Mode = false;


                        //enable movement for littleguy
                        //disable VR interactible on littleguy
        }
        else
        {
            vrPlayer.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
           //  littleGuy.GetComponent<ThirdPersonController>().MoveSpeed = 0;
         //    littleGuy.GetComponent<ThirdPersonController>().canMove = false;
       //      littleGuy.GetComponent<XRGrabInteractable>().enabled = true;
            VR_Mode = true;

            //disable movement for littleguy
            //enable VR interactible littleguy
        }
    }
}
