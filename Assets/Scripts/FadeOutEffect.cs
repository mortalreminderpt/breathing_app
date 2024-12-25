using UnityEngine;
using UnityEngine.UI;

public class FadeOutEffect : MonoBehaviour
{
    public Image fadeImage; // Drag the Image component here in the Inspector
    public float fadeDuration = 1.0f; // Duration for the fade

    private float fadeTimer = 0;

    void Update()
    {
        if (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha); // Set new alpha
        }
    }
}