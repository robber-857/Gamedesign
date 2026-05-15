using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour
{

    public float triggerDisappearTime = 3f;

    public float restoreTime = 6f;
    
    private Collider2D _iceCollider2D;    
    private SpriteRenderer _spriteRenderer; 

    private float _currentStayTime;   
    private bool _isPlayerOnIce;      
    private bool _isIceDisabled;      

    private void Awake()
    {
        _iceCollider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _iceCollider2D.enabled = true;
        _spriteRenderer.enabled = true;
    }

    private void Update()
    {
        if (_isPlayerOnIce && !_isIceDisabled)
        {
            _currentStayTime += Time.deltaTime;
            if (_currentStayTime >= triggerDisappearTime)
            {
                StartCoroutine(DisableIceThenRestore());
            }
        }
    }


    private IEnumerator DisableIceThenRestore()
    {
        _isIceDisabled = true;
        _iceCollider2D.enabled = false; 
        _spriteRenderer.enabled = false; 

  
        yield return new WaitForSeconds(restoreTime);

  
        _iceCollider2D.enabled = true;
        _spriteRenderer.enabled = true;
        _isIceDisabled = false;
        _currentStayTime = 0; 
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.tag=="Player")
        {
            _isPlayerOnIce = true;
            _currentStayTime = 0;
        }
    }


    private void OnCollisionExit2D(Collision2D other)
    {

        if (other.gameObject.tag=="Player")
        {
            _isPlayerOnIce = false;
            _currentStayTime = 0; 
        }
    }
    
    private void OnApplicationPause(bool pause)
    {
        if (pause) _currentStayTime = 0;
    }
}
