using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBoxController : MonoBehaviour
{
    public Animator animator;
    
    public AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerAttack")
        {
            Smash();
        }
    }

    public void Smash()
    {
        TaskBoxManger.instance.AddSmashBox();
        audioSource.Play();
        animator.SetTrigger("smash");
    }


    public void DestoryItem()
    {
        Destroy(gameObject);
    }
    
    
    
}
