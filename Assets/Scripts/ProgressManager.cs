using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    private const string LevelKey = "CurrentLevel";

    public static int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt(LevelKey, 1); // Default to level 1 if not set
    }

    public static void SetCurrentLevel(int level)
    {
        PlayerPrefs.SetInt(LevelKey, level);
        PlayerPrefs.Save();
    }

}
