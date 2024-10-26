using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace myTools{
    public static class LookAtCoroutine
    {
        public static IEnumerator RotateObject(GameObject rotateObject, GameObject targetObject, float duration)
        {
            float startTime = Time.time;
            int frame=0;
            Quaternion startRotation=rotateObject.transform.rotation;  // 回転開始時の角度
            GameObject DirectionObj = new GameObject("Direction");
            DirectionObj.transform.position=rotateObject.transform.position;
            DirectionObj.transform.LookAt(targetObject.transform.position);
            Quaternion targetRotation=DirectionObj.transform.rotation;
            targetObject.transform.LookAt(rotateObject.transform.position);
            Transform cameraObj=rotateObject.transform.Find("PlayerCameraRoot");
            Quaternion cStartRotation = cameraObj.transform.rotation;
            while (Time.time < startTime + duration)
            {
                float elapsedTime = Time.time - startTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                rotateObject.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                cameraObj.rotation = Quaternion.Slerp(cStartRotation, targetRotation, t);
                Debug.Log("frame="+frame);
                frame++;
                yield return null;
            }
        }
    }
}

