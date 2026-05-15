using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public GameObject DropObjPrefab; 

    public void Drop()
    {
        Debug.Log("µôÂä");
        GameObject.Instantiate(DropObjPrefab, transform.position, transform.rotation);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
}
