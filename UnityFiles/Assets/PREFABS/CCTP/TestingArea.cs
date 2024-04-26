using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingArea : MonoBehaviour
{
    public bool player_in_range = false;

    public Material RedMaterial;
    public Material GreenMaterial;

    public GameObject interaction_area;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player_in_range = true;
            interaction_area.GetComponent<Renderer>().material = GreenMaterial;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player_in_range = false;
            interaction_area.GetComponent<Renderer>().material = RedMaterial;
        }
    }
}
