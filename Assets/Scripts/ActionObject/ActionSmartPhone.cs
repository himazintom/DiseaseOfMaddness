using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSmartPhone : ActionObjects
{
    public GameManager gameManager;
    

    new void Start(){
        base.Start();
        if(gameManager.saveData.smartPhoneCheck){//もしすでにスマホを回収していたら
            this.gameObject.SetActive(false);
        }
    }
    public override void Action()
    {
        if(gameManager.saveData.sceneNum==1){//古木田から逃げるシーンだったら
            gameManager.saveData.smartPhoneCheck=true;
            this.gameObject.SetActive(false);
            gameManager.NextScene();
        }        
    }
}
