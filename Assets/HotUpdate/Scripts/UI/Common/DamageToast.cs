using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class DamageToast : MonoBehaviour
    {
        [SerializeField] private Transform _damageGroup;
        [SerializeField] private Image _imgDamage;
        [SerializeField] private Text _txtDamage;

        private bool _isCriticalAttack;
        private RectTransform _damageGroupRect;
        private RectTransform _canvasRect;
        private Vector2 _localPoint;

        public async void Init(int attack, bool isCriticalAttack, Vector3 enemyWorldPosition)
        {
            _txtDamage.text = attack.ToString();
            if (isCriticalAttack) _imgDamage.sprite = await GameManager.Resource.LoadResource<Sprite>("Assets/UI/Common/Common.spriteatlas[IconDamageCritical]", "damage");
            _damageGroupRect = _damageGroup.GetComponent<RectTransform>();
            _isCriticalAttack = isCriticalAttack;

            Vector3 screenPoint = Camera.main.WorldToScreenPoint(enemyWorldPosition);
            if (_canvasRect == null) _canvasRect = UIManager.GetInstance()._canvas.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPoint, null, out _localPoint);
            _damageGroupRect.anchoredPosition = _localPoint;
            _damageGroup.SetParent(_canvasRect.transform, false);

            PlayDamageAnimation();
        }

        #region 主要方法：伤害数值动画
        private void PlayDamageAnimation()
        {
            Color originalColor = _txtDamage.color;

            Sequence sequence = DOTween.Sequence();

            // 随机水平偏移
            float randomOffsetX = Random.Range(-40f, 40f);
            Vector2 endPosition = _damageGroupRect.anchoredPosition + new Vector2(randomOffsetX, 150f);

            sequence.Append(_damageGroupRect.DOAnchorPos(endPosition, 1F).SetEase(Ease.OutQuad));
            sequence.Join(_txtDamage.DOFade(0, 1f).SetEase(Ease.InQuad));

            if (_isCriticalAttack)
            {
                sequence.Join(_damageGroupRect.DOScale(1.8f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    _damageGroupRect.DOScale(1.5f, 0.1f);
                }));
            }

            sequence.Join(_txtDamage.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f), 0.5f).SetLoops(2, LoopType.Yoyo));

            sequence.OnComplete(() =>
            {
                // 放回对象池
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.DamageToast, gameObject);
            });

            sequence.SetAutoKill(true);
        }
        #endregion
    }
}
