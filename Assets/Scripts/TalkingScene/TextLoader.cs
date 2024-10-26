using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class TextLoader : MonoBehaviour
{   
    private string[] textMessage; //テキストの加工前の一行を入れる変数
    private List<string[,]> textList;
    private List<int> textSceme;
    private int rowLength;//テキスト内の行数を取得する変数
    private int columnLength;//テキスト内の列数を取得する変数
    private int nowTextScene = 0;
    private int sceneProcess=0;
    private float timer = 0;

    [Tooltip("The time need for loading nexr scene")]
    public float duration = 0.8f;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;
    public GameManager gameManager;
    public SoundPlayer soundPlayer;
    public FunctionPlayer functionPlayer;
    public void TextLoadStart()
    {
        //initialization
        nameText.text = "";
        contentText.text = "";

        TextAsset textasset = new TextAsset(); //テキストファイルのデータを取得するインスタンスを作成
        textasset = Resources.Load("Test", typeof(TextAsset)) as TextAsset; //Resourcesフォルダから対象テキストを取得
        string TextLines = textasset.text; //テキスト全体をstring型で入れる変数を用意して入れる

        //Splitで一行づつを代入した1次配列を作成
        textMessage = TextLines.Split('\n'); //

        //行数と列数を取得
        // Get the row2 since row1 is unexpected...
        columnLength = textMessage[0].Split(',').Length;//,で分けられた数
        rowLength = textMessage.Length;
        Debug.Log("読み込んだテキストの行数は"+rowLength+"でした");

        //2次配列を定義
        textList=new List<string[,]>();
        textSceme = new List<int>();

        //Sceneの区切り目を探し、//を読み込まないようにする
        for (int i=0; i<rowLength; i++){
            var tempText=textMessage[i].Split(',')[0];//i行目の一個目の文字列を取得
            if(tempText=="P"){
                //Debug.Log("Sceme="+i);
                textSceme.Add(i);//i行目でシーンが分かれると追加
            }
        }
    
        int processedRow=0;//txtファイルの何行目を調べてるか
        for(int i=0; i<textSceme.Count; i++){
            int rowVol = (i<textSceme.Count-1 ? textSceme[i+1]-textSceme[i] : rowLength-textSceme[i]);//シーンの会話数（行数を取得）(//を除く)
            //Debug.Log("rowVol="+rowVol);
            var tempTexts = new string[rowVol,columnLength];
            for(int j=0;j<rowVol;j++){//Pより下の行を読み込む
                for(int k=0;k<columnLength;k++){
                    var tempString=textMessage[processedRow].Split(',')[k];
                    tempTexts[j,k]=tempString;
                    if(k==1){//内容をデバックで表示
                        //Debug.Log("Row="+processedRow+"  text="+tempString);
                    }
                }
                processedRow++;
            }
            //processedRow++;//Pの行分
            textList.Add(tempTexts);
        }
        Debug.Log("textList.Count"+textList.Count);
    }

    public void TextSceneSelect(int sceneNum){
        nowTextScene=sceneNum;
        sceneProcess=1;
        TextChange();
    }
    private void Update(){
        timer += Time.deltaTime;
        InputCheck();
    }
    
    private void InputCheck(){
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard.enterKey.wasPressedThisFrame || mouse.leftButton.wasPressedThisFrame || keyboard.ctrlKey.isPressed)//おしっぱにしたら次が読み込まれない
        {
            if (timer >= duration)
            {
                sceneProcess++;
                TextChange();
                timer = 0;
            }
        }
    }

    private void TextChange(){
        if(SceneChangeCheck()){//シーンの最終行に達していなかったら
            SoundPlayCheck();
            nameText.text=textList[nowTextScene][sceneProcess,0];
            contentText.text=textList[nowTextScene][sceneProcess,1];
        }else{
            Debug.Log("シーン"+nowTextScene+"のテキスト終了");
            soundPlayer.Stop(SoundPlayer.PlaySoundType.BGM);
            soundPlayer.Stop(SoundPlayer.PlaySoundType.SE);
            soundPlayer.Stop(SoundPlayer.PlaySoundType.Voice);
            gameManager.NextScene();
        }
    }

    private bool SceneChangeCheck(){//シーンの最終行に達していないか確認
        //Debug.Log("nowTextScene="+nowTextScene+"   sceneProcess="+sceneProcess);
        //Debug.Log("nowRow="+(textSceme[nowTextScene]+sceneProcess));
        if(nowTextScene!=textSceme.Count-1){//テキストファイルの中で最後のシーンでなかったら
            if(textSceme[nowTextScene]+sceneProcess==textSceme[nowTextScene+1]){//今のテキストが次のシナリオの最終行についてるかチェック
                return false;
            }
        }else{//テキストファイルの中で最後のシーンだったら
            if(textSceme[nowTextScene]+sceneProcess==rowLength){//もし最終行に達したら
                return false;
            }
        }
        
        return true;
    }
    private void SoundPlayCheck(){//テキスト読み込み時の再生するか否か
        var tempText=textList[nowTextScene][sceneProcess,0];
        if(tempText=="BGM" || tempText=="SE" || tempText=="Voice"){
            SoundPlayer.PlaySoundType type = (tempText=="BGM" ? SoundPlayer.PlaySoundType.BGM : (tempText=="SE" ? SoundPlayer.PlaySoundType.SE : SoundPlayer.PlaySoundType.Voice));
            bool temp=soundPlayer.Play(textList[nowTextScene][sceneProcess,1],type);
            if(temp){//もし、曲が見つかったら
                NextCheck();
            }   
        }else if(tempText=="ANIM"){

        }else if(tempText=="FUNC"){
            var temp=textList[nowTextScene][sceneProcess,1];
            if(temp=="LookAtFollower"){
                gameManager.LookAtFollower();
                NextCheck();
            }if(temp=="LayDownFollower"){
                gameManager.LayDownFollower();
                NextCheck();
            }if(temp=="ChangeNormalFollower"){
                gameManager.ChangeNormalFollower();
                NextCheck();
            }
            //bool temp=functionPlayer.Play(textList[nowTextScene][sceneProcess,1]);
        }
    }
    void NextCheck(){
        sceneProcess++;
        SoundPlayCheck();
    }
}
