using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerController : MonoBehaviour
{
    public Transform mainObj;
    public Transform targetObj;
    public float needTime=4.0f;
    float time=0.0f;
    private bool waterCheck=false;
    
    [Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;
    [Header("Player Sound")]
		public AudioSource audioSource;
		private float shoesSoundTime=0f;
		private bool shoesLeft=false;
		public float NeedShoesSoundTime=1f;
		public AudioClip WalkingGroundClipLeft;
		public AudioClip WalkingGroundClipRight;
		public AudioClip EnterRiverClip;
		public AudioClip ExitRiverClip;
    
    [Tooltip("Move speed on the Ground")]
    public float forwardSpeed=1.0f;
    [Tooltip("Move speed in the River")]
		public float RiverMoveSpeed = 4.0f;
    // Start is called before the first frame update
    void Start()
    {
        time=0;
    }

    // Update is called once per frame
    void Update()
    {
        time+=Time.deltaTime;
        if(time>needTime){
            time=0.0f;
        }
        // ターゲット方向のベクトルを取得
        Vector3 relativePos = targetObj.transform.position - mainObj.transform.position;
        // 方向を、回転情報に変換
        Quaternion rotation = Quaternion.LookRotation (relativePos);
        float temp=time/needTime;
        mainObj.rotation=Quaternion.Slerp(mainObj.rotation, rotation, temp);
        MoveForward();

        MoveSound();
    }
    private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}
    void MoveForward(){
        if(waterCheck==true){
            mainObj.Translate(Vector3.forward*RiverMoveSpeed*Time.deltaTime);
        }else{
            mainObj.Translate(Vector3.forward*forwardSpeed*Time.deltaTime);
        }
        
    }
    void MoveSound(){
        if(Grounded==true && waterCheck==false){//WalkingSound
            shoesSoundTime+=Time.deltaTime;
            if(shoesSoundTime>NeedShoesSoundTime){
                if(shoesLeft==true){
                    audioSource.PlayOneShot(WalkingGroundClipLeft);
                    shoesLeft=false;
                }else{
                    audioSource.PlayOneShot(WalkingGroundClipRight);
                    shoesLeft=true;
                }
                shoesSoundTime=0f;
            }
        }
    }
    void OnTriggerEnter(Collider collider){
        if(collider.gameObject.tag=="River"){
            audioSource.PlayOneShot(EnterRiverClip);
            waterCheck=true;
        }
    }
    void OnTriggerExit(Collider collider){
        if(collider.gameObject.tag=="River"){
            audioSource.PlayOneShot(ExitRiverClip);
            waterCheck=false;
        }
    }
}