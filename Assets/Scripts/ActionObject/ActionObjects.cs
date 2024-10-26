using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EventBridge;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using StarterAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActionObjects : MonoBehaviour
{
    public GameObject triggerCheckObject;
    public bool triggerCheck=true;
    public bool actionWithHit=false;
    public bool canReuseAfterDone=false;
    public GameObject actionButtonDrawPanel;
    GameObject MyOwnObj;
    public TextMeshProUGUI actionSingleText;
    public TextMeshProUGUI actionMultiText;
    public string ActionMultiText="";
    public Animator animator;
    // Start is called before the first frame update
    private IComponentEventHandler m_handler;
    private Outline outline;
    public bool doneCheck;
    public FirstPersonController firstPersonController;
    public void Start(){
        MyOwnObj=this.gameObject;
        m_handler = triggerCheckObject.transform.RequestEventHandlers();
        if(triggerCheck==true){
            m_handler.TriggerStay += TriggerStay;
            m_handler.TriggerExit += TriggerExit;
        }else{
            m_handler.CollisionEnter += CollisionEnter;
            m_handler.CollisionStay += CollisionStay;
        }
        outline=this.gameObject.GetComponent<Outline>();
        if (outline!=null){
            outline.enabled=false;
        }
    }
    public virtual void Action(){
        //if you need animation. Add "base.Action();" in child Class
        if(animator!=null){
            Debug.Log("yeah");
            animator.SetBool("Active",true);
            doneCheck=true;
        }
    }

    void TriggerStay(Collider col){//もしカーソルが合ったら
        if(!doneCheck){
            if(col.gameObject==MyOwnObj){
                if(actionWithHit==false){
                    //Debug.Log("TriggerStay_カーソルが当たった");
                    actionSingleText.text="E";
                    actionMultiText.text=ActionMultiText;
                    actionButtonDrawPanel.SetActive(true);
                    var keyboard = Keyboard.current;
                    if(keyboard.eKey.isPressed){
                        Action();
                        actionButtonDrawPanel.SetActive(false);
                    }
                    if(outline!=null){
                        outline.enabled=true;
                    }
                }else{
                    if(col.gameObject==MyOwnObj){
                        //Debug.Log("TriggerStay_身体が当たった");
                        Action();
                    }
                }
            }
        }
    }
    void TriggerExit(Collider col){
        if(col.gameObject==MyOwnObj){
            //Debug.Log("TriggerExit");
            actionButtonDrawPanel.SetActive(false);
        }
        if(outline!=null){
            outline.enabled=false;
        }
    }
    void CollisionEnter(Collision col){
        if(!doneCheck){
            if(col.gameObject==MyOwnObj){
                if(actionWithHit==false){
                    //Debug.Log("CollisionEnter_カーソルが当たった");
                    actionSingleText.text="E";
                    actionMultiText.text=ActionMultiText;
                    actionButtonDrawPanel.SetActive(true);
                    var keyboard = Keyboard.current;
                    if(keyboard.eKey.isPressed){
                        NowActionFalse();
                        Action();
                        actionButtonDrawPanel.SetActive(false);
                    }
                    if(outline!=null){
                        outline.enabled=true;
                    }
                }else{
                    if(col.gameObject==MyOwnObj){
                        //Debug.Log("CollisionEnter_身体が当たった");
                        Action();
                    }
                }
            }
        }
    }

    void CollisionStay(Collision col){
        if(col.gameObject==MyOwnObj){
            //Debug.Log("CollisionStay_カーソル当たり続けてるるる");
            actionButtonDrawPanel.SetActive(false);
        }
        if(outline!=null){
            outline.enabled=false;
        }
    }
    public void NowActionTrue(){
        firstPersonController.NowAction=true;
    }
    public void NowActionFalse(){
        firstPersonController.NowAction=false;
    }
}

/*#if UNITY_EDITOR
    [CustomEditor(typeof(ActionObjects),true)]
    // 継承クラスは Editor を設定する
    public class ActionObjectsEditor : Editor
    {
        // GUIの表示関数をオーバーライドする
        public override void OnInspectorGUI()
        {
            // 元のインスペクター部分を表示
            base.OnInspectorGUI();
            ActionObjects t = target as ActionObjects;
            // private関数を実行するボタンの作成
            if (GUILayout.Button("LocateInit"))
            {
                //var actionObjects=this.gameObject.GetComponent<ActionObjects>();
                //t.LocateInit();//もしpublic関数だったらこっ地で動く
                 t.SendMessage("LocateInit", null, SendMessageOptions.DontRequireReceiver);//private関数だったらこっち
                //ActionObjects.SendMessage("LocateInit", null, SendMessageOptions.DontRequireReceiver);
            }
            

        }
    }
#endif*/