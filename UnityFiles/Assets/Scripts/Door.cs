using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class Door : MonoBehaviour
{
    public bool IsOpen = false;

    private Vector3 startPosition;

    private Vector3 endPosition;

    public float SlideAmount;

    public float Speed = 1f;

    private Vector3 SlideDirection = Vector3.up;
    private float time = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        while (time < 3 && IsOpen)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
           
            time += Time.deltaTime * Speed;
        }
    }

    public void SlideOpen()
    {
        endPosition = startPosition + SlideAmount * SlideDirection;
        startPosition = transform.position;

         time = 0;
        IsOpen = true;
        
    }

    public void SlideClose()
    {
        endPosition = startPosition;
        Vector3 StartPosition = transform.position;
        float time = 0;
        IsOpen = false;
        while (time < 1)
        {
            transform.position = Vector3.Lerp(StartPosition, endPosition, time);
           
            time += Time.deltaTime * Speed;
        }


    }
}
