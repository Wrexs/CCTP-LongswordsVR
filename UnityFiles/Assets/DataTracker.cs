using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class DataTracker : MonoBehaviour
{
    private static int maxStorage = 5;
    public Vector3[] positionLog = new Vector3[maxStorage];
    public float collectionInterval = 0.2f;
    private float timer = 0;
    public enum pointNames
    {
        Point = 0,
        TrueEdge = 1,
        FalseEdge = 2,
        Crossguard = 3,
        Null = 4,
        
    }

    public pointNames pointName; 


    // Start is called before the first frame update
    void Start()
    {
        // Populate the array with default values at run-time, to avoid referencing errors for empty array elements.
        for (int i = 0; i < positionLog.Length; i++)
        {
            positionLog[i] = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        }

        timer = collectionInterval;

    }

    // Update is called once per frame
    void Update()
    {
        //When the timer has reached 0, shift array values up by 1 and record the current position into the last value.
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (timer <= 0)
        {
            for (int i = 1; i < positionLog.Length; i++)
            {
                positionLog[i - 1] = positionLog[i];
            }

            positionLog[maxStorage - 1] = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            timer = collectionInterval;
        }
       
    }


    private void DetermineCurrentStrikeType()
    {
        
    }
}
