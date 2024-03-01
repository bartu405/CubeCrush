using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PopUpManager : MonoBehaviour
{

    public GameObject winPopupCanvas;
    public GameObject failPopupCanvas;


    public void TryAgain()
    {
        SceneManager.LoadScene("LevelScene");
    }

    public void ActivateWinPopup()
    {
        Debug.Log("You win!");

        winPopupCanvas.SetActive(true);

        // Invoke the LoadMain method after 5 seconds
        Invoke("LoadMain", 5f);
    }

    public IEnumerator ActivateFailPopup()
    {
        // Wait for a short delay to allow the win check to complete
        yield return new WaitForSeconds(0.5f);

        // Only activate the fail popup if the win condition has not been met
        if (!PlayManager.winConditionMet)
        {
            PlayManager.failConditionMet = true;
            failPopupCanvas.SetActive(true);
        }
    }

    public void LoadMain()
    {

        SceneManager.LoadScene("MainScene");  // Load the MainScene
    }
}
