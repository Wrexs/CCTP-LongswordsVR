using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrailsHandler : MonoBehaviour
{
    public GameObject oxTrail;
    public GameObject pflugTrail;
    public GameObject alberTrail;
    public GameObject vomtagTrail;

    public GameObject sword;
    public Material orignalMat;
    public Material redMat;
    public Material greenMat;
    public Material yellowMat;
    public Material blueMat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<SwordGuardData>().isGuarding)
        {
            var currentGuard = GetComponent<SwordGuardData>().trackedGuard;

            switch (currentGuard)
            {
                case StanceObject.guardPose.Alber:
                    oxTrail.SetActive(false);
                    pflugTrail.SetActive(false);
                    alberTrail.SetActive(true);
                    vomtagTrail.SetActive(false);
                    sword.GetComponent<Renderer>().material = yellowMat;
                    break;
                case StanceObject.guardPose.Ox:
                    oxTrail.SetActive(true);
                    pflugTrail.SetActive(false);
                    alberTrail.SetActive(false);
                    vomtagTrail.SetActive(false);
                    sword.GetComponent<Renderer>().material = redMat;
                    break;
                case StanceObject.guardPose.Pflug:
                    oxTrail.SetActive(false);
                    pflugTrail.SetActive(true);
                    alberTrail.SetActive(false);
                    vomtagTrail.SetActive(false);
                    sword.GetComponent<Renderer>().material = greenMat;
                    break;
                case StanceObject.guardPose.Vomtag:
                    oxTrail.SetActive(false);
                    pflugTrail.SetActive(false);
                    alberTrail.SetActive(false);
                    vomtagTrail.SetActive(true);
                    sword.GetComponent<Renderer>().material = blueMat;
                    break;
                case StanceObject.guardPose.Null:
                    oxTrail.SetActive(false);
                    pflugTrail.SetActive(false);
                    alberTrail.SetActive(false);
                    vomtagTrail.SetActive(false);
                    sword.GetComponent<Renderer>().material = orignalMat;
                    break;
            }
            
        }
        else
        {
            oxTrail.SetActive(false);
            pflugTrail.SetActive(false);
            alberTrail.SetActive(false);
            vomtagTrail.SetActive(false);
            sword.GetComponent<Renderer>().material = orignalMat;
        }
        

    }
}
