using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoundPlayer : MonoBehaviour
{
    public SoundCollecter soundCollector;
    public enum PlaySoundType{
        SE,
        BGM,
        Voice,
        WorldBGM,
    }
    public AudioSource BGMPLayer;
    public AudioSource SEPLayer;
    public AudioSource VoicePlayer;
    public AudioSource WorldBGMPLayer;
    // Start is called before the first frame update
    public bool Play(string name,PlaySoundType type){
        var temp=soundCollector.GetSound(name);
        if(temp!=null){//もし、リストの中から音源の名前を検索してヒットしたら
            if(type==PlaySoundType.BGM){
                BGMPLayer.clip=temp;
                BGMPLayer.Play(0);
            }else if(type==PlaySoundType.SE){
                SEPLayer.clip=temp;
                SEPLayer.Play(0);
            }else if(type==PlaySoundType.Voice){
                VoicePlayer.clip=temp;
                VoicePlayer.Play(0);
            }else if(type==PlaySoundType.WorldBGM){
                WorldBGMPLayer.clip=temp;
                WorldBGMPLayer.Play(0);
            }
            return true;
        }
        return false;
    }
    public void Stop(PlaySoundType type){
        if(type==PlaySoundType.BGM){
            BGMPLayer.Stop();
        }else if(type==PlaySoundType.SE){
            SEPLayer.Stop();
        }else if(type==PlaySoundType.Voice){
            VoicePlayer.Stop();
        }else if(type==PlaySoundType.WorldBGM){
            WorldBGMPLayer.Stop();
        }
    }
}
