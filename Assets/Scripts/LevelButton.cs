using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public Text levelText;

    private void Start()
    {
        UpdateLevelText();
    }

    private void UpdateLevelText()
    {
        int currentLevel = ProgressManager.GetCurrentLevel();
        if (currentLevel > 10)
        {
            levelText.text = "Finished!";
        }
        else
        {
            levelText.text = $"Level {currentLevel}";
        }
    }

    public void LoadLevelScene()
    {
        int currentLevel = ProgressManager.GetCurrentLevel();
        if (currentLevel <= 10)
        {
            SceneManager.LoadScene("LevelScene");
        }
    }
}
