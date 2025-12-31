using Cyber;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class PropertyPanelParam : OpenUIParam
    {
        
    }

    public class PropertyPanel : UIBasePanel
    {
        [Header("装备")]
        [SerializeField] private EquipCell _weaponCell;          // 武器
        [SerializeField] private EquipCell _helmetCell;          // 头盔
        [SerializeField] private EquipCell _cuirassCell;         // 胸甲
        [SerializeField] private EquipCell _secondaryWeaponCell; // 灵珠
        [SerializeField] private EquipCell _cuishCell;           // 腿甲
        [SerializeField] private EquipCell _shoesCell;           // 鞋子

        [Header("属性：左侧")]
        [SerializeField] private Text _txtHP;
        [SerializeField] private Text _txtMP;
        [SerializeField] private Text _txtEXP;
        [SerializeField] private Text _txtAttack;
        [SerializeField] private Text _txtArmorPenetration;
        [SerializeField] private Text _txtCriticalProbability;
        [SerializeField] private Text _txtSuckProbability;

        [Header("属性：右侧")]
        [SerializeField] private Text _txtCHP;
        [SerializeField] private Text _txtCMP;
        [SerializeField] private Text _txtCEXP;
        [SerializeField] private Text _txtDefense;
        [SerializeField] private Text _txtDamageAvoidance;
        [SerializeField] private Text _txtCriticalMultiplier;
        [SerializeField] private Text _txtSuckMultiplier;

        private PlayerInfo _playerInfo;
        private List<ItemInfo> _nowEquips;

        public override string GetPanelName()
        {
            return GlobalDefine.PropertyPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            _playerInfo = PlayerDataManager.GetInstance().GetPlayerInfo();
            _nowEquips = _playerInfo._nowEquips;

            InitEquips();
            InitProperties();

            AddListeners();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            RemoveListeners();
        }

        private void AddListeners()
        {
            GameManager.Event.AddListener(GameEventType.UpdatePropertyPanelUI, UpdateUI);
        }
        
        private void RemoveListeners()
        {
            GameManager.Event.RemoveListener(GameEventType.UpdatePropertyPanelUI, UpdateUI);

            _weaponCell.RemoveListeners();
            _helmetCell.RemoveListeners();
            _cuirassCell.RemoveListeners();
            _secondaryWeaponCell.RemoveListeners();
            _cuishCell.RemoveListeners();
            _shoesCell.RemoveListeners();
        }

        #region 主要方法
        /// <summary>
        /// 初始化装备格子
        /// </summary>
        private void InitEquips()
        {
            UpdateEquips();
        }

        /// <summary>
        /// 更新装备格子
        /// </summary>
        private void UpdateEquips()
        {
            foreach (ItemInfo info in _nowEquips)
            {
                int type = (info._id - 4000) / 1000;
                switch (type)
                {
                    case 0:
                        {
                            _weaponCell.Init(info);
                        }
                        break;
                    case 1:
                        {
                            _helmetCell.Init(info);
                        }
                        break;
                    case 2:
                        {
                            _cuirassCell.Init(info);
                        }
                        break;
                    case 3:
                        {
                            _secondaryWeaponCell.Init(info);
                        }
                        break;
                    case 4:
                        {
                            _cuishCell.Init(info);
                        }
                        break;
                    case 5:
                        {
                            _shoesCell.Init(info);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 初始化属性
        /// </summary>
        private void InitProperties()
        {
            UpdateProperties();
        }

        /// <summary>
        /// 更新属性
        /// </summary>
        private void UpdateProperties()
        {
            // 左侧
            _txtHP.text = string.Format("<color=grey>血量值上限：</color>{0}", _playerInfo._maxHP.ToString());
            _txtMP.text = string.Format("<color=grey>法力值上限：</color>{0}", _playerInfo._maxMP.ToString());
            _txtEXP.text = string.Format("<color=grey>升级所需经验：</color>{0}", _playerInfo._maxEXP.ToString());
            _txtAttack.text = string.Format("<color=#0091FF>攻击力：</color>{0}", _playerInfo._pAttack.ToString());
            _txtArmorPenetration.text = string.Format("<color=#0091FF>破甲值：</color>{0}%", _playerInfo._pArmorPenetration.ToString());
            _txtCriticalProbability.text = string.Format("<color=#E34242>暴击率：</color>{0}%", _playerInfo._pCriticalProbability.ToString());
            _txtSuckProbability.text = string.Format("<color=#E34242>吸血率：</color>{0}%", _playerInfo._pSuckProbability.ToString());

            // 右侧
            _txtCHP.text = string.Format("<color=grey>当前血量值：</color>{0}", _playerInfo._currentHP.ToString());
            _txtCMP.text = string.Format("<color=grey>当前法力值：</color>{0}", _playerInfo._currentMP.ToString());
            _txtCEXP.text = string.Format("<color=grey>当前经验值：</color>{0}", _playerInfo._currentEXP.ToString());
            _txtDefense.text = string.Format("<color=#0091FF>防御力：</color>{0}", _playerInfo._pDefense.ToString());
            _txtDamageAvoidance.text = string.Format("<color=#0091FF>免伤值：</color>{0}%", _playerInfo._pDamageAvoidance.ToString());
            _txtCriticalMultiplier.text = string.Format("<color=#E34242>暴击倍率：</color>{0}%", (_playerInfo._pCriticalMultiplier * 100).ToString());
            _txtSuckMultiplier.text = string.Format("<color=#E34242>吸血倍率：</color>{0}%", _playerInfo._pSuckMultiplier.ToString());
        }

        private void UpdateUI()
        {
            UpdateEquips();
            UpdateProperties();
        }
        #endregion
    }
}
