using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMGM : SingletonMono<ChangeMGM>
{
    public AudioClip m_MainAC;
    public AudioClip m_BossFightAC;

    public AudioSource As;
    public void ChangeBGMToBossFight()
    {
        As.clip = m_BossFightAC;
        As.Play();
    }

    
}
