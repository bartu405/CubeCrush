using UnityEngine;

public class PopupAnimation : MonoBehaviour
{
    public float startYPosition = 1200f; // The starting Y position
    public float endYPosition = 0f; // The ending Y position
    public float animationDuration = 1f; // The duration of the animation

    public float animationDelay = 0.5f;

    void Start()
    {
        // Start the popup off-screen
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startYPosition);

        // Animate the popup coming down
        ShowPopup();
    }

    public void ShowPopup()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        LeanTween.moveY(rectTransform, endYPosition, animationDuration)
    .setEase(LeanTweenType.easeOutBack)
    .setDelay(animationDelay);
    }
}
