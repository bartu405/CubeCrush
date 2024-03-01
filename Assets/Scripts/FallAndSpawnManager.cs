using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallAndSpawnManager : MonoBehaviour
{


    public LevelBuilder levelBuilder;
    public TntHintManager tntHintManager;

    public LevelManager levelManager;


    public void HandleFallingAndSpawning()
    {
        StartCoroutine(FallAndSpawnCubes());
    }

    private IEnumerator FallAndSpawnCubes()
    {
        bool cubesMoved;
        LevelData levelData = levelManager.GetLevelData();
        float cubeWidth = 0.8f;
        float cubeHeight = 0.8f;
        float spacing = 0.8f;
        Vector2 startPosition = new Vector2(
                -(cubeWidth * levelData.grid_width + spacing * (levelData.grid_width - 1)) / 2 + cubeWidth / 2,
                -(cubeHeight * levelData.grid_height + spacing * (levelData.grid_height - 1)) / 2 + cubeHeight / 2 - 3.6f
            );

        List<int> emptyColumns = new List<int>(); // Track which columns have empty cells

        do
        {
            cubesMoved = false;
            // Loop through each column
            for (int x = 0; x < levelData.grid_width; x++)
            {
                // Start from the second row and move up
                for (int y = 1; y < levelData.grid_height; y++)
                {
                    Vector3 checkPosition = new Vector3(startPosition.x + x * (cubeWidth + spacing), startPosition.y + (y - 1) * (cubeHeight + spacing), 0);
                    Vector3 abovePosition = new Vector3(startPosition.x + x * (cubeWidth + spacing), startPosition.y + y * (cubeHeight + spacing), 0);

                    Collider2D checkCollider = Physics2D.OverlapPoint(checkPosition);
                    Collider2D aboveCollider = Physics2D.OverlapPoint(abovePosition);

                    // If the current cell is empty but the cell above is not, move the above cube down
                    if (checkCollider == null && aboveCollider != null)
                    {
                        GameObject aboveCube = aboveCollider.gameObject;
                        Cube aboveCubeScript = aboveCube.GetComponent<Cube>();
                        if (aboveCubeScript != null && aboveCubeScript.cubeType != CubeType.Box && aboveCubeScript.cubeType != CubeType.Stone) // Check if the cube is not a Box or Stone, since they don't fall to empty spaces
                        {
                            Vector3 originalPosition = aboveCube.transform.position;
                            float elapsedTime = 0f;
                            float fallDuration = 0.005f; // Reduced duration for faster falling

                            while (elapsedTime < fallDuration)
                            {
                                aboveCube.transform.position = Vector3.Lerp(originalPosition, checkPosition, Mathf.SmoothStep(0, 1, elapsedTime / fallDuration));
                                elapsedTime += Time.deltaTime;
                                yield return null;
                            }

                            aboveCube.transform.position = checkPosition; // Ensure the cube is exactly in the right position
                            cubesMoved = true;

                            // Update the grid data
                            int aboveY = y;
                            int newY = y - 1;
                            levelData.grid[x + aboveY * levelData.grid_width] = "0"; // Set the original position to empty
                            levelData.grid[x + newY * levelData.grid_width] = aboveCubeScript.cubeType.ToString().ToLower(); // Update the new position with the cube type
                        }
                    }
                }
            }

            // Check for empty columns
            emptyColumns.Clear();
            for (int x = 0; x < levelData.grid_width; x++)
            {
                Vector3 topPosition = new Vector3(startPosition.x + x * (cubeWidth + spacing), startPosition.y + (levelData.grid_height - 1) * (cubeHeight + spacing), 0);
                Collider2D collider = Physics2D.OverlapPoint(topPosition);
                if (collider == null) // Check if the top cell is empty
                {
                    emptyColumns.Add(x);
                }
            }

            // Spawn new random cubes at the top of each empty column
            foreach (int x in emptyColumns)
            {
                Vector3 spawnPosition = new Vector3(startPosition.x + x * (cubeWidth + spacing), startPosition.y + (levelData.grid_height - 1) * (cubeHeight + spacing), 0);
                GameObject newCube = Instantiate(levelBuilder.GetRandomCubePrefab(), spawnPosition, Quaternion.identity);
                // Update the grid data
                levelData.grid[x + (levelData.grid_height - 1) * levelData.grid_width] = newCube.GetComponent<Cube>().cubeType.ToString().ToLower();
            }

        } while (cubesMoved);


        // Spawn new random cubes at the top of each column
        for (int x = 0; x < levelData.grid_width; x++)
        {
            Vector3 spawnPosition = new Vector3(startPosition.x + x * (cubeWidth + spacing), startPosition.y + (levelData.grid_height - 1) * (cubeHeight + spacing), 0);
            Collider2D collider = Physics2D.OverlapPoint(spawnPosition);
            if (collider == null) // Check if the top cell is empty
            {
                Instantiate(levelBuilder.GetRandomCubePrefab(), spawnPosition, Quaternion.identity);
            }
        }



        tntHintManager.ConvertToTntCubesIfNeeded();
        tntHintManager.RevertTntCubesIfNeeded();


        yield return null;
    }
}
