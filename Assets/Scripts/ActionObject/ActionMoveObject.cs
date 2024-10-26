using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMoveObject : ActionObjects
{
    public GameObject moveObject;
    public Transform moveTransform;
    // Start is called before the first frame update
    public override void Action()
    {
        NowActionTrue();
        Debug.Log("Before="+moveObject.transform.position);
        moveObject.transform.position=moveTransform.position;
        moveObject.transform.rotation=moveTransform.rotation;
        Debug.Log("After="+moveObject.transform.position);
        Debug.Log("あだだ");
        Invoke("NowActionFalse",0.1f);
    }
    
}
