using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNode : MonoBehaviour
{
    public int id;
    [SerializeField]private GameObject cameraObject;
    private void Start()
    {
        GameEventManager.current.onRoomTriggerEnter += OnCameraMove;
        cameraObject = GameObject.Find("MainCamera");
    }

    private void OnCameraMove(int id)
    {
        if (id == this.id)
        {
            cameraObject.transform.position = gameObject.transform.position;
        }
    }
}
