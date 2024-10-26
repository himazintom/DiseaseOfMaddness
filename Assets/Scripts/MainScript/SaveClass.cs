using UnityEngine;
using System.IO;

public static class SaveClass
{
    static string SavePath(string path)
        => $"{Application.persistentDataPath}/JsonSaveData/{path}";

    public static void SaveData<T>(string path, T data)
    {
        using (StreamWriter sw = new StreamWriter(SavePath(path)+".json", false))
        {
            Debug.Log("セーブしマスタ");
            string jsonstr = JsonUtility.ToJson(data, true);
            sw.Write(jsonstr);
            sw.Flush();
        }
    }

    public static T LoadData<T>(string path)
    {
        if (File.Exists(SavePath(path)+".json"))//データが存在する場合は返す
        {
            using (StreamReader sr = new StreamReader(SavePath(path)+".json"))
            {
                string datastr = sr.ReadToEnd();
                return JsonUtility.FromJson<T>(datastr);
            }
        }else{
            Directory.CreateDirectory($"{Application.persistentDataPath}/JsonSaveData");
            Debug.Log("新規作成");
            return default;
        }
        //存在しない場合はdefaultを返却
        
    }
}
