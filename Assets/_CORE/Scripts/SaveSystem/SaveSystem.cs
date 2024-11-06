using System;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    public string path;

    [Space()]
    public SaveDataFields DataFields;

    string jsonInString = "";

    const string FirstPlayTimeKey = "FirstPlayTimeFORDR";

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        try
        {
            path = Path.Combine(Application.persistentDataPath + "/SaveData.json");

            if (File.Exists(path))
            {
                LoadJSON();
            }

            else
            {
                CreateJSON();
            }

            if (PlayerPrefs.GetInt("FIRST_PLAY", 0) == 0)
            {
                PlayerPrefs.SetInt("FIRST_PLAY", 1);

                PlayerPrefs.SetString(FirstPlayTimeKey, DateTime.Now.ToString());
            }
        }

        catch
        {

        }
    }

    #region JSON

    public void CreateJSON()
    {
        try
        {
            Debug.Log("CREATING JSON......");

            jsonInString = JsonUtility.ToJson(DataFields, true);

            File.WriteAllText(path, jsonInString);
        }

        catch
        {

        }

        if (File.Exists(path))
        {
            LoadJSON();
        }
    }

    public void LoadJSON()
    {
        try
        {
            Debug.Log("LOADING JSON......");

            string loadedJsonDataString = File.ReadAllText(path);

            JsonUtility.FromJsonOverwrite(loadedJsonDataString, DataFields);
        }

        catch
        {

        }
    }

    public void SaveJSON()
    {
        try
        {
            Debug.Log("SAVING JSON......");

            jsonInString = JsonUtility.ToJson(DataFields, true);

            File.WriteAllText(path, jsonInString);
        }

        catch
        {

        }
    }

    #endregion

    void OnApplicationQuit()
    {
        try
        {
            SaveJSON();
        }
        catch { }
    }

    public void OnApplicationPause()
    {
        try
        {
            SaveJSON();
        }
        catch { }
    }
}

[Serializable]
public class SaveDataFields
{
    [Space()]
    [Header("--------------------  Level Data  --------------------")]
    public int currentLevel = 1;
    public int levelNumber = 1;
    public int totalLevels = 5;
}