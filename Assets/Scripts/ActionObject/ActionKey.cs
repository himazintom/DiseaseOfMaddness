using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionKey : ActionObjects
{
    public GameManager gameManager;
    

    new void Start(){
        base.Start();
        if(gameManager.saveData.underGroundKeyCheck==true){
            this.gameObject.SetActive(false);
        }
    }
    public override void Action()
    {
        if(gameManager.saveData.sceneNum==7){
            gameManager.saveData.underGroundKeyCheck=true;
            this.gameObject.SetActive(false);
            gameManager.NextScene();
        }
        
    }
}
