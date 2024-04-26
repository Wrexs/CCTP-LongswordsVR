using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonVR : MonoBehaviour
{
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    private GameObject presser;
    private AudioSource sound;
    private bool isPressed;
    public GameObject littleGuy;
    public GameObject avatar;
    public List<GameObject> avatarHands;
    public List<GameObject> littleguyHands;
    public GameObject littleguyVR;
    public GameObject door;
    public float moveOffset;
    public GameObject target;
    public GameObject original;
    public Material camerafeed;
    public Material off;
    public GameObject screen;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed)
        {
            button.transform.localPosition = new Vector3(0, 0.003f, 0);
            presser = other.gameObject;
           onPress.Invoke();
            sound.Play();
            isPressed = true;
            Debug.Log("BUTTON PRESSED");
        }
    }
/*
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser)
        {
            button.transform.localPosition = new Vector3(0, 0.015f, 0);
            onRelease.Invoke();
            isPressed = false;
        }
        
    }
    */

    public void EnableMovement()
    {
      // GameObject.FindWithTag("Player").GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
        avatar.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
      littleGuy.GetComponent<ThirdPersonController>().MoveSpeed = 2.0f;
       littleGuy.GetComponent<ThirdPersonController>().SprintSpeed = 5.335f;
       littleGuy.GetComponent<ThirdPersonController>().canMove = true;
        littleGuy.GetComponent<XRGrabInteractable>().enabled = false;
        
        Invoke(nameof(DisableMovement),5f);
       
        
    }

    public void cameraOn()
    {
        screen.GetComponent<Renderer>().material = camerafeed;
        Invoke("cameraOff",5);
    }

    public void cameraOff()
    {
        button.transform.localPosition = new Vector3(0, 0.015f, 0);
        isPressed = false;
        sound.Play();
        screen.GetComponent<Renderer>().material = off;
    }
    
    
    public void OpenDoor()
    {
        door.GetComponent<Door>().SlideOpen();
      

    }

    public void CloseDoor()
    {
     
    }

    public void EnableHands()
    {
        for (int i = 0; i < avatarHands.Count; i++)
        {
            avatarHands[i].gameObject.SetActive(false);
        }
     for (int i = 0; i < littleguyHands.Count; i++)
        {
            littleguyHands[i].gameObject.SetActive(true);
        }
  
        Invoke(nameof(DisableHands),5f);
        
    }

    public void DisableMovement()
    {
        button.transform.localPosition = new Vector3(0, 0.015f, 0);
        isPressed = false;
        sound.Play();
        //GameObject.FindWithTag("Player").GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
        avatar.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
        
        littleGuy.GetComponent<ThirdPersonController>().MoveSpeed = 0;
       littleGuy.GetComponent<ThirdPersonController>().canMove = false;
        littleGuy.GetComponent<XRGrabInteractable>().enabled = true;
        
    }

    public void DisableHands()
    {
        for (int i = 0; i < avatarHands.Count; i++)
        {
            avatarHands[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < littleguyHands.Count; i++)
        {
            littleguyHands[i].gameObject.SetActive(false);
        }
        button.transform.localPosition = new Vector3(0, 0.015f, 0);
        isPressed = false;
        sound.Play();
        
    }


    // Update is called once per frame
    
}
