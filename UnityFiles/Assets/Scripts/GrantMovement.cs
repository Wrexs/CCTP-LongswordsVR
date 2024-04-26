using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantMovement : MonoBehaviour
{
    public GameObject characterSwitcher;
    // Start is called before the first frame update
    public void Swapcharacters()
    {
        characterSwitcher.GetComponent<SwitchCharacters>().SwitchPlayers();
    }
}
