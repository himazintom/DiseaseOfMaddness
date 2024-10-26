using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithCamera : MonoBehaviour
{
    public Transform MainCamera;
    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.rotation=MainCamera.rotation;
    }
}
