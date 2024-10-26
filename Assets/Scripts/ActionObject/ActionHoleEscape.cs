using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHoleEscape : ActionObjects
{
    public GameManager gameManager;
    public override void Action()
    {
        if(gameManager.saveData.sceneNum==5){//古木田から逃げるシーンだったら
            gameManager.NextScene();
        }
    }
}
