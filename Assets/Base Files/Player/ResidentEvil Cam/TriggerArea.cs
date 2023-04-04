using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    public int id;
    private void OnTriggerEnter(Collider other)
    {
        GameEventManager.current.RoomTriggerEnter(id);
    }
}
