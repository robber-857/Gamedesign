using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoxController : MonoBehaviour
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
        audioSource.Play();
        animator.SetTrigger("smash");
    }


    public void DestoryItem()
    {
        Destroy(gameObject);
    }
}
