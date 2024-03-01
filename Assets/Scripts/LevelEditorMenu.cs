using UnityEngine;
using UnityEditor;

public class LevelEditorMenu : MonoBehaviour
{

    // We also set the priority, because level 10 was showing up after level 1

    [MenuItem("Levels/Set Current Level to 1", false, 1)]
    private static void SetLevelTo1()
    {
        ProgressManager.SetCurrentLevel(1);
    }

    [MenuItem("Levels/Set Current Level to 2", false, 2)]
    private static void SetLevelTo2()
    {
        ProgressManager.SetCurrentLevel(2);
    }

    [MenuItem("Levels/Set Current Level to 3", false, 3)]
    private static void SetLevelTo3()
    {
        ProgressManager.SetCurrentLevel(3);
    }

    [MenuItem("Levels/Set Current Level to 4", false, 4)]
    private static void SetLevelTo4()
    {
        ProgressManager.SetCurrentLevel(4);
    }

    [MenuItem("Levels/Set Current Level to 5", false, 5)]
    private static void SetLevelTo5()
    {
        ProgressManager.SetCurrentLevel(5);
    }

    [MenuItem("Levels/Set Current Level to 6", false, 6)]
    private static void SetLevelTo6()
    {
        ProgressManager.SetCurrentLevel(6);
    }

    [MenuItem("Levels/Set Current Level to 7", false, 7)]
    private static void SetLevelTo7()
    {
        ProgressManager.SetCurrentLevel(7);
    }

    [MenuItem("Levels/Set Current Level to 8", false, 8)]
    private static void SetLevelTo8()
    {
        ProgressManager.SetCurrentLevel(8);
    }

    [MenuItem("Levels/Set Current Level to 9", false, 9)]
    private static void SetLevelTo9()
    {
        ProgressManager.SetCurrentLevel(9);
    }

    [MenuItem("Levels/Set Current Level to 10", false, 10)]
    private static void SetLevelTo10()
    {
        ProgressManager.SetCurrentLevel(10);
    }
}
