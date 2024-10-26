using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionKnifeObject : ActionObjects
{
    public GameManager gameManager;
    // Start is called before the first frame update
    new void Start(){
        base.Start();
        if(gameManager.saveData.knifeCheck==true){
            this.gameObject.SetActive(false);
        }
    }
    public override void Action()
    {
        if(gameManager.saveData.sceneNum==11){
            this.gameObject.SetActive(false);
            gameManager.NextScene();
        }
    }
}
