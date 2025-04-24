using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Cyber
{
    public class AwardAnimation : MonoBehaviour
    {
        private Vector3 initPosition;

        private void Start()
        {
            PlayAnimation();
        }

        public void PlayAnimation()
        {
            Sequence sequence1 = DOTween.Sequence();
            sequence1.SetEase(Ease.Linear);
            sequence1.Append(transform.DORotate(new Vector3(0, transform.eulerAngles.y + 180, 0), 2));

            Sequence sequence2 = DOTween.Sequence();

            sequence2.Append(transform.DOJump(new Vector3(transform.position.x, -11f, transform.position.z), 0.1f, 1, 2));
            sequence2.SetEase(Ease.Linear);
            sequence2.OnComplete(() => 
            {
                PlayAnimation();
            });
        }
    }
}
