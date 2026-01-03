using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class DamageParam : OpenUIParam
    {
        public int attack;
        public bool isCriticalAttack;
        public Vector2 localPosition;
    }

    public class DamagePanel : UIBasePanel
    {
        [SerializeField] private Transform _damageGroup;
        [SerializeField] private Image _imgDamage;
        [SerializeField] private Text _txtDamage;

        private bool _isCriticalAttack;
        private RectTransform _damageGroupRect;
        private Vector2 _localPoint;

        public override string GetPanelName()
        {
            return GlobalDefine.DamagePanel;
        }

        protected override async void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            _isBlockingWindow = false;

            DamageParam damageParam = param as DamageParam;

            _txtDamage.text = damageParam.attack.ToString();
            if (damageParam.isCriticalAttack)
            {
                _imgDamage.sprite = await GameManager.Resource.LoadResource<Sprite>("Assets/UI/Common/Common.spriteatlas[IconDamageCritical]", "damage");
            }
            _damageGroupRect = _damageGroup.GetComponent<RectTransform>();
            _isCriticalAttack = damageParam.isCriticalAttack;
            _localPoint = damageParam.localPosition;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_damageGroupRect);

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
                UIManager.GetInstance().ClosePanelAndDestory(GetPanelName());
            });

            sequence.SetAutoKill(true);
        }
        #endregion
    }
}
