using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HitUI : MonoBehaviour
{
    [SerializeField] Image hitImage;
    Coroutine showCoroutine;


    void Start()
    {
        hitImage.enabled = false;
    }

    public void Show(float duration)
    {
        // If it’s already showing, restart the timer
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
        }

        showCoroutine = StartCoroutine(ShowHitUI(duration));
    }


    private IEnumerator ShowHitUI(float duration)
    {
        hitImage.enabled = true;

        // Set initial color with high alpha
        Color c = hitImage.color;
        c.r = 0.5f;  // red
        c.g = 0f;    // Green
        c.b = 0f;    // Blue
        c.a = 0.1f;  //alpha
        hitImage.color = c;

        float elapsed = 0f;
        float fadeDuration = duration;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.8f, 0f, elapsed / fadeDuration);
            c.a = alpha;
            hitImage.color = c;
            yield return null;
        }

        hitImage.enabled = false;
    }
}
