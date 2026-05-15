using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButtonOnEnemy : MonoBehaviour
{
    public PressButtonController controller;
    public void ChangeState()
    {
        Debug.Log("更改Press");
        controller.gameObject.SetActive(true);
    }
}
