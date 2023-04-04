using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager current;

    private void Awake()
    {
        current = this;
    }

    public event Action<int> onRoomTriggerEnter;
    public void RoomTriggerEnter(int id)
    {
        if (onRoomTriggerEnter != null)
        {
            onRoomTriggerEnter(id);
        }
    }
}
