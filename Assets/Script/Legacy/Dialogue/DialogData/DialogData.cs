using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DialogData",menuName ="Create Dialog/New Dialog")]
public class DialogData : ScriptableObject
{
    public List<DialogPiece> dialogData = new List<DialogPiece>();

}

[System.Serializable]
public class DialogPiece
{
    public Sprite sprite;

    public string name;

    [TextArea()]
    public string content;

    public AudioClip audioclip;

}
