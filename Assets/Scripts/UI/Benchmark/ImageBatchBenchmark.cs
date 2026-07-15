using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cyber
{
    public class ImageBatchBenchmark : MonoBehaviour
    {
        private const string CanvasName = "BenchmarkCanvas";
        private const string TitleText = "Image 1000 Batch Benchmark";
        private const string RunningText = "测试中，请等待结果...";
        private const string BuiltinFontPath = "LegacyRuntime.ttf";

        [Header("Benchmark Setup")]
        [SerializeField] private int imageCount = 1000;
        [SerializeField] private int columnCount = 40;
        [SerializeField] private int sampleCount = 8;
        [SerializeField] private Vector2 cellSize = new Vector2(16f, 16f);
        [SerializeField] private Vector2 spacing = new Vector2(4f, 4f);

        private readonly List<Image> benchmarkImages = new List<Image>();

        private Font defaultFont;
        private Button enabledBenchmarkButton;
        private Button alphaBenchmarkButton;
        private Button setActiveBenchmarkButton;
        private Text descriptionText;
        private Text enabledResultText;
        private Text alphaResultText;
        private Text setActiveResultText;
        private bool isRunning;

        private void Start()
        {
            defaultFont = Resources.GetBuiltinResource<Font>(BuiltinFontPath);
            BuildRuntimeUi();
            UpdateDescription("点击按钮后会执行多轮采样，并显示 1000 个 Image / GameObject 同批次修改的耗时。");
            enabledResultText.text = "Enabled 基准：未执行";
            alphaResultText.text = "Alpha 基准：未执行";
            setActiveResultText.text = "SetActive 基准：未执行";
        }

        private void BuildRuntimeUi()
        {
            var canvas = GetOrCreateCanvas();
            CreateOrGetEventSystem();

            ClearGeneratedChildren(canvas.transform);
            benchmarkImages.Clear();

            var title = CreateText("Title", canvas.transform, TitleText, 34, FontStyle.Bold, TextAnchor.UpperLeft);
            SetRect(title.rectTransform, new Vector2(24f, -24f), new Vector2(900f, 44f), TextAnchor.UpperLeft);

            descriptionText = CreateText("Description", canvas.transform, string.Empty, 20, FontStyle.Normal, TextAnchor.UpperLeft);
            SetRect(descriptionText.rectTransform, new Vector2(24f, -76f), new Vector2(1200f, 60f), TextAnchor.UpperLeft);

            enabledBenchmarkButton = CreateButton(
                "BtnEnabledBenchmark",
                canvas.transform,
                "测试 1000 Image 切换 enabled",
                new Vector2(24f, -146f),
                new Vector2(320f, 52f));
            enabledBenchmarkButton.onClick.AddListener(() => StartCoroutine(RunBenchmark(BenchmarkMode.Enabled)));

            alphaBenchmarkButton = CreateButton(
                "BtnAlphaBenchmark",
                canvas.transform,
                "测试 1000 Image Color.a = 0",
                new Vector2(364f, -146f),
                new Vector2(320f, 52f));
            alphaBenchmarkButton.onClick.AddListener(() => StartCoroutine(RunBenchmark(BenchmarkMode.AlphaZero)));

            setActiveBenchmarkButton = CreateButton(
                "BtnSetActiveBenchmark",
                canvas.transform,
                "测试 1000 GameObject.SetActive(false)",
                new Vector2(704f, -146f),
                new Vector2(360f, 52f));
            setActiveBenchmarkButton.onClick.AddListener(() => StartCoroutine(RunBenchmark(BenchmarkMode.SetActiveFalse)));

            enabledResultText = CreateText("EnabledResult", canvas.transform, string.Empty, 20, FontStyle.Normal, TextAnchor.UpperLeft);
            SetRect(enabledResultText.rectTransform, new Vector2(24f, -210f), new Vector2(1400f, 70f), TextAnchor.UpperLeft);

            alphaResultText = CreateText("AlphaResult", canvas.transform, string.Empty, 20, FontStyle.Normal, TextAnchor.UpperLeft);
            SetRect(alphaResultText.rectTransform, new Vector2(24f, -286f), new Vector2(1400f, 70f), TextAnchor.UpperLeft);

            setActiveResultText = CreateText("SetActiveResult", canvas.transform, string.Empty, 20, FontStyle.Normal, TextAnchor.UpperLeft);
            SetRect(setActiveResultText.rectTransform, new Vector2(24f, -362f), new Vector2(1400f, 70f), TextAnchor.UpperLeft);

            var imagePanel = CreatePanel("ImagePanel", canvas.transform, new Color(0.11f, 0.11f, 0.11f, 0.92f));
            SetRect(imagePanel.rectTransform, new Vector2(24f, -456f), new Vector2(880f, 560f), TextAnchor.UpperLeft);

            CreateBenchmarkImages(imagePanel.transform);
        }

        private Canvas GetOrCreateCanvas()
        {
            var canvas = GameObject.Find(CanvasName)?.GetComponent<Canvas>();
            if (canvas == null)
            {
                var canvasObject = new GameObject(CanvasName, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = canvasObject.GetComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            return canvas;
        }

        private void CreateOrGetEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
            var inputSystemModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModuleType != null)
            {
                eventSystemObject.AddComponent(inputSystemModuleType);
            }
            else
            {
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }
        }

        private void ClearGeneratedChildren(Transform parent)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        private void CreateBenchmarkImages(Transform parent)
        {
            var rows = Mathf.CeilToInt(imageCount / (float)columnCount);
            var panelRect = parent as RectTransform;
            var panelWidth = panelRect.rect.width;
            var totalWidth = columnCount * cellSize.x + (columnCount - 1) * spacing.x;
            var totalHeight = rows * cellSize.y + (rows - 1) * spacing.y;
            var startX = (panelWidth - totalWidth) * 0.5f;
            var startY = -32f;

            for (var i = 0; i < imageCount; i++)
            {
                var imageObject = new GameObject($"BenchmarkImage_{i:0000}", typeof(RectTransform), typeof(Image));
                imageObject.transform.SetParent(parent, false);

                var image = imageObject.GetComponent<Image>();
                image.color = new Color(0.25f, 0.7f, 1f, 1f);
                image.raycastTarget = false;

                var row = i / columnCount;
                var column = i % columnCount;
                var posX = startX + column * (cellSize.x + spacing.x);
                var posY = startY - row * (cellSize.y + spacing.y);

                var rect = image.rectTransform;
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                rect.anchoredPosition = new Vector2(posX, posY);
                rect.sizeDelta = cellSize;

                benchmarkImages.Add(image);
            }

            var footer = CreateText("Footer", parent, $"{imageCount} 个 Image / {rows} 行", 18, FontStyle.Normal, TextAnchor.LowerRight);
            SetRect(footer.rectTransform, new Vector2(-20f, 20f), new Vector2(260f, 24f), TextAnchor.LowerRight);
        }

        private IEnumerator RunBenchmark(BenchmarkMode mode)
        {
            if (isRunning)
            {
                yield break;
            }

            isRunning = true;
            SetButtonsInteractable(false);
            UpdateDescription(RunningText);

            var writeTimes = new List<double>(sampleCount);
            var frameTimes = new List<double>(sampleCount);

            for (var i = 0; i < sampleCount; i++)
            {
                ResetBenchmarkImages();
                //Canvas.ForceUpdateCanvases();
                yield return null;

                var watch = Stopwatch.StartNew();

                RunBenchmarkMutation(mode);

                var writeCost = watch.Elapsed.TotalMilliseconds;
                //Canvas.ForceUpdateCanvases();
                yield return new WaitForEndOfFrame();
                watch.Stop();

                writeTimes.Add(writeCost);
                frameTimes.Add(watch.Elapsed.TotalMilliseconds);
            }

            ResetBenchmarkImages();
            //Canvas.ForceUpdateCanvases();
            yield return null;

            var resultText = BuildResultText(mode, writeTimes, frameTimes);
            if (mode == BenchmarkMode.Enabled)
            {
                enabledResultText.text = resultText;
            }
            else if (mode == BenchmarkMode.AlphaZero)
            {
                alphaResultText.text = resultText;
            }
            else
            {
                setActiveResultText.text = resultText;
            }

            UpdateDescription("结果已更新：write 表示批量修改耗时；frame 表示包含本帧 UI 刷新的总耗时。");
            SetButtonsInteractable(true);
            isRunning = false;
        }

        private void ResetBenchmarkImages()
        {
            for (var i = 0; i < benchmarkImages.Count; i++)
            {
                var image = benchmarkImages[i];
                image.gameObject.SetActive(true);
                image.enabled = true;

                var color = image.color;
                color.a = 1f;
                image.color = color;
            }
        }

        private void RunBenchmarkMutation(BenchmarkMode mode)
        {
            switch (mode)
            {
                case BenchmarkMode.Enabled:
                    SetImageEnabled(false);
                    break;
                case BenchmarkMode.AlphaZero:
                    SetImageAlpha(0f);
                    break;
                case BenchmarkMode.SetActiveFalse:
                    SetImageGameObjectActive(false);
                    break;
            }
        }

        private void SetImageEnabled(bool value)
        {
            for (var i = 0; i < benchmarkImages.Count; i++)
            {
                benchmarkImages[i].enabled = value;
            }
        }

        private void SetImageAlpha(float alpha)
        {
            for (var i = 0; i < benchmarkImages.Count; i++)
            {
                var image = benchmarkImages[i];
                var color = image.color;
                color.a = alpha;
                image.color = color;
            }
        }

        private void SetImageGameObjectActive(bool active)
        {
            for (var i = 0; i < benchmarkImages.Count; i++)
            {
                benchmarkImages[i].gameObject.SetActive(active);
            }
        }

        private void SetButtonsInteractable(bool interactable)
        {
            enabledBenchmarkButton.interactable = interactable;
            alphaBenchmarkButton.interactable = interactable;
            setActiveBenchmarkButton.interactable = interactable;
        }

        private void UpdateDescription(string content)
        {
            descriptionText.text = content;
        }

        private string BuildResultText(BenchmarkMode mode, List<double> writeTimes, List<double> frameTimes)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}: {1} 轮采样 | write avg {2:F4} ms (min {3:F4} / max {4:F4}) | frame avg {5:F4} ms (min {6:F4} / max {7:F4})",
                GetBenchmarkLabel(mode),
                sampleCount,
                CalculateAverage(writeTimes),
                CalculateMin(writeTimes),
                CalculateMax(writeTimes),
                CalculateAverage(frameTimes),
                CalculateMin(frameTimes),
                CalculateMax(frameTimes));
        }

        private string GetBenchmarkLabel(BenchmarkMode mode)
        {
            switch (mode)
            {
                case BenchmarkMode.Enabled:
                    return "Enabled";
                case BenchmarkMode.AlphaZero:
                    return "Alpha";
                case BenchmarkMode.SetActiveFalse:
                    return "SetActive";
                default:
                    return mode.ToString();
            }
        }

        private double CalculateAverage(List<double> values)
        {
            var sum = 0d;
            for (var i = 0; i < values.Count; i++)
            {
                sum += values[i];
            }

            return values.Count == 0 ? 0d : sum / values.Count;
        }

        private double CalculateMin(List<double> values)
        {
            if (values.Count == 0)
            {
                return 0d;
            }

            var min = values[0];
            for (var i = 1; i < values.Count; i++)
            {
                if (values[i] < min)
                {
                    min = values[i];
                }
            }

            return min;
        }

        private double CalculateMax(List<double> values)
        {
            if (values.Count == 0)
            {
                return 0d;
            }

            var max = values[0];
            for (var i = 1; i < values.Count; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                }
            }

            return max;
        }

        private Image CreatePanel(string objectName, Transform parent, Color color)
        {
            var panelObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
            panelObject.transform.SetParent(parent, false);

            var image = panelObject.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private Button CreateButton(string objectName, Transform parent, string label, Vector2 anchoredPosition, Vector2 size)
        {
            var buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            var buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.45f, 0.8f, 1f);

            var button = buttonObject.GetComponent<Button>();
            SetRect(buttonImage.rectTransform, anchoredPosition, size, TextAnchor.UpperLeft);

            var buttonText = CreateText("Label", buttonObject.transform, label, 20, FontStyle.Bold, TextAnchor.MiddleCenter);
            buttonText.color = Color.white;
            StretchRect(buttonText.rectTransform, new Vector2(16f, 10f));

            return button;
        }

        private Text CreateText(string objectName, Transform parent, string content, int fontSize, FontStyle fontStyle, TextAnchor alignment)
        {
            var textObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            var text = textObject.GetComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.color = Color.white;
            text.text = content;
            return text;
        }

        private void SetRect(RectTransform rectTransform, Vector2 anchoredPosition, Vector2 size, TextAnchor anchor)
        {
            var anchorValue = GetAnchor(anchor);
            rectTransform.anchorMin = anchorValue;
            rectTransform.anchorMax = anchorValue;
            rectTransform.pivot = anchorValue;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;
        }

        private void StretchRect(RectTransform rectTransform, Vector2 padding)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = new Vector2(padding.x, padding.y);
            rectTransform.offsetMax = new Vector2(-padding.x, -padding.y);
        }

        private Vector2 GetAnchor(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.UpperLeft:
                    return new Vector2(0f, 1f);
                case TextAnchor.UpperCenter:
                    return new Vector2(0.5f, 1f);
                case TextAnchor.UpperRight:
                    return new Vector2(1f, 1f);
                case TextAnchor.MiddleLeft:
                    return new Vector2(0f, 0.5f);
                case TextAnchor.MiddleCenter:
                    return new Vector2(0.5f, 0.5f);
                case TextAnchor.MiddleRight:
                    return new Vector2(1f, 0.5f);
                case TextAnchor.LowerLeft:
                    return new Vector2(0f, 0f);
                case TextAnchor.LowerCenter:
                    return new Vector2(0.5f, 0f);
                case TextAnchor.LowerRight:
                    return new Vector2(1f, 0f);
                default:
                    return new Vector2(0.5f, 0.5f);
            }
        }

        private enum BenchmarkMode
        {
            Enabled,
            AlphaZero,
            SetActiveFalse
        }
    }
}
