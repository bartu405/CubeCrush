using UnityEngine;

public class TopUIAnimation : MonoBehaviour
{
    public float startYPosition = 3000; // The starting Y position
    public float endYPosition = 872; // The ending Y position
    public float animationDuration = 0.8f; // The duration of the animation

    public float animationDelay = 0f;



    void Start()
    {
        // Start the popup off-screen
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startYPosition);

        // Animate the popup coming down
        ShowUI();
    }

    public void ShowUI()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        LeanTween.moveY(rectTransform, endYPosition, animationDuration)
    .setEase(LeanTweenType.easeOutQuint)
    .setDelay(animationDelay);
    }
}
