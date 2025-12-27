using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public enum PopType
    {
        Zoom,
        Translation,
    }

    public enum TranslationType
    {
        Left,
        Right,
    }

    public class PopPanel : MonoBehaviour
    {
        [Header("弹出类型")]
        [SerializeField] private PopType _popType;
        [Tooltip("只有选择 Translation 时该参数有效")]
        [SerializeField] private TranslationType _translationType;
        [Header("弹出曲线")]
        [SerializeField] private AnimationCurve _scaleCurve;

        private Coroutine _popCoroutine;
        private Vector3 _originalScale;
        private Vector3 _originalPosition;
        private RectTransform _rectTransform;

        private void OnEnable()
        {
            switch (_popType)
            {
                case PopType.Zoom:
                    {
                        _originalScale = transform.localScale;
                        _popCoroutine = StartCoroutine(PlayZoomAnimation());
                    }
                    break;
                case PopType.Translation:
                    {
                        _rectTransform = GetComponent<RectTransform>();
                        _originalPosition = _rectTransform.anchoredPosition;
                        _popCoroutine = StartCoroutine(PlayTranslationAnimation());
                    }
                    break;
            }
        }

        private void OnDisable()
        {
            switch(_popType)
            {
                case PopType.Zoom:
                    {
                        if (_popCoroutine != null)
                        {
                            StopCoroutine(_popCoroutine);
                            _popCoroutine = null;
                        }
                        transform.localScale = _originalScale;
                    }
                    break;
                case PopType.Translation:
                    {
                        if (_popCoroutine != null)
                        {
                            StopCoroutine(_popCoroutine);
                            _popCoroutine = null;
                        }
                        _rectTransform.anchoredPosition = _originalPosition;
                    }
                    break;
            }
        }

        private IEnumerator PlayZoomAnimation()
        {
            float elapsed = 0f;
            float time = _scaleCurve.keys[_scaleCurve.length - 1].time;
            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                float scaleValue = _scaleCurve.Evaluate(elapsed);
                transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
                yield return null;
            }

            transform.localScale = _originalScale;
        }

        /// <summary>
        /// 平移动画 (效果还不太好)
        /// </summary>
        private IEnumerator PlayTranslationAnimation()
        {
            Vector3 startPosition = CalculateStartPosition();

            _rectTransform.anchoredPosition = startPosition;

            float elapsed = 0f;
            float time = _scaleCurve.keys[_scaleCurve.length - 1].time;
            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                float translationValue = _scaleCurve.Evaluate(elapsed);
                _rectTransform.anchoredPosition = Vector3.Lerp(startPosition, _originalPosition, translationValue);
                yield return null;
            }

            _rectTransform.anchoredPosition = _originalPosition;
        }

        private Vector3 CalculateStartPosition()
        {
            float offsetX = 0;
            switch (_translationType)
            {
                case TranslationType.Left:
                    offsetX = -Screen.width;
                    break;
                case TranslationType.Right:
                    offsetX = Screen.width;
                    break;
            }

            return _originalPosition + new Vector3(offsetX, 0, 0);
        }
    }
}
