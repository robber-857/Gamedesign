using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpItemController : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;

    private void Awake()
    {
        animator  = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            animator.SetTrigger("jump");
            other.gameObject.GetComponent<PlayerController>().JumpMaxPower();
            audioSource.Play();
        }
    }
}
