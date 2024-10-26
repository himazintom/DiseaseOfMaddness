using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    /*[SerializeField]
    private
    int time_duration = 0;*/

    public Image panelImage;

    public AudioSource audioSound;

    private float waitTime = 0;

    const int COLOR_MAX = 255;
    const int INT_MAX = 100;

    private IEnumerator panelFadeOut(float time_duration)
    {

        waitTime = time_duration / COLOR_MAX;

        for (int i = 0; i <= COLOR_MAX; i++)
        {
            yield return new WaitForSeconds(waitTime);
            Debug.Log("Panel: i="+i);
            panelImage.color += new Color32(0, 0, 0, 1);
        }
        //finally, one
        panelImage.color = new Color32(0, 0, 0, 255);
    }

    private  IEnumerator panelFadeIn(float time_duration)
    {
        waitTime = time_duration / COLOR_MAX;

        for (int i = 0; i <= COLOR_MAX; i++)
        {
            yield return new WaitForSeconds(waitTime);

            panelImage.color -= new Color32(0, 0, 0,1);
        }
        //finally, zero
        panelImage.color = new Color32(0, 0, 0, 0);
    }

    public void panelFaderController(string mode,float time_duration)
    {
        if (mode == "IN")
        {
            StartCoroutine(panelFadeIn(time_duration));
        }
        else if (mode == "OUT")
        {
            StartCoroutine(panelFadeOut(time_duration));
        }
    }

    private IEnumerator audioFadeIn(float time_duration)
    {

        waitTime = time_duration / INT_MAX;
        var nowVolume=audioSound.volume;
        float int_max = nowVolume / INT_MAX;
        

        for (int i = 0; i <= INT_MAX; i++)
        {
            yield return new WaitForSeconds(waitTime);
            Debug.Log("Audio: k="+i);
            audioSound.volume +=int_max;
        }
        //finally, one
        audioSound.volume = 1;
    }

    private IEnumerator audioFadeOut(float time_duration)
    {

        waitTime = time_duration / INT_MAX;
        float int_max = 1f / INT_MAX;

        for (int i = 0; i <= INT_MAX; i++)
        {
            yield return new WaitForSeconds(waitTime);
            Debug.Log("Audio: i="+i);
            audioSound.volume -= int_max;
        }
        //finally, one
        audioSound.volume = 0;
    }

    public void audioFaderController(string mode, float time_duration)
    {
        if (mode == "IN")
        {
            StartCoroutine(audioFadeIn(time_duration));
        }
        else if (mode == "OUT")
        {
            StartCoroutine(audioFadeOut(time_duration));
        }
    }

    //for debug and example
    /*public void fadeIn_panel()
    {
        panelFaderController("IN",3);
    }

    public void fadeOut_panel()
    {
        panelFaderController("OUT", 3);
    }*/
}
