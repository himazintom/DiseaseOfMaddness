using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DictionalyAddon;

public class SoundCollecter : MonoBehaviour
{
    public delegate void Function();
    [Serializable]
	public class StringAudioClipKeyValuePair : SerializableKeyValuePair<string, AudioClip> { }
	
	[Serializable]
	public class StringAudioClipDictionary : SerializableDictionary<string, AudioClip, StringAudioClipKeyValuePair> { }

    [Serializable]
	public class StringAnimationKeyValuePair : SerializableKeyValuePair<String, Animation> { }
	
	[Serializable]
    public class StringAnimationDictionary : SerializableDictionary<string, Animation, StringAnimationKeyValuePair> { }
    
    [Serializable]
	public class StringFunctionKeyValuePair : SerializableKeyValuePair<String, System.Action> { }
	
	[Serializable]
    public class StringFunctionDictionary : SerializableDictionary<string, System.Action, StringFunctionKeyValuePair> { }
    
    public StringAudioClipDictionary SoundList;
    public StringAnimationDictionary AnimationList;
    //public Dictionary<string, Function> FunctionList= new Dictionary<string, Function>();


    public AudioClip GetSound(string name){
        if(SoundList[name]){
            return SoundList[name];
        }
        return null;
    }
    public Animation GetAnimation(string name){
        if(AnimationList[name]){
            return AnimationList[name];
        }
        return null;
    }
    // public Function GetFunction(string name){
    //     if(FunctionList[name]!=null){
    //         return FunctionList[name];
    //     }
    //     return null;
    // }

    // public System.Action GetFunctionsssss(string name){
    //     if(fdire[name]!=null){
    //         return fdire[name];
    //     }
    //     return null;
    // }
}
