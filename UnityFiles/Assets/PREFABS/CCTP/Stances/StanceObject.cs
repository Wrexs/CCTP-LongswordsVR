using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StanceScriptableObject", order = 1)]
public class StanceObject : ScriptableObject
{
    
    public string stanceName;
    public enum guardPose
    {
        Vomtag = 0,
        Alber = 1,
        Ox = 2,
        Pflug = 3,
        Null = 4,
    }
    public enum generalDirection
    {
        Forwards = 0,
        Upwards = 1,
        Downwards = 2,
        Null = 3,
    }
    public enum relativityToHead
    {
        AboveHead = 0,
        BelowHead = 1,
        Null = 2,
    }
    public enum plane
    {
        Horizontal = 0,
        Vertical = 1,
        Null = 2,
    }

    public bool favourDownwardsStrike;
    public bool favourUpwardsStrike;
    public bool favourHorizontalStrike;
    public bool favourThrust;
    public generalDirection requiredGeneralDirection;
    public relativityToHead requiredRelativityToHead;
    public plane requiredPlane;
    public guardPose stanceID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
