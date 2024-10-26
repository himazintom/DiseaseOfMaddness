using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using StarterAssets;
using TMPro;
using UnityEngine.Splines;
using UnityEngine.SceneManagement;
using myTools;

public class GameManager : MonoBehaviour
{
    [Tooltip("Select Scene you want to start before Entering Play Mode")]
    public int FirstSceneNum=-1;
    public StarterAssetsInputs starterAssetsInputs;
    public TextLoader textLoader;
    public SoundPlayer soundPlayer;
    public FollowPlayer followPlayer;
    public SplineAnimate splineAnimate;
    public DirectionArrow directionArrow;
    public GameObject playPanel;
    public GameObject gameoverPanel;
    public GameObject helpPanel;
    public GameObject talkingPanel;
    public GameObject LastSelectPalnel;
    public TextMeshProUGUI lastContentText;
    public TextMeshProUGUI helpText;
    public GameObject playerObj;
    public GameObject followerObj;
    public GameObject normalFollowerObj;
    public Animator followerAnimator;
    public GameObject actionTextObj;
    public GameObject spawnPoints;
    public float oneStepLength=1f;
    private int killCount=0;
    public SaveData saveData;
    int[] textSceneNum={0,2,4,6,8,10,12,14,15,16,17,18};
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("GameManagerのStartを通ったよ");
        saveData=new SaveData();
        var temp=SaveClass.LoadData<SaveData>("GameData");
        if(temp==default){//初期値
            saveData.sceneNum=0;
            saveData.playerPos=playerObj.transform.position;
            saveData.playerRotat=playerObj.transform.rotation;
            saveData.followerPos=followerObj.transform.position;
            saveData.followerRotat=followerObj.transform.rotation;
            saveData.smartPhoneCheck=false;
            saveData.underGroundKeyCheck=false;
            saveData.knifeCheck=false;
            saveData.trueEnd=false;
            
        }
        if(FirstSceneNum>=0){
            saveData.sceneNum=FirstSceneNum;
            if(FirstSceneNum==8){
                saveData.underGroundKeyCheck=true;
            }
        }
        normalFollowerObj.SetActive(false);
        textLoader.TextLoadStart();
        PanelReset();
        LoadScene(saveData.sceneNum);
    }

    void PanelReset(){
        gameoverPanel.SetActive(false);
        actionTextObj.SetActive(false);
        talkingPanel.SetActive(false);
        LastSelectPalnel.SetActive(false);
    }
    public void NextScene(){//読み込みシーン番号を増やす
        saveData.sceneNum++;
        LoadScene(saveData.sceneNum);
    }
    public void RetryTrigger(){
        gameoverPanel.SetActive(false);
        LoadScene(saveData.sceneNum);
    }
    public void LoadScene(int scene){//シーン読み込み
        var firstPersonController = playerObj.GetComponent<FirstPersonController>();
        firstPersonController.enabled=false;
        Debug.Log("シーン"+scene+"を読み込み開始します！");
        directionArrow.Check(scene);
        var tempPlayerSpawnObj = spawnPoints.transform.Find("Scene"+scene+"Player");
        var tempFollowerSpawnObj = spawnPoints.transform.Find("Scene"+scene+"Follower");
        if(tempPlayerSpawnObj!=null){
            Debug.Log("Before: PlyaerPos="+playerObj.transform.position+"  tempPos="+tempPlayerSpawnObj.transform.position);
            playerObj.transform.position=tempPlayerSpawnObj.position;//指定位置に移動
            var t = tempPlayerSpawnObj.rotation.eulerAngles;
            playerObj.transform.rotation=Quaternion.Euler(0f,t.y,0f);
            Debug.Log("After: PlyaerPos="+playerObj.transform.position+"  tempPos="+tempPlayerSpawnObj.transform.position);
            var tempCamera=playerObj.transform.Find("PlayerCameraRoot");
            if(tempCamera!=null){
                tempCamera.rotation=Quaternion.Euler(t.x, t.y, 0f);//y軸だけを代入
            }
            Debug.Log("Player移動しました");
        }if(tempFollowerSpawnObj!=null){
            followPlayer.CanFollowFalse();
            followerObj.transform.position=tempFollowerSpawnObj.position;
            followerObj.transform.rotation=tempFollowerSpawnObj.rotation;
        }
        if(Array.IndexOf(textSceneNum,scene)>-1){//もし会話シーンだったら
            //Debug.Log("会話シーン");
            Cursor.visible = true;
            playPanel.SetActive(false);
            talkingPanel.SetActive(true);
            starterAssetsInputs.SetCursorState(false);
            helpPanel.SetActive(false);//上のhelpmessageを消す
            playerObj.GetComponent<FirstPersonController>().enabled=false;//プレイヤーのコントロールをなくす
            followerObj.SetActive(false);
            if(followerObj.GetComponent<Rigidbody>()!=null){//rigidbodyコンポーネントを持っていたら
                followerObj.GetComponent<Rigidbody>().isKinematic=true;
            }
        }else{//行動シーンだったら
            followerObj.GetComponent<Animator>().enabled=true;
            followerObj.GetComponent<Rigidbody>().isKinematic=false;
            Cursor.visible = false;
            starterAssetsInputs.SetCursorState(true);
            //starterAssetsInputs.cursorInputForLook=true;
            playPanel.SetActive(true);
            helpPanel.SetActive(true);
            talkingPanel.SetActive(false);
            playerObj.GetComponent<FirstPersonController>().enabled=false;
            followerObj.SetActive(true);
            Invoke("CanFollowTrue",1.0f);
            Invoke("FirstPersonControllerTrue",0.1f);

            //followerObj.GetComponent<FollowerController>().enabled=true;
        }
    
        switch (scene){
            case 0://s橋の上を移動中
                soundPlayer.Play("BigRiver",SoundPlayer.PlaySoundType.WorldBGM);
                textLoader.TextSceneSelect(0);
                break;
            case 1://スマホを探す
                helpText.text="川の近くにあるスマホを探せ";//上のメッセージ変更
                followerObj.SetActive(false);
                break;
            case 2://古木田誕生
                ChangeSpawnPoint(3);
                followerAnimator.SetInteger("AnimMode",0);
                followerObj.GetComponent<AudioSource>().enabled=false;
                followerObj.SetActive(true);
                textLoader.TextSceneSelect(1);
                break;
            case 3://古木田から逃げろ
                followerAnimator.SetInteger("AnimMode",1);
                followerObj.GetComponent<AudioSource>().enabled=true;
                soundPlayer.Play("Run!And!Run!",SoundPlayer.PlaySoundType.BGM);
                helpText.text="フルキダから逃げろ";//上のメッセージ変更
                break;
            case 4://古木田の独り言
                ChangeSpawnPoint(5);
                textLoader.TextSceneSelect(2);
                break;
            case 5://証拠探し
                followerAnimator.SetInteger("AnimMode",1);
                soundPlayer.Play("Run!And!Run!",SoundPlayer.PlaySoundType.BGM);
                helpText.text="どこかに逃げろ";//上のメッセージ変更
                break;
            case 6://地下に入ってジャムを見つける
                soundPlayer.Play("WaterDropping",SoundPlayer.PlaySoundType.WorldBGM);
                textLoader.TextSceneSelect(3);
                break;
            case 7://地下から脱出しよう
                helpText.text="地下を探索しよう";//上のメッセージ変更
                followerObj.SetActive(false);
                break;
            case 8://かぎをみつけた
                ChangeSpawnPoint(9);
                followerObj.SetActive(true);
                followerAnimator.SetInteger("AnimMode",0);
                textLoader.TextSceneSelect(4);
                break;
            case 9://地下から脱出する
                followerAnimator.SetInteger("AnimMode",1);
                soundPlayer.Play("Run!And!Run!",SoundPlayer.PlaySoundType.BGM);
                helpText.text="地下から脱出しろ";
                break;
            case 10://地下から脱出。高揚しているろっこつ..
                soundPlayer.Stop(SoundPlayer.PlaySoundType.BGM);
                soundPlayer.Play("BigRiver",SoundPlayer.PlaySoundType.WorldBGM);
                textLoader.TextSceneSelect(5);
                break;
            case 11://外に出て逃げながらナイフを探す
                helpText.text="他の証拠を探せ";
                followerObj.SetActive(false);
                //Invoke("FollowerObjSetActiveTrue",5f);
                break;
            case 12://ナイフを発見
                //ChangeSpawnPoint(13);
                textLoader.TextSceneSelect(6);
                followerObj.SetActive(true);
                break;
            case 13://川に沈む内蔵を探す
                helpText.text="川を調べるぞ";
                followerObj.SetActive(false);
                break;
            case 14://内臓を見つけ、フルキダに見つかりついにろっこつ豹変
                followerObj.SetActive(true);
                followerAnimator.SetInteger("AnimMode",0);
                textLoader.TextSceneSelect(7);
                break;
            case 15://ナイフで殺すか殺さないか
                followerObj.SetActive(true);
                followerAnimator.SetInteger("AnimMode",3);
                talkingPanel.SetActive(false);
                ShowSelectButtons();
                break;
            case 16://NotKillEnd
                LastSelectPalnel.SetActive(false);
                textLoader.TextSceneSelect(9);
                saveData.trueEnd=true;
                break;
            case 17://killEnd
                if(!saveData.trueEnd){//シーン16を通って無かったら
                    LastSelectPalnel.SetActive(false);
                    textLoader.TextSceneSelect(8);
                    saveData.trueEnd=false;
                }else{
                    NextScene();
                }
                break;
            case 18:
                SaveClass.SaveData<SaveData>("GameData",saveData);
                SceneManager.LoadScene("EndRoll");
                break;
        }
        Debug.Log("シーン"+scene+"の読み込み完了！");
    }

    public void Gameover(){
        Cursor.visible = true;
        starterAssetsInputs.SetCursorState(false);
        gameoverPanel.SetActive(true);
        playerObj.GetComponent<FirstPersonController>().enabled=false;
        followPlayer.CanFollowFalse();
        followerObj.GetComponent<Animator>().enabled=false;
    }
    void ShowSelectButtons(){
        LastSelectPalnel.SetActive(true);
        lastContentText.text="君には慈悲というものは無いのか...?";
    }
    void OnApplicationQuit(){
        SaveClass.SaveData<SaveData>("GameData",saveData);
    }
    void CanFollowTrue(){
        followPlayer.canFollow=true;
    }
    void FirstPersonControllerTrue(){
        playerObj.GetComponent<FirstPersonController>().enabled=true;
    }
    bool ChangeSpawnPoint(int num){
        var temp=spawnPoints.transform.Find("Scene"+num+"Player");
        if(temp!=null){
            temp.position=playerObj.transform.position;
            temp.rotation=playerObj.transform.rotation;
            return true;
        }
        return false;
    }
    void FollowerObjSetActiveTrue(){
        followerObj.SetActive(true);
    }
    public void TriggerNotKill(){
        saveData.sceneNum=16;
        LoadScene(saveData.sceneNum);
    }public void TriggerKill(){
        killCount++;
        playerObj.transform.position+=Vector3.right*oneStepLength;
        switch(killCount){
            case 1:
                lastContentText.text="私は君を救いたい...それが私の本心だ。決して復讐なんかじゃない。";
                break;
            case 2:
                lastContentText.text="君にだって家族がいるんだろう？これ以上、家族を苦しめるんじゃない...";
                break;
            case 3:
                lastContentText.text="お願いだ...君が何を考えているかもう分からない。ただ、命だけは助けてくれ。";
                break;
            case 4:
                lastContentText.text="頼む...殺さないでくれ...";
                break;
            case 5:
                saveData.sceneNum=17;
                LoadScene(saveData.sceneNum);
                break;
        }
    }
    public void LookAtFollower(){
        StartCoroutine(LookAtCoroutine.RotateObject(playerObj,followerObj,0.5f));
    }

    public void LayDownFollower(){
        followerAnimator.SetInteger("AnimMode",2);
    }

    public void ChangeNormalFollower(){
        normalFollowerObj.transform.position=followerObj.transform.position;
        normalFollowerObj.transform.rotation=followerObj.transform.rotation;
        
        followerObj.SetActive(false);
        normalFollowerObj.SetActive(true);
        followerAnimator=normalFollowerObj.GetComponent<Animator>();
        followerAnimator.SetInteger("AnimMode",0);
        followerObj=normalFollowerObj;
    }
    
}
