using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class ConvertCell : MonoBehaviour
    {
        [SerializeField] private Image _imgIcon;
        [SerializeField] private Text _txtNum;

        private bool _canBeStacked; // ÊÇ·ñ¿ÉÒÔ¶Ñµþ

        public void Init(ItemInfo info)
        {
            _imgIcon.enabled = false;
            _txtNum.enabled = false;

            if (info != null && info._id != 0)
            {
                TBItemData data = ItemDataManager.GetInstance().GetData(info._id);
                GameManager.Resource.LoadResourceAsync<Sprite>(data.icon, GetInstanceID().ToString(), (obj, result) =>
                {
                    _imgIcon.sprite = obj as Sprite;
                    _imgIcon.enabled = true;
                });
                _canBeStacked = (data.type != 1);
                if (_canBeStacked) _txtNum.enabled = true;
                _txtNum.text = info._num.ToString();
            }
        }
    }
}
