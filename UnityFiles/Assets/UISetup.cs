using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UISetup : MonoBehaviour
{
    public TextMeshProUGUI swordGrab;
    public TextMeshProUGUI guardposvalues;
    public TextMeshProUGUI edgeposvalues;
    public TextMeshProUGUI pointposvalues;
    public TextMeshProUGUI swordposevalue;
    public TextMeshProUGUI relative;
    public TextMeshProUGUI plane;
    public TextMeshProUGUI direction;
    public GameObject swordPos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if sword.grabbed, swordGrab text = true or false
        guardposvalues.text = "Update 1: "+ swordPos.GetComponent<SwordGuardData>().crossguardPositionLog[0] + "\n" + "Update 2: " + swordPos.GetComponent<SwordGuardData>().crossguardPositionLog[1] + "\n" + "Update 3: " + swordPos.GetComponent<SwordGuardData>().crossguardPositionLog[2] + "\n" + "Update 4: " + swordPos.GetComponent<SwordGuardData>().crossguardPositionLog[3] + "\n" + "Update 5: " + swordPos.GetComponent<SwordGuardData>().crossguardPositionLog[4];
        edgeposvalues.text = "Update 1: "+ swordPos.GetComponent<SwordGuardData>().edgePositionLog[0] + "\n" + "Update 2: " + swordPos.GetComponent<SwordGuardData>().edgePositionLog[1] + "\n" + "Update 3: " + swordPos.GetComponent<SwordGuardData>().edgePositionLog[2] + "\n" + "Update 4: " + swordPos.GetComponent<SwordGuardData>().edgePositionLog[3] + "\n" + "Update 5: " + swordPos.GetComponent<SwordGuardData>().edgePositionLog[4];
        pointposvalues.text = "Update 1: "+ swordPos.GetComponent<SwordGuardData>().pointPositionLog[0] + "\n" + "Update 2: " + swordPos.GetComponent<SwordGuardData>().pointPositionLog[1] + "\n" + "Update 3: " + swordPos.GetComponent<SwordGuardData>().pointPositionLog[2] + "\n" + "Update 4: " + swordPos.GetComponent<SwordGuardData>().pointPositionLog[3] + "\n" + "Update 5: " + swordPos.GetComponent<SwordGuardData>().pointPositionLog[4];
        relative.text = "Relativity: " + swordPos.GetComponent<SwordGuardData>().currentRelativityToHead;
        plane.text = "Plane: " + swordPos.GetComponent<SwordGuardData>().currentPlane;
        direction.text = "Direction: " + swordPos.GetComponent<SwordGuardData>().currentDirection;
        swordposevalue.text = "Guard: " + swordPos.GetComponent<SwordGuardData>().currentguard;
        // swordposvalues from sword, last 5
        // Predict the location based on relative to head and direction of point.
    }
}
