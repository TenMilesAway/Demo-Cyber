using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Cyber
{
    /// <summary>
    /// 自动销毁的提示框
    /// </summary>
    public class ToastPanel : MonoBehaviour
    {
        [SerializeField] private Text txtToast;
        [SerializeField] private Transform m_toastTrs;

        private Sequence quence;

        public void Init(string content)
        {
            quence = DOTween.Sequence();
            txtToast.text = content;
            Show();
        }

        private void Show()
        {
            quence.Insert(0, m_toastTrs.DOLocalMoveY(0, 0));
            quence.Insert(0,
                m_toastTrs.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0));
            quence.Insert(0,
                txtToast.GetComponent<Text>().DOColor(new Color(0.99f, 1, 0.55f, 0), 0));
            quence.Insert(0.1f, m_toastTrs.DOLocalMoveY(100, 0.2f).SetEase(Ease.OutCirc));
            quence.Insert(0.1f,
                m_toastTrs.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1f), 0.2f));
            quence.Insert(0.1f,
                txtToast.GetComponent<Text>().DOColor(new Color(0.99f, 1, 0.55f, 1), 0.2f));
            quence.Insert(2, m_toastTrs.DOLocalMoveY(300, 0.5f).SetEase(Ease.InQuad));
            quence.Insert(2f,
                m_toastTrs.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.5f));
            quence.Insert(2f,
                txtToast.GetComponent<Text>().DOColor(new Color(0.99f, 1, 0.55f, 0), 0.5f));
            quence.OnComplete(() =>
            {
                m_toastTrs.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                txtToast.GetComponent<Text>().color = new Color(0.99f, 1, 0.55f, 1);
                m_toastTrs.GetComponent<RectTransform>().localPosition = new Vector3(0, 100, 0);
                Destroy(gameObject);
            });
        }
    }
}
