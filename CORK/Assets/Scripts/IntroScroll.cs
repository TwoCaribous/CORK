using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Drives the Star-Wars-style intro crawl.
///
/// Hierarchy expected:
///   Canvas
///     ├─ CrawlViewport  (RectTransform with RectMask2D — the visible window)
///     │    └─ CrawlText (RectTransform — the moving text block)
///     └─ SkipPrompt     (Text shown at the bottom)
///
/// How it works:
///   1. CrawlText starts positioned just below the viewport (off screen).
///   2. Each frame it moves upward at scrollSpeed pixels/sec.
///   3. When the text has fully cleared the top of the viewport, the game scene loads.
///   4. Pressing any key at any time skips directly to the game scene.
///
/// Attach to: any GameObject in the Intro scene (e.g. the Canvas itself).
/// </summary>
public class IntroScroll : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The RectTransform of the text block that will scroll upward.")]
    public RectTransform crawlText;

    [Tooltip("The RectTransform of the viewport panel (has RectMask2D on it).")]
    public RectTransform crawlViewport;

    [Tooltip("Optional 'Press any key to skip' label at the bottom of the screen.")]
    public TextMeshProUGUI skipPrompt;

    [Header("Settings")]
    [Tooltip("How many pixels per second the text scrolls upward.")]
    public float scrollSpeed = 80f;

    [Tooltip("Seconds to wait on a black screen before the crawl begins.")]
    public float openingDelay = 2f;

    [Tooltip("Name of the scene to load when the crawl finishes or is skipped.")]
    public string gameSceneName = "Game";

    bool scrolling = false;
    bool transitioning = false;

    IEnumerator Start()
    {
        // Wait one frame so Unity's Canvas layout pass has completed and
        // rect dimensions (including ContentSizeFitter) are valid.
        yield return null;

        // Position the text just below the bottom edge of the viewport so it
        // scrolls into view naturally from the bottom.
        if (crawlText != null && crawlViewport != null)
        {
            float startY = -(crawlViewport.rect.height / 2f);
            Debug.Log($"[IntroScroll] viewportH={crawlViewport.rect.height:F1}  textH={crawlText.rect.height:F1}  startY={startY:F1}");
            crawlText.anchoredPosition = new Vector2(crawlText.anchoredPosition.x, startY);
        }
        else
        {
            Debug.LogError($"[IntroScroll] Missing reference — crawlText={crawlText}  crawlViewport={crawlViewport}");
        }

        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        if (skipPrompt != null)
            skipPrompt.gameObject.SetActive(false);

        // Brief black screen pause before scroll begins.
        yield return new WaitForSeconds(openingDelay);

        if (skipPrompt != null)
            skipPrompt.gameObject.SetActive(true);

        scrolling = true;
        Debug.Log("[IntroScroll] Scrolling started.");
    }

    void Update()
    {
        if (transitioning) return;

        // Skip on any key or mouse click.
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            LoadGame();
            return;
        }

        if (!scrolling || crawlText == null || crawlViewport == null) return;

        // Move the text upward.
        crawlText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        Debug.Log($"[IntroScroll] pos={crawlText.anchoredPosition.y:F1}");

        // The crawl is done when the bottom of the text block has cleared the
        // top of the viewport (i.e. all text has scrolled past).
        float textBottom = crawlText.anchoredPosition.y - crawlText.rect.height;
        float viewportTop = crawlViewport.rect.height / 2f;

        if (textBottom > viewportTop)
            LoadGame();
    }

    void LoadGame()
    {
        if (transitioning) return;
        transitioning = true;
        scrolling = false;

        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null) audio.Stop();

        Debug.Log($"[IntroScroll] Loading scene: '{gameSceneName}'");
        SceneManager.LoadScene(gameSceneName);
    }
}
