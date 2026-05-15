using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryObjC : MonoBehaviour
{
    public float TTime;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Destroy(gameObject, TTime);
    }

    
}
