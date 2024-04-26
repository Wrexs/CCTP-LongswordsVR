using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementSwitcher : MonoBehaviour
{
    public InputActionReference movementSwitch;
    private bool teleportMove = true;

    public ContinuousMoveProviderBase leftMovement;
    public XRRayInteractor rightTeleportRay;
    //public ActionBasedController rightTeleport;
    //public ContinuousMoveProviderBase leftMove;
    
    // Start is called before the first frame update
    void Start()
    {
        teleportMove = true;
        movementSwitch.action.performed += SwitchMovement;
        UpdateMoveSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SwitchMovement(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
        {
            teleportMove = !teleportMove;
            UpdateMoveSettings();
        }
    }

    private void UpdateMoveSettings()
    {
        if (teleportMove)
        {
            leftMovement.enabled = false;
            rightTeleportRay.enabled = true;
        }
        else
        {
            leftMovement.enabled = true;
            rightTeleportRay.enabled = false;
        }
        
    }
}
