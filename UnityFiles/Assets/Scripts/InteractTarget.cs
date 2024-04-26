using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractTarget : MonoBehaviour
{
    public Canvas UIthing;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInteractTarget()
    {
        Debug.Log("Setting Target");
        UIthing.GetComponent<WristCanvas>().SetTarget(gameObject);
       
    }

    public void ResetInteractTarget()
    {
        Debug.Log("Resetting Target");
        UIthing.GetComponent<WristCanvas>().ResetTarget();
    }
}
