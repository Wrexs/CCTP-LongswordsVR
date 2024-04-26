using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WristCanvas : MonoBehaviour
{
    private GameObject target;

    public InputActionAsset inputactions;

    public Canvas wristUICanvas;
    public TextMeshProUGUI targettext;

    private InputAction menu;
    // Start is called before the first frame update
    void Start()
    {
        menu = inputactions.FindActionMap("XRI RightHand Interaction").FindAction("RaycastMenu");
menu.Enable();
menu.performed += ToggleMenu;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        menu.performed -= ToggleMenu;
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        wristUICanvas.enabled = !wristUICanvas.enabled;
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        targettext.GetComponent<TextMeshProUGUI>().SetText(target.name);
    }

    public GameObject GetTarget()
    {
        return target;
    }

    public void ResetTarget()
    {
        targettext.GetComponent<TextMeshProUGUI>().SetText("NO TARGET SELECTED");
    }
}
