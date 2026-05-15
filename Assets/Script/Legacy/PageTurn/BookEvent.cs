using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookEvent : MonoBehaviour
{
    public BookUI bookui;

    public void Event_FanYeOver()
    {
        bookui.ChangePageNext();
    }

    public void Event_FanYeOver1()
    {
        bookui.ChangePagePre();
    }
}
