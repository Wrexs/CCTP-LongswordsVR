using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damagetext;
    public float lifetime = 3f;
    private GameObject playerCamera;
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GameObject.FindWithTag("MainCamera");
        StartCoroutine(DestroyAfterLifetime());

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation( transform.position - playerCamera.transform.position );
    }

    public void SetDamageText(string damageString)
    {
        damagetext.SetText(damageString);
    }
    
    public IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
        
    }
}
