using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public PlayManager playManager;

    public RectTransform vaseContainer;
    public RectTransform boxContainer;
    public RectTransform stoneContainer;

    public Text remainingBoxesText;
    public Text remainingStonesText;
    public Text remainingVasesText;

    public GameObject boxCounter;
    public GameObject boxCheck;
    public GameObject stoneCounter;
    public GameObject stoneCheck;
    public GameObject vaseCounter;
    public GameObject vaseCheck;


    public Text movesLeftText;
    public void UpdateObstacleUIPositions(List<string> ObstaclesOfTheGame)
    {
        // Define preset positions for 1, 2, and 3 obstacles
        Vector2 positionForOneObstacle = new Vector2(-522, -49);
        Vector2[] positionsForTwoObstacles = { new Vector2(-594, -49), new Vector2(-441, -49) };
        Vector2[] positionsForThreeObstacles = { new Vector2(-441, -49), new Vector2(-576, 25), new Vector2(-576, -113) };


        // Get the list of active obstacle containers
        List<RectTransform> activeObstacleContainers = new List<RectTransform>();
        if (ObstaclesOfTheGame.Contains("Boxes")) activeObstacleContainers.Add(boxContainer);
        else boxContainer.gameObject.SetActive(false);

        if (ObstaclesOfTheGame.Contains("Stones")) activeObstacleContainers.Add(stoneContainer);
        else stoneContainer.gameObject.SetActive(false);

        if (ObstaclesOfTheGame.Contains("Vases")) activeObstacleContainers.Add(vaseContainer);
        else vaseContainer.gameObject.SetActive(false);

        // Update positions based on the number of obstacles
        switch (activeObstacleContainers.Count)
        {
            case 1:
                activeObstacleContainers[0].anchoredPosition = positionForOneObstacle;
                break;
            case 2:
                for (int i = 0; i < activeObstacleContainers.Count; i++)
                {
                    activeObstacleContainers[i].anchoredPosition = positionsForTwoObstacles[i];
                }
                break;
            case 3:
                for (int i = 0; i < activeObstacleContainers.Count; i++)
                {
                    activeObstacleContainers[i].anchoredPosition = positionsForThreeObstacles[i];
                }
                break;
        }
    }


    public void UpdateRemainingBoxesUI(int remainingBoxes)
    {
        if (remainingBoxesText != null)
        {
            remainingBoxesText.text = remainingBoxes.ToString();

        }
        if (remainingBoxes <= 0)
        {
            // All obstacles of this type have been cleared
            boxCounter.SetActive(false);
            boxCheck.SetActive(true);
        }

    }

    public void UpdateRemainingStonesUI(int remainingStones)
    {
        if (remainingStonesText != null)
        {
            remainingStonesText.text = remainingStones.ToString();

        }
        if (remainingStones <= 0)
        {
            // All obstacles of this type have been cleared
            stoneCounter.SetActive(false);
            stoneCheck.SetActive(true);
        }

    }

    public void UpdateRemainingVasesUI(int remainingVases)
    {
        if (remainingVasesText != null)
        {
            remainingVasesText.text = remainingVases.ToString();

        }
        if (remainingVases <= 0)
        {
            // All obstacles of this type have been cleared
            vaseCounter.SetActive(false);
            vaseCheck.SetActive(true);
        }

    }

    // Update the UI text to display the current number of moves left
    public void UpdateMovesLeftUI()
    {
        if (movesLeftText != null && !(playManager.movesLeft < 0))
        {
            movesLeftText.text = "" + playManager.movesLeft;
        }
    }
}

