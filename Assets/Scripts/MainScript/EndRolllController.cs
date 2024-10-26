using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndRollController : MonoBehaviour
{
    [SerializeField]
    private
    RectTransform credits_text;

    [SerializeField]
    private
    RectTransform endPoint;

    [SerializeField] private
    float magnitude = 1.0f;//move up per second
    SaveData saveData;

    private bool yet_end = true;
    public AudioSource audioSource;
    public AudioClip TrueEndBgm;
    public AudioClip BadEndBgm;

    // Start is called before the first frame update
    void Start()
    {   
        saveData=new SaveData();
        saveData=SaveClass.LoadData<SaveData>("GameData");
        if(saveData.trueEnd){
            audioSource.clip=BadEndBgm;
        }else{
            audioSource.clip=TrueEndBgm;
        }
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (yet_end)
        {
            if (credits_text.localPosition.y > endPoint.localPosition.y)
            {
                StartCoroutine("backtitle");
                yet_end = false;
            }
            else
            {
                credits_text.localPosition += Vector3.up * magnitude*Time.deltaTime;
            }
        }
        
    }

    IEnumerator backtitle()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Title");
    }
}
