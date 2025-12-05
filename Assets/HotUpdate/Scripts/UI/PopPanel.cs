using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class PopPanel : MonoBehaviour
    {
        [SerializeField]
        private AnimationCurve _scaleCurve;

        private Coroutine _popCoroutine;
        private Vector3 _originalScale;

        private void OnEnable()
        {
            _originalScale = transform.localScale;
            _popCoroutine = StartCoroutine(PlayPopAnimation());
        }

        private void OnDisable()
        {
            if (_popCoroutine != null)
            {
                StopCoroutine(_popCoroutine);
                _popCoroutine = null;
            }
            transform.localScale = _originalScale;
        }

        private IEnumerator PlayPopAnimation()
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
    }
}
