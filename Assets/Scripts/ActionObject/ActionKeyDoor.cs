using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionKeyDoor : ActionObjects
{
    public GameObject keyConnect;
    public GameObject chane;
    public GameManager gameManager;
    
    public override void Action(){
        if(gameManager.saveData.underGroundKeyCheck){//鍵を持ってたら
            keyConnect.SetActive(false);
            chane.SetActive(false);
            animator.SetBool("Active",true);
            this.gameObject.GetComponent<AudioSource>().Play();
            this.gameObject.GetComponent<BoxCollider>().enabled=false;
            doneCheck=true;
        }
    }
}
