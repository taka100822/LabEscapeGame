using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void FadeOut(System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(0f, 1f, onComplete));
    }

    public void FadeIn(System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(1f, 0f, onComplete));
    }

    private IEnumerator FadeCoroutine(float from, float to, System.Action onComplete)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, to);
        onComplete?.Invoke();
    }
}
