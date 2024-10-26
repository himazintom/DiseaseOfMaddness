using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOrganObject : ActionObjects
{
    public GameManager gameManager;
    // Start is called before the first frame update
    public override void Action()
    {
        if(gameManager.saveData.sceneNum==13){
            gameManager.NextScene();
        }
    }
}
