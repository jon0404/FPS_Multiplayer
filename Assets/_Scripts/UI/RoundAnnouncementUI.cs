using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundAnnouncementUI : MonoBehaviour
{
    private static RoundAnnouncementUI instance;

    private GameObject announcementPanel;
    private GameObject scoreboardPanel;
    private TMP_Text headlineText;
    private TMP_Text detailText;
    private TMP_Text scoreboardText;
    private Coroutine hideAnnouncement;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateForScene()
    {
        GetOrCreate();
    }

    public static void Present(string headline, string detail, string scoreboard, float duration)
    {
        GetOrCreate().PresentInternal(headline, detail, scoreboard, duration);
    }

    private static RoundAnnouncementUI GetOrCreate()
    {
        if (instance != null)
            return instance;

        var uiObject = new GameObject(nameof(RoundAnnouncementUI));
        instance = uiObject.AddComponent<RoundAnnouncementUI>();
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        BuildCanvas();
    }

    private void BuildCanvas()
    {
        var canvasObject = new GameObject("Round HUD Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        canvasObject.transform.SetParent(transform, false);

        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        announcementPanel = CreatePanel(canvasObject.transform, "Round Announcement", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(720f, 142f), new Vector2(0f, -28f));
        headlineText = CreateText(announcementPanel.transform, "Headline", new Vector2(0.05f, 0.45f), new Vector2(0.95f, 0.94f), 34f, FontStyles.Bold, TextAlignmentOptions.Center);
        detailText = CreateText(announcementPanel.transform, "Detail", new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.43f), 20f, FontStyles.Normal, TextAlignmentOptions.Center);

        scoreboardPanel = CreatePanel(canvasObject.transform, "Round Scores", Vector2.one, Vector2.one, Vector2.one, new Vector2(340f, 260f), new Vector2(-24f, -24f));
        CreateText(scoreboardPanel.transform, "Score Label", new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.96f), 18f, FontStyles.Bold, TextAlignmentOptions.Left).text = "ROUND WINS";
        scoreboardText = CreateText(scoreboardPanel.transform, "Scores", new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.75f), 22f, FontStyles.Normal, TextAlignmentOptions.Left);

        announcementPanel.SetActive(false);
        scoreboardPanel.SetActive(false);
    }

    private void PresentInternal(string headline, string detail, string scoreboard, float duration)
    {
        if (scoreboard != null)
        {
            scoreboardText.text = scoreboard;
            scoreboardPanel.SetActive(!string.IsNullOrWhiteSpace(scoreboard));
        }

        headlineText.text = headline;
        detailText.text = detail;
        announcementPanel.SetActive(!string.IsNullOrWhiteSpace(headline));

        if (hideAnnouncement != null)
            StopCoroutine(hideAnnouncement);

        hideAnnouncement = duration > 0f ? StartCoroutine(HideAnnouncementAfter(duration)) : null;
    }

    private IEnumerator HideAnnouncementAfter(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        announcementPanel.SetActive(false);
        hideAnnouncement = null;
    }

    private GameObject CreatePanel(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 size, Vector2 position)
    {
        var panelObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        var rect = panelObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        var image = panelObject.GetComponent<Image>();
        image.color = new Color(0.035f, 0.055f, 0.065f, 0.94f);
        return panelObject;
    }

    private TMP_Text CreateText(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax, float fontSize, FontStyles style, TextAlignmentOptions alignment)
    {
        var textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        var rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var text = textObject.GetComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = Color.white;
        text.enableAutoSizing = true;
        text.fontSizeMin = 14f;
        text.fontSizeMax = fontSize;
        text.enableWordWrapping = true;
        return text;
    }
}