using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    public GameObject rotateObject;
    private GameObject arrowObject;
    private Transform targetObject;
    public GameObject targetPoins;
    private Transform temp;
    private Material targetMaterial;
    private bool active=false;
    [System.Serializable]
    public struct Lengths{
        public float min;
        public float max;
        public float minDistance;
    }
    [SerializeField]
    public Lengths len;

    void Start(){
        if(len.max<len.min){
            var temp=len.min;
            len.min=len.max;
            len.max=temp;
        }if(len.min<0f){
            len.min=0f;
        }if(len.max-len.min<len.minDistance){
            len.max=len.min=len.minDistance;
        }
        arrowObject=rotateObject.transform.GetChild(0).gameObject;
    }
    public void Check(int scene){
        var temp = targetPoins.transform.Find("Scene"+scene);
        if(temp!=null){
            active=true;
            targetObject=temp;
            targetMaterial=arrowObject.gameObject.GetComponent<MeshRenderer>().material;
            rotateObject.SetActive(true);
        }else{
            rotateObject.SetActive(false);
            active=false;
        }
    }
    void Update()
    {
        if(active){
            rotateObject.transform.LookAt(targetObject.transform.position);
            //Debug.Log("rotation="+rotateObject.transform.rotation);
            
            float dis = Vector3.Distance(rotateObject.transform.position, targetObject.position);
            if(targetMaterial!=null){
                Color tempColor=targetMaterial.color;
                if(dis<len.min){
                    tempColor.a=0f;
                }else if(len.min<=dis && dis<=len.max){
                    var tmax=len.max-len.min;
                    var tdis=dis-len.min;
                    var x=tdis/tmax;
                    var y=x*x;
                    tempColor.a=y;
                    Debug.Log("tempColor.a="+tempColor.a);
                }else if(len.max<dis){
                    tempColor.a=1f;
                }
                targetMaterial.color=tempColor;
            }
        }
    }
}
