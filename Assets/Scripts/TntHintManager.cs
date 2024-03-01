using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TntHintManager : MonoBehaviour
{

    public PlayManager playManager;
    public LevelManager levelManager;

    public Sprite redTntSprite;
    public Sprite greenTntSprite;
    public Sprite blueTntSprite;
    public Sprite yellowTntSprite;

    public GameObject redCubePrefab;
    public GameObject greenCubePrefab;
    public GameObject blueCubePrefab;
    public GameObject yellowCubePrefab;


    public void ChangeCubeToTNT(GameObject cube)
    {
        Cube cubeScript = cube.GetComponent<Cube>();
        if (!cubeScript.isTnt) // Check if the cube is not already TNT
        {
            cubeScript.isTnt = true; // Mark the cube as TNT
            SpriteRenderer spriteRenderer = cube.GetComponent<SpriteRenderer>();
            switch (cubeScript.cubeType)
            {
                case CubeType.Red:
                    spriteRenderer.sprite = redTntSprite;
                    break;
                case CubeType.Green:
                    spriteRenderer.sprite = greenTntSprite;
                    break;
                case CubeType.Blue:
                    spriteRenderer.sprite = blueTntSprite;
                    break;
                case CubeType.Yellow:
                    spriteRenderer.sprite = yellowTntSprite;
                    break;
            }
        }
    }
    public void ConvertToTntCubesIfNeeded()
    {
        LevelData levelData = levelManager.GetLevelData();
        float cubeWidth = 0.8f;
        float cubeHeight = 0.8f;
        float spacing = 0.8f;
        Vector2 startPosition = new Vector2(
            -(cubeWidth * levelData.grid_width + spacing * (levelData.grid_width - 1)) / 2 + cubeWidth / 2,
            -(cubeHeight * levelData.grid_height + spacing * (levelData.grid_height - 1)) / 2 + cubeHeight / 2 - 3.6f
        );

        for (int x = 0; x < levelData.grid_width; x++)
        {
            for (int y = 0; y < levelData.grid_height; y++)
            {
                Vector3 checkPosition = new Vector3(startPosition.x + x * (cubeWidth + spacing), startPosition.y + y * (cubeHeight + spacing), 0);
                Collider2D collider = Physics2D.OverlapPoint(checkPosition);
                if (collider != null)
                {
                    Cube cubeScript = collider.gameObject.GetComponent<Cube>();
                    if (cubeScript != null)
                    {
                        List<GameObject> connectedCubes = new List<GameObject>();
                        int connectedCount = playManager.CheckAndDestroyConnected(checkPosition, cubeScript.cubeType, connectedCubes);
                        if (connectedCount >= 5)
                        {
                            foreach (var cube in connectedCubes)
                            {
                                ChangeCubeToTNT(cube);
                            }
                        }
                    }
                }
            }
        }
    }

    // This is for the start only, since the cubes get time to go to their positions, so converttotntcubes method cant find their position
    public IEnumerator DelayedTntConversion()
    {
        yield return new WaitForSeconds(0.7f);
        ConvertToTntCubesIfNeeded();
    }

    public void RevertTntCubesIfNeeded()
    {
        LevelData levelData = levelManager.GetLevelData();
        float cubeWidth = 0.8f; // Match this with the actual width of your cubes
        float cubeHeight = 0.8f; // Match this with the actual height of your cubes
        float spacing = 0.8f; // The spacing between cubes, adjust as needed
        Vector2 startPosition = new Vector2(
            -(cubeWidth * levelData.grid_width + spacing * (levelData.grid_width - 1)) / 2 + cubeWidth / 2,
            -(cubeHeight * levelData.grid_height + spacing * (levelData.grid_height - 1)) / 2 + cubeHeight / 2 - 3.6f
        );

        for (int x = 0; x < levelData.grid_width; x++)
        {
            for (int y = 0; y < levelData.grid_height; y++)
            {
                Vector3 checkPosition = new Vector3(startPosition.x + x * (cubeWidth + spacing), startPosition.y + y * (cubeHeight + spacing), 0);
                Collider2D collider = Physics2D.OverlapPoint(checkPosition);
                if (collider != null)
                {
                    Cube cubeScript = collider.gameObject.GetComponent<Cube>();
                    if (cubeScript.isTnt)
                    {
                        List<GameObject> connectedCubes = new List<GameObject>();
                        int connectedCount = playManager.CheckAndDestroyConnected(checkPosition, cubeScript.cubeType, connectedCubes);
                        if (connectedCount < 5)
                        {
                            // Revert the cube back to its normal color sprite
                            cubeScript.isTnt = false;
                            SpriteRenderer spriteRenderer = collider.gameObject.GetComponent<SpriteRenderer>();
                            switch (cubeScript.cubeType)
                            {
                                case CubeType.Red:

                                    spriteRenderer.sprite = redCubePrefab.GetComponent<SpriteRenderer>().sprite;
                                    break;
                                case CubeType.Green:

                                    spriteRenderer.sprite = greenCubePrefab.GetComponent<SpriteRenderer>().sprite;
                                    break;

                                case CubeType.Blue:

                                    spriteRenderer.sprite = blueCubePrefab.GetComponent<SpriteRenderer>().sprite;
                                    break;
                                case CubeType.Yellow:

                                    spriteRenderer.sprite = yellowCubePrefab.GetComponent<SpriteRenderer>().sprite;
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
