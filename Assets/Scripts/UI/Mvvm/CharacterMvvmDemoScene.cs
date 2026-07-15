using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MvvmTest
{
    public sealed class CharacterMvvmDemoScene : MonoBehaviour
    {
        private const string CanvasName = "MvvmDemoCanvas";
        private const string BuiltinFontPath = "LegacyRuntime.ttf";

        private Font defaultFont;
        private Slider hpBar;
        private Text hpText;
        private Text goldText;
        private Text levelText;
        private Text statusText;
        private Button buyButton;
        private Button damageButton;
        private Button healButton;
        private Button gainGoldButton;
        private Button levelUpButton;
        private Button resetButton;
        private CharacterMvvmView view;
        private CharacterMvvmModel model;
        private CharacterMvvmViewModel viewModel;

        private void Start()
        {
            defaultFont = Resources.GetBuiltinResource<Font>(BuiltinFontPath);

            var canvas = GetOrCreateCanvas();
            CreateOrGetEventSystem();
            ClearGeneratedChildren(canvas.transform);
            BuildRuntimeUi(canvas.transform);
        }

        private void OnDestroy()
        {
            DisposePanel();
        }

        public void DisposePanel()
        {
            if (viewModel != null)
            {
                viewModel.StatusText.Value = "已解除 View 绑定，业务按钮已停用，点击重新绑定可恢复。";
            }

            view?.Dispose();
            view = null;
        }

        public void RebindPanel()
        {
            EnsurePresentationObjects();
            EnsureView();

            view.Bind(viewModel);
            viewModel.StatusText.Value = "已重新绑定 View 与 ViewModel。";
        }

        private void BuildRuntimeUi(Transform root)
        {
            var title = CreateText("Title", root, "MVVM UI Demo", 34, FontStyle.Bold, TextAnchor.UpperLeft);
            SetRect(title.rectTransform, new Vector2(32f, -24f), new Vector2(900f, 48f), TextAnchor.UpperLeft);

            var description = CreateText(
                "Description",
                root,
                "View 继承 UI 基类并持有 _viewModel / _disposables；订阅放在 View 内，ViewModel 只负责驱动 Model 与派生展示数据。",
                20,
                FontStyle.Normal,
                TextAnchor.UpperLeft);
            SetRect(description.rectTransform, new Vector2(32f, -78f), new Vector2(1300f, 56f), TextAnchor.UpperLeft);

            var panel = CreatePanel("DemoPanel", root, new Color(0.09f, 0.12f, 0.18f, 0.94f));
            SetRect(panel.rectTransform, new Vector2(32f, -156f), new Vector2(960f, 520f), TextAnchor.UpperLeft);

            var hpLabel = CreateText("HpLabel", panel.transform, "HP", 22, FontStyle.Bold, TextAnchor.UpperLeft);
            SetRect(hpLabel.rectTransform, new Vector2(28f, -28f), new Vector2(120f, 32f), TextAnchor.UpperLeft);

            hpBar = CreateSlider("HpBar", panel.transform, new Vector2(28f, -74f), new Vector2(620f, 28f));
            hpText = CreateText("HpText", panel.transform, string.Empty, 20, FontStyle.Normal, TextAnchor.UpperLeft);
            SetRect(hpText.rectTransform, new Vector2(670f, -66f), new Vector2(220f, 32f), TextAnchor.UpperLeft);

            goldText = CreateText("GoldText", panel.transform, string.Empty, 22, FontStyle.Normal, TextAnchor.UpperLeft);
            SetRect(goldText.rectTransform, new Vector2(28f, -126f), new Vector2(280f, 32f), TextAnchor.UpperLeft);

            levelText = CreateText("LevelText", panel.transform, string.Empty, 22, FontStyle.Normal, TextAnchor.UpperLeft);
            SetRect(levelText.rectTransform, new Vector2(320f, -126f), new Vector2(220f, 32f), TextAnchor.UpperLeft);

            var statusTitle = CreateText("StatusTitle", panel.transform, "状态", 22, FontStyle.Bold, TextAnchor.UpperLeft);
            SetRect(statusTitle.rectTransform, new Vector2(28f, -182f), new Vector2(120f, 32f), TextAnchor.UpperLeft);

            statusText = CreateText("StatusText", panel.transform, string.Empty, 20, FontStyle.Normal, TextAnchor.UpperLeft);
            SetRect(statusText.rectTransform, new Vector2(28f, -220f), new Vector2(860f, 70f), TextAnchor.UpperLeft);

            buyButton = CreateButton("BuyButton", panel.transform, "购买 -100 Gold", new Vector2(28f, -330f), new Vector2(220f, 52f));
            damageButton = CreateButton("DamageButton", panel.transform, "受伤 -15 HP", new Vector2(268f, -330f), new Vector2(180f, 52f));
            healButton = CreateButton("HealButton", panel.transform, "治疗 +10 HP", new Vector2(468f, -330f), new Vector2(180f, 52f));
            gainGoldButton = CreateButton("GainGoldButton", panel.transform, "获得 +200 Gold", new Vector2(668f, -330f), new Vector2(220f, 52f));
            levelUpButton = CreateButton("LevelUpButton", panel.transform, "升级 +1", new Vector2(28f, -400f), new Vector2(220f, 52f));
            resetButton = CreateButton("ResetButton", panel.transform, "重置", new Vector2(268f, -400f), new Vector2(180f, 52f));
            var disposePanelButton = CreateButton("DisposePanelButton", panel.transform, "调用 DisposePanel", new Vector2(468f, -400f), new Vector2(180f, 52f));
            var rebindPanelButton = CreateButton("RebindPanelButton", panel.transform, "重新绑定", new Vector2(668f, -400f), new Vector2(220f, 52f));

            disposePanelButton.onClick.AddListener(DisposePanel);
            rebindPanelButton.onClick.AddListener(RebindPanel);

            var footer = CreateText(
                "Footer",
                panel.transform,
                "测试建议：先操作业务按钮，再点击 DisposePanel 验证解绑，最后点击重新绑定恢复自动刷新。",
                18,
                FontStyle.Normal,
                TextAnchor.LowerLeft);
            SetRect(footer.rectTransform, new Vector2(28f, 20f), new Vector2(860f, 28f), TextAnchor.LowerLeft);

            EnsurePresentationObjects();
            EnsureView();
            RebindPanel();
        }

        private void EnsurePresentationObjects()
        {
            if (model == null)
            {
                model = new CharacterMvvmModel();
            }

            if (viewModel == null)
            {
                viewModel = new CharacterMvvmViewModel(model);
            }
        }

        private void EnsureView()
        {
            if (view != null)
            {
                return;
            }

            view = hpBar.GetComponentInParent<CharacterMvvmView>();
            if (view == null)
            {
                view = hpBar.gameObject.transform.parent.gameObject.AddComponent<CharacterMvvmView>();
            }

            view.Initialize(
                hpBar,
                hpText,
                goldText,
                levelText,
                statusText,
                buyButton,
                damageButton,
                healButton,
                gainGoldButton,
                levelUpButton,
                resetButton);
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
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

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
            buttonImage.color = new Color(0.18f, 0.44f, 0.84f, 1f);

            var button = buttonObject.GetComponent<Button>();
            SetRect(buttonImage.rectTransform, anchoredPosition, size, TextAnchor.UpperLeft);

            var buttonText = CreateText("Label", buttonObject.transform, label, 20, FontStyle.Bold, TextAnchor.MiddleCenter);
            buttonText.color = Color.white;
            StretchRect(buttonText.rectTransform, new Vector2(16f, 10f));
            return button;
        }

        private Slider CreateSlider(string objectName, Transform parent, Vector2 anchoredPosition, Vector2 size)
        {
            var sliderObject = new GameObject(objectName, typeof(RectTransform), typeof(Slider));
            sliderObject.transform.SetParent(parent, false);

            var rectTransform = sliderObject.GetComponent<RectTransform>();
            SetRect(rectTransform, anchoredPosition, size, TextAnchor.UpperLeft);

            var background = new GameObject("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(sliderObject.transform, false);
            var backgroundImage = background.GetComponent<Image>();
            backgroundImage.color = new Color(0.14f, 0.16f, 0.2f, 1f);
            StretchRect(backgroundImage.rectTransform, Vector2.zero);

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObject.transform, false);
            var fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(4f, 4f);
            fillAreaRect.offsetMax = new Vector2(-4f, -4f);

            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            var fillImage = fill.GetComponent<Image>();
            fillImage.color = new Color(0.23f, 0.78f, 0.38f, 1f);
            StretchRect(fillImage.rectTransform, Vector2.zero);

            var slider = sliderObject.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            slider.direction = Slider.Direction.LeftToRight;
            slider.targetGraphic = fillImage;
            slider.fillRect = fillImage.rectTransform;
            return slider;
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
    }
}
