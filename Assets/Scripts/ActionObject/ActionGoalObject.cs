using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGoalObject : ActionObjects {
    public GameManager gameManager;
    public override void Action(){
        doneCheck=true;
        gameManager.NextScene();
    }
}
