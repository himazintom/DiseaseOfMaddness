using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonController : MonoBehaviour
{
    [SerializeField]
    private
    AudioClip sound_button_start;
    public FadeController fadeController;

    public GameObject fadePanel;

    /*[SerializeField]
    private
    AudioClip sound_button_setting;*/

    // Start is called before the first frame update
    void Start()
    {
        fadePanel.SetActive(false);
    }

    public void OnClickStart() {
        fadePanel.SetActive(true);
        SceneManager.LoadScene("TestScene");
    }
    public void OnClickSetting()
    {
        SceneManager.LoadScene("Settings");
    }

    public void OnClickSeisakusya()
    {
        Application.OpenURL("https://manajishi.com/disease_of_madness/manajishi_and_himazi_madness.html");
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("Title");
    }

    /*private void FadeAndNextScene(float second = 3f, System.Action<float> callbackMethod = null) {
    fadeController.panelFaderController("OUT", second);
    fadeController.audioFaderController("OUT", second);
    callbackMethod?.Invoke(second);*/
    }
