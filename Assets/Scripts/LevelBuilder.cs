using System.Collections.Generic;
using UnityEngine;


public class LevelBuilder : MonoBehaviour
{

    public UIManager uiManager;
    public TntHintManager tntHintManager;


    private int remainingBoxes;
    private int remainingStones;
    private int remainingVases;

    public int RemainingBoxes => remainingBoxes;
    public int RemainingStones => remainingStones;
    public int RemainingVases => remainingVases;

    public RectTransform gridBackground;

    public GameObject redCubePrefab;
    public GameObject blueCubePrefab;
    public GameObject yellowCubePrefab;
    public GameObject greenCubePrefab;

    public GameObject tntPrefab;


    public GameObject vasePrefab;
    public GameObject boxPrefab;
    public GameObject stonePrefab;

    public List<string> ObstaclesOfTheGame = new List<string>();

    public IReadOnlyList<string> obstaclesOfTheGame
    {
        get { return ObstaclesOfTheGame.AsReadOnly(); }
    }

    public static LevelBuilder Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BuildLevel(LevelData data)
    {
        PlayManager.winConditionMet = false;
        PlayManager.failConditionMet = false;

        float cubeWidth = 0.8f;
        float cubeHeight = 0.8f;
        float spacing = 0.8f;

        // Calculate the start position of the grid
        Vector2 startPosition = new Vector2(
            -(cubeWidth * data.grid_width + spacing * (data.grid_width - 1)) / 2 + cubeWidth / 2,
            -(cubeHeight * data.grid_height + spacing * (data.grid_height - 1)) / 2 + cubeHeight / 2 - 3.6f
        );

        float backgroundWidth = 98 * data.grid_width;
        float backgroundHeight = 100 * data.grid_height;
        gridBackground.sizeDelta = new Vector2(backgroundWidth, backgroundHeight);

        // Position the grid off-screen to the left
        Vector3 gridStartPosition = new Vector3(-backgroundWidth, startPosition.y, 0);
        Vector3 gridEndPosition = new Vector3(startPosition.x, startPosition.y, 0);

        // Start position at x = -1500, keeping the current y position
        Vector2 gridAnimationStartPositions = new Vector2(-8000, gridBackground.anchoredPosition.y);
        gridBackground.anchoredPosition = gridStartPosition;

        Vector2 gridAnimationEndPositions = new Vector2(0, -322);

        // Tween the anchoredPosition from gridStartPosition to gridEndPosition
        LeanTween.value(gridBackground.gameObject, (Vector2 val) =>
        {
            gridBackground.anchoredPosition = val;
        }, gridAnimationStartPositions, gridAnimationEndPositions, 0.55f)
        .setEase(LeanTweenType.easeInOutQuad);

        for (int i = 0; i < data.grid.Length; i++)
        {
            GameObject prefab = GetPrefab(data.grid[i]);
            if (prefab != null)
            {
                int x = i % data.grid_width;
                int y = i / data.grid_width;

                Vector3 position = new Vector3(
                    gridStartPosition.x + x * (cubeWidth + spacing),
                    gridStartPosition.y + y * (cubeHeight + spacing),
                    0
                );

                GameObject cubeObject = Instantiate(prefab, position, Quaternion.identity);

                // Define your custom starting position
                Vector3 cubeStartPosition = new Vector3(
                    -100,
                    gridEndPosition.y + y * (cubeHeight + spacing),
                    0
                );

                // Set the cube's initial position to the starting position
                cubeObject.transform.position = cubeStartPosition;

                // Define the final position based on gridEndPosition, cubeWidth, spacing, etc.
                Vector3 cubeFinalPosition = new Vector3(
                    gridEndPosition.x + x * (cubeWidth + spacing),
                    gridEndPosition.y + y * (cubeHeight + spacing),
                    0
                );

                // Animate the cube from the starting position to the final position
                LeanTween.move(cubeObject, cubeFinalPosition, 0.55f).setEase(LeanTweenType.easeInOutQuad);

                // Set maxDamage for Box obstacles
                Cube cubeComponent = cubeObject.GetComponent<Cube>();

                // Set maxDamage for Vase obstacles
                if (cubeComponent != null && cubeComponent.cubeType == CubeType.Vase)
                {
                    remainingVases++;
                    uiManager.UpdateRemainingVasesUI(remainingVases);
                }
                if (cubeComponent != null && cubeComponent.cubeType == CubeType.Box)
                {
                    remainingBoxes++;
                    uiManager.UpdateRemainingBoxesUI(remainingBoxes);
                }
                if (cubeComponent != null && cubeComponent.cubeType == CubeType.Stone)
                {
                    remainingStones++;
                    uiManager.UpdateRemainingStonesUI(remainingStones);
                }
            }
        }

        // Update obstacle list
        CheckObstacles();
        uiManager.UpdateObstacleUIPositions(ObstaclesOfTheGame);
        StartCoroutine(tntHintManager.DelayedTntConversion());

    }

    private void CheckObstacles()
    {
        // Initialize obstacles list
        ObstaclesOfTheGame.Clear();

        if (remainingBoxes > 0) ObstaclesOfTheGame.Add("Boxes");
        if (remainingStones > 0) ObstaclesOfTheGame.Add("Stones");
        if (remainingVases > 0) ObstaclesOfTheGame.Add("Vases");
    }

    public void UpdateObstacleCounts()
    {
        // Reset the counts
        remainingBoxes = 0;
        remainingStones = 0;
        remainingVases = 0;

        // Find all Cube objects in the scene
        Cube[] cubes = FindObjectsOfType<Cube>();

        // Count the number of Boxes, Stones, and Vases
        foreach (Cube cube in cubes)
        {
            switch (cube.cubeType)
            {
                case CubeType.Box:
                    remainingBoxes++;
                    break;
                case CubeType.Stone:
                    remainingStones++;
                    break;
                case CubeType.Vase:
                    remainingVases++;
                    break;
            }
        }

        // Update the UI for each obstacle
        uiManager.UpdateRemainingBoxesUI(remainingBoxes);
        uiManager.UpdateRemainingStonesUI(remainingStones);
        uiManager.UpdateRemainingVasesUI(remainingVases);


    }

    public GameObject GetPrefab(string type)
    {
        switch (type)
        {
            case "r": return redCubePrefab;
            case "g": return greenCubePrefab;
            case "b": return blueCubePrefab;
            case "y": return yellowCubePrefab;
            case "bo": return boxPrefab;
            case "t": return tntPrefab;
            case "s": return stonePrefab;
            case "v": return vasePrefab;
            case "rand": return GetRandomCubePrefab();
            default:
                Debug.LogWarning("Unrecognized type: " + type);
                return null;
        }
    }

    public GameObject GetRandomCubePrefab()
    {
        // List of available cube prefabs
        GameObject[] cubePrefabs = { redCubePrefab, greenCubePrefab, blueCubePrefab, yellowCubePrefab };

        // Return a random cube prefab
        return cubePrefabs[Random.Range(0, cubePrefabs.Length)];
    }
}