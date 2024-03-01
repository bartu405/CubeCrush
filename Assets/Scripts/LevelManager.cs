using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public PlayManager playManager; // Assign this in the Inspector with the GameObject that has the LevelLoader script

    public TextAsset level1Json;
    public TextAsset level2Json;
    public TextAsset level3Json;
    public TextAsset level4Json;
    public TextAsset level5Json;
    public TextAsset level6Json;
    public TextAsset level7Json;
    public TextAsset level8Json;
    public TextAsset level9Json;
    public TextAsset level10Json;



    public UIManager uiManager;
    public LevelBuilder levelBuilder;
    public TntHintManager tntHintManager;

    void Start()
    {
        if (playManager == null)
        {
            playManager = GetComponent<PlayManager>(); // Fallback in case it's not assigned.
        }

        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        LevelData levelData = GetLevelData();
        if (levelData != null)
        {
            levelBuilder.BuildLevel(levelData);
            playManager.movesLeft = levelData.move_count;
            uiManager.UpdateMovesLeftUI();
            StartCoroutine(tntHintManager.DelayedTntConversion());
        }
        else
        {
            Debug.LogError("Failed to load level data.");
        }
    }


    private TextAsset GetLevelJson(int level)
    {
        switch (level)
        {
            case 1: return level1Json;
            case 2: return level2Json;
            case 3: return level3Json;
            case 4: return level4Json;
            case 5: return level5Json;
            case 6: return level6Json;
            case 7: return level7Json;
            case 8: return level8Json;
            case 9: return level9Json;
            case 10: return level10Json;
            default:
                Debug.LogError("Invalid level number");
                return null;
        }
    }

    public LevelData GetLevelData()
    {
        int currentLevel = ProgressManager.GetCurrentLevel();
        TextAsset levelJson = GetLevelJson(currentLevel);
        if (levelJson != null)
        {
            return JsonUtility.FromJson<LevelData>(levelJson.text);
        }
        else
        {
            Debug.LogError($"Level JSON data not assigned for level: {currentLevel}");
            return null;
        }
    }
}

