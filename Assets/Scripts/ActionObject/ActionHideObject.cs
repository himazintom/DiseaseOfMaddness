using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHideObject : ActionObjects
{
    public GameManager gameManager;
    public Transform player;
    public override void Action()
    {
        if(gameManager.saveData.sceneNum==3){//古木田から逃げるシーンだったら
            player.position=this.gameObject.transform.position;
            gameManager.NextScene();
        }
    }
}
