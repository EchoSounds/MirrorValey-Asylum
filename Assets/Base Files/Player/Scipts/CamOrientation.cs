using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamOrientation : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] bool followX, followY, followZ;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (targetObject != null)
        {
            // Get the target object's y rotation
            Quaternion targetRotation = targetObject.transform.rotation;

            // Set the current object's y rotation to match the target object's y rotation
            Vector3 eulerAngles = transform.rotation.eulerAngles;
            if(followX)eulerAngles.x = targetRotation.eulerAngles.x;
            if(followY)eulerAngles.y = targetRotation.eulerAngles.y;
            if(followZ)eulerAngles.z = targetRotation.eulerAngles.z;
            transform.rotation = Quaternion.Euler(eulerAngles);
        }
        else if (targetObject == null)
        {
            Debug.Log("No Target Set for Orientation");
        }
    }
}