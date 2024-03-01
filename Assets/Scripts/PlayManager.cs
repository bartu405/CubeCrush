using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayManager : MonoBehaviour
{

    public int movesLeft;

    public Sprite brokenVaseSprite;
    public GameObject vaseParticlePrefab_2;

    public static bool winConditionMet = false;
    public static bool failConditionMet = false;

    public GameObject tntPrefab;

    public LevelBuilder levelBuilder;
    public TntManager tntManager;

    public UIManager uiManager;

    public TntHintManager tntHintManager;

    public PopUpManager popUpManager;

    public FallAndSpawnManager fallAndSpawnManager;

    private bool levelCompleted = false;
    public void CompleteLevel()
    {
        if (!levelCompleted)
        {
            levelCompleted = true;
            int currentLevel = ProgressManager.GetCurrentLevel();
            Debug.Log($"Completing level: {currentLevel}");
            if (currentLevel <= 10)
            {
                ProgressManager.SetCurrentLevel(currentLevel + 1);
            }
            else
            {
                Debug.Log("All levels completed!");
            }
        }
    }


    private void CheckForWin()
    {
        if (winConditionMet)
        {
            return; // Exit the method if the player has already won
        }

        bool allCleared = true;
        foreach (string obstacle in levelBuilder.ObstaclesOfTheGame)
        {
            if (obstacle == "Boxes" && levelBuilder.RemainingBoxes > 0)
            {
                allCleared = false;
                break;
            }
            else if (obstacle == "Stones" && levelBuilder.RemainingStones > 0)
            {
                allCleared = false;
                break;
            }
            else if (obstacle == "Vases" && levelBuilder.RemainingVases > 0)
            {
                allCleared = false;
                break;
            }
        }

        if (allCleared)
        {

            winConditionMet = true;
            CompleteLevel();
            popUpManager.ActivateWinPopup();

        }
    }


    public IEnumerator SmoothDestroy(GameObject cube)
    {
        Cube cubeScript = cube.GetComponent<Cube>();
        bool shouldUpdateCounts = cubeScript != null && (cubeScript.cubeType == CubeType.Box || cubeScript.cubeType == CubeType.Vase || cubeScript.cubeType == CubeType.Stone);

        float duration = 0.1f;
        float elapsed = 0f;
        Vector3 originalScale = cube.transform.localScale;

        while (elapsed < duration)
        {
            float scale = Mathf.Lerp(1f, 0f, elapsed / duration);

            if (cube == null) yield break;

            cube.transform.localScale = originalScale * scale;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Instantiate the particle system for cubes with a slight offset in the Z-index, since they sometimes appear behind the grid
        Vector3 particlePosition = new Vector3(cube.transform.position.x, cube.transform.position.y, cube.transform.position.z - 5f); // Adjust the Z value as needed
        ParticleManager particleManager = FindObjectOfType<ParticleManager>();
        particleManager.InstantiateParticle(cubeScript.cubeType, particlePosition);

        Destroy(cube);

        // Wait 1 frame, ensure that cubes destruction completed
        yield return null;

        // Update the obstacle count if you destroyed any
        if (shouldUpdateCounts)
        {
            levelBuilder.UpdateObstacleCounts();
            CheckForWin();
        }

        if (!winConditionMet)
        {
            fallAndSpawnManager.HandleFallingAndSpawning();
            tntHintManager.ConvertToTntCubesIfNeeded();
        }
    }


    public void DecreaseMovesLeft()
    {
        movesLeft--;
        uiManager.UpdateMovesLeftUI();
        CheckForWin();
        if (movesLeft <= 0)
        {
            // First check for a win before showing the fail popup
            CheckForWin();

            // Only show the fail popup if the win condition is not met
            if (!winConditionMet)
            {
                StartCoroutine(popUpManager.ActivateFailPopup());
            }
        }
    }

    void Update()
    {

        // This check prevents you to blast cubes after win or fail popup shows up
        if (!winConditionMet && !failConditionMet)
        {
            // Check for mouse click
            if (Input.GetMouseButtonDown(0))
            {
                // Cast a ray from the camera to the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                if (hit.collider != null)
                {
                    GameObject clickedCube = hit.collider.gameObject;
                    Cube cubeScript = clickedCube.GetComponent<Cube>();

                    // Check if the clicked cube is not a Box, Vase, or Stone since they won't blast
                    if (cubeScript != null && cubeScript.cubeType != CubeType.Box && cubeScript.cubeType != CubeType.Vase && cubeScript.cubeType != CubeType.Stone)
                    {
                        // Get the position of the clicked cube
                        Vector3 clickedPosition = clickedCube.transform.position;

                        // Handle TNT explosion
                        if (cubeScript.cubeType == CubeType.Tnt)
                        {
                            // Check to avoid tnt exploding itself multiple times
                            cubeScript.isExploded = true;

                            // Trigger TNT explosion
                            tntManager.ExplodeTNT(clickedPosition);
                            StartCoroutine(SmoothDestroy(clickedCube)); // Destroy the TNT cube
                            DecreaseMovesLeft();
                        }
                        else
                        {
                            // Check for connected cubes of the same type
                            List<GameObject> connectedCubes = new List<GameObject>();
                            CheckAndDestroyConnected(clickedPosition, cubeScript.cubeType, connectedCubes);

                            // If 5 or more cubes are connected, instantiate a TNT prefab at the clicked cube's position
                            if (connectedCubes.Count >= 5)
                            {

                                Instantiate(tntPrefab, clickedPosition, Quaternion.identity);
                                Destroy(clickedCube); // Destroy the clicked cube
                            }

                            // If at least 2 cubes are connected, destroy them and damage adjacent obstacles
                            if (connectedCubes.Count > 1)
                            {
                                List<GameObject> damagedVases = new List<GameObject>();
                                foreach (var cube in connectedCubes)
                                {

                                    StartCoroutine(SmoothDestroy(cube));
                                    DamageAdjacentObstacles(cube.transform.position, damagedVases); // Pass the list as an argument
                                }
                                DecreaseMovesLeft();
                            }
                        }
                    }
                }
            }
        }
    }
    private void DamageAdjacentObstacles(Vector3 position, List<GameObject> damagedVases)
    {
        Vector3[] adjacentPositions = {
        position + Vector3.up,
        position + Vector3.down,
        position + Vector3.left,
        position + Vector3.right
    };

        foreach (Vector3 adjacentPosition in adjacentPositions)
        {
            Collider2D collider = Physics2D.OverlapPoint(adjacentPosition);
            if (collider != null)
            {
                Cube cube = collider.gameObject.GetComponent<Cube>();
                if (cube != null && (cube.cubeType == CubeType.Box || (cube.cubeType == CubeType.Vase && !damagedVases.Contains(collider.gameObject))))
                {
                    cube.damage++;
                    if (cube.cubeType == CubeType.Vase)
                    {
                        damagedVases.Add(collider.gameObject); // Add the Vase to the list of damaged Vases to avoid it being damaged twice by one blast
                    }
                    if (cube.cubeType == CubeType.Vase && cube.damage == 1)
                    {
                        Instantiate(vaseParticlePrefab_2, collider.transform.position, Quaternion.identity);

                        // Change the sprite to the broken vase sprite
                        SpriteRenderer spriteRenderer = collider.gameObject.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null && brokenVaseSprite != null)
                        {
                            spriteRenderer.sprite = brokenVaseSprite;
                        }
                    }
                    else if (cube.damage >= cube.maxDamage)
                    {
                        StartCoroutine(SmoothDestroy(collider.gameObject));
                        levelBuilder.UpdateObstacleCounts();


                    }
                }
            }
        }
    }

    public int CheckAndDestroyConnected(Vector3 position, CubeType clickedCubeType, List<GameObject> connectedCubes)
    {
        int count = 0;
        Collider2D neighborCollider = Physics2D.OverlapPoint(position);

        if (neighborCollider != null)
        {
            Cube neighborCube = neighborCollider.gameObject.GetComponent<Cube>();
            if (neighborCube != null && neighborCube.cubeType == clickedCubeType && !connectedCubes.Contains(neighborCollider.gameObject))
            {
                connectedCubes.Add(neighborCollider.gameObject);
                count = 1; // This cube is connected, so start count at 1


                // Increment the count by the number of connected cubes in each direction
                count += CheckAndDestroyConnected(neighborCube.transform.position + Vector3.up, clickedCubeType, connectedCubes);
                count += CheckAndDestroyConnected(neighborCube.transform.position + Vector3.down, clickedCubeType, connectedCubes);
                count += CheckAndDestroyConnected(neighborCube.transform.position + Vector3.left, clickedCubeType, connectedCubes);
                count += CheckAndDestroyConnected(neighborCube.transform.position + Vector3.right, clickedCubeType, connectedCubes);
            }
        }

        return count; // Return the total count of connected cubes
    }
}
