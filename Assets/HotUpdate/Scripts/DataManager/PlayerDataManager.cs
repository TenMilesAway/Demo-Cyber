using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cyber;

namespace HA
{
    /// <summary>
    /// 玩家信息管理器
    /// </summary>
    public class PlayerDataManager : BaseManager<PlayerDataManager>
    {
        private PlayerInfo _playerInfo; // 玩家信息
        private PlayerInput _input;     // 玩家输入
        private Transform _player;      // 玩家位置

        private const string _playerTag = "Player"; // 玩家 Tag

        public void Init()
        {
            NetManager.AddMsgListener(GameEventType.MsgPlayerBaseLoad.ToString(), RpsPlayerBaseLoad);           // 3响应：获得玩家基础信息
            NetManager.AddMsgListener(GameEventType.MsgPlayerBaseSave.ToString(), RpsPlayerBaseSave);           // 4响应：保存玩家基础信息
            NetManager.AddMsgListener(GameEventType.MsgPlayerStatsLoad.ToString(), RpsPlayerStatsLoad);         // 5响应：获得玩家状态信息
            NetManager.AddMsgListener(GameEventType.MsgPlayerStatsSave.ToString(), RpsPlayerStatsSave);         // 6响应：保存玩家状态信息
            NetManager.AddMsgListener(GameEventType.MsgPlayerInventoryLoad.ToString(), RpsPlayerInventoryLoad); // 7响应：获得玩家背包信息
            NetManager.AddMsgListener(GameEventType.MsgPlayerInventorySave.ToString(), RpsPlayerInventorySave); // 8响应：保存玩家背包信息

            GameManager.Event.AddListener(GameEventType.ReqPlayerBaseLoad, ReqPlayerBaseLoad);                  // 3请求：获得玩家基础信息
            GameManager.Event.AddListener(GameEventType.ReqPlayerBaseSave, ReqPlayerBaseSave);                  // 4请求：保存玩家基础信息
            GameManager.Event.AddListener(GameEventType.ReqPlayerStatsLoad, ReqPlayerStatsLoad);                // 5请求：获得玩家状态信息
            GameManager.Event.AddListener(GameEventType.ReqPlayerStatsSave, ReqPlayerStatsSave);                // 6请求：保存玩家状态信息
            GameManager.Event.AddListener(GameEventType.ReqPlayerInventoryLoad, ReqPlayerInventoryLoad);        // 7请求：获得玩家背包信息
            GameManager.Event.AddListener(GameEventType.ReqPlayerInventorySave, ReqPlayerInventorySave);        // 8请求：保存玩家背包信息

            // 请求玩家数据
            ReqPlayerBaseLoad();
            ReqPlayerStatsLoad();
            ReqPlayerInventoryLoad();

            // 玩家输入
            _input = GameObject.FindGameObjectWithTag(_playerTag).GetComponent<PlayerInput>();
        }

        #region 主要方法：修改玩家信息
        /// <summary>
        /// 设置玩家 Transform
        /// </summary>
        public void SetPlayer(Transform player)
        {
            _player = player;
        }

        /// <summary>
        /// 设置玩家至指定坐标
        /// </summary>
        public void SetPlayerToPlace(Vector3 position)
        {
            _player.position = position;
        }

        /// <summary>
        /// 当前玩家的 MainCameraTransform 是否存在
        /// </summary>
        public bool GetPlayerMainCamera()
        {
            if (_player.GetComponentInChildren<Player>().MainCameraTransform == null) return false;
            else return true;
        }

        /// <summary>
        /// 设置玩家的 MainCameraTransform
        /// </summary>
        public void SetPlayerMainCamera()
        {
            _player.GetComponentInChildren<Player>().MainCameraTransform = Camera.main.transform;
        }

        /// <summary>
        /// 获取玩家信息 (引用)
        /// </summary>
        public PlayerInfo GetPlayerInfo()
        {
            return _playerInfo;
        }

        /// <summary>
        /// 设置玩家信息
        /// </summary>
        public void SetPlayerInfo(PlayerInfo playerInfo)
        {
            _playerInfo = playerInfo;
        }

        /// <summary>
        /// 获取玩家输入组件
        /// </summary>
        public PlayerInput GetPlayerInput()
        {
            return _input;
        }

        /// <summary>
        /// 异步获取玩家信息 (初始化阶段)
        /// </summary>
        /// <param name="timeoutSeconds">总请求时长</param>
        /// <param name="pollInterval">每次请求间隔</param>
        public async Task<PlayerInfo> GetPlayerInfoAsync(int timeoutSeconds = 1, float pollInterval = 0.02f)
        {
            float startTime = Time.time;

            while (Time.time - startTime < timeoutSeconds)
            {
                PlayerInfo playerInfo = GetPlayerInfo();

                if (playerInfo != null && !string.IsNullOrEmpty(playerInfo._id) && playerInfo._maxHP != 0)
                {
                    HADebug.LogFormat("玩家信息加载成功[{0}], 耗时[{1}]秒", playerInfo.ToString(), Time.time - startTime);
                    return playerInfo;
                }

                await Task.Delay((int)(pollInterval * 1000));
            }

            HADebug.LogErrorFormat("玩家信息加载超时, 超过[{0}]秒", timeoutSeconds);
            return default;
        }
        #endregion

        #region 主要方法：涉及数据刷新和上传
        /// <summary>
        /// 玩家获得经验值
        /// </summary>
        public void SendEXPToPlayer(int exp)
        {
            _playerInfo._currentEXP += exp;

            UpdateLevel();
            ReqPlayerBaseSave();
            ReqPlayerStatsSave();
        }
        #endregion

        #region 监听方法：发送请求
        /// <summary>
        /// 请求：获取玩家基础信息
        /// </summary>
        private void ReqPlayerBaseLoad()
        {
            MsgPlayerBaseLoad msg = new MsgPlayerBaseLoad();

            msg.playerBaseEntity = new PlayerBaseEntity(false);
            msg.playerBaseEntity.id = GameManager.GlobalData.PlayerID;

            NetManager.Send(msg);
        }

        /// <summary>
        /// 请求：保存玩家基础信息
        /// </summary>
        private void ReqPlayerBaseSave()
        {
            MsgPlayerBaseSave msg = new MsgPlayerBaseSave();

            msg.playerBaseEntity = new PlayerBaseEntity
            {
                id = _playerInfo._id,
                name = _playerInfo._name,
                head = _playerInfo._head,
                level = _playerInfo._level,
                common_currency = _playerInfo._commonCurrency,
                rare_currency = _playerInfo._rareCurrency
            };

            NetManager.Send(msg);
        }

        /// <summary>
        /// 请求：获取玩家状态信息
        /// </summary>
        private void ReqPlayerStatsLoad()
        {
            MsgPlayerStatsLoad msg = new MsgPlayerStatsLoad();

            msg.playerStatsEntity = new PlayerStatsEntity(false);
            msg.playerStatsEntity.player_id = GameManager.GlobalData.PlayerID;

            NetManager.Send(msg);
        }

        /// <summary>
        /// 请求：保存玩家状态信息
        /// </summary>
        private void ReqPlayerStatsSave()
        {
            MsgPlayerStatsSave msg = new MsgPlayerStatsSave();

            msg.playerStatsEntity = new PlayerStatsEntity
            {
                player_id = _playerInfo._id,
                max_hp = _playerInfo._maxHP,
                max_mp = _playerInfo._maxMP,
                max_exp = _playerInfo._maxEXP,
                current_hp = _playerInfo._currentHP,
                current_mp = _playerInfo._currentMP,
                current_exp = _playerInfo._currentEXP,
                attack = _playerInfo._pAttack,
                armor_penetration = _playerInfo._pArmorPenetration,
                defense = _playerInfo._pDefense,
                damage_avoidance = _playerInfo._pDamageAvoidance,
                critical_probability = _playerInfo._pCriticalProbability,
                critical_multiplier = _playerInfo._pCriticalMultiplier,
                suck_probability = _playerInfo._pSuckProbability,
                suck_multiplier = _playerInfo._pSuckMultiplier,
            };

            NetManager.Send(msg);
        }

        /// <summary>
        /// 请求：获取玩家背包信息
        /// </summary>
        private void ReqPlayerInventoryLoad()
        {
            MsgPlayerInventoryLoad msg = new MsgPlayerInventoryLoad();

            msg.playerInventoryEntity = new PlayerInventoryEntity(false);
            msg.playerInventoryEntity.player_id = GameManager.GlobalData.PlayerID;

            NetManager.Send(msg);
        }

        /// <summary>
        /// 请求：保存玩家背包信息
        /// </summary>
        private void ReqPlayerInventorySave()
        {
            MsgPlayerInventorySave msg = new MsgPlayerInventorySave();

            msg.playerInventoryEntity = new PlayerInventoryEntity
            {
                player_id = _playerInfo._id,
                items = _playerInfo._allItems,
                now_equips = _playerInfo._nowEquips,
                inventory_num = _playerInfo._inventoryItemNum,
                safebox_num = _playerInfo._safeboxNum,
            };

            NetManager.Send(msg);
        }
        #endregion

        #region 监听方法：请求响应
        /// <summary>
        /// 响应：获取玩家基础信息
        /// </summary>
        private void RpsPlayerBaseLoad(MsgBase msgBase)
        {
            MsgPlayerBaseLoad msg = (MsgPlayerBaseLoad)msgBase;

            if (msg.result == 0)
            {
                HADebug.Log("[客户端] PlayerBase 获取成功!");
                UpdatePlayerInfoByPlayerBase(msg.playerBaseEntity);
            }
            else
            {
                HADebug.LogWarning("[客户端] PlayerBase 获取失败, 生成默认数据并存储");
                PlayerBaseEntity entity = new PlayerBaseEntity(true);
                UpdatePlayerInfoByPlayerBase(entity);
                ReqPlayerBaseSave();
            }
        }

        /// <summary>
        /// 响应：保存玩家基础信息
        /// </summary>
        private void RpsPlayerBaseSave(MsgBase msgBase)
        {
            MsgPlayerBaseSave msg = (MsgPlayerBaseSave)msgBase;

            if (msg.result == 0)
            {
                HADebug.LogFormat("[客户端] PlayerBase 存储成功!");
            }
            else
            {
                HADebug.LogErrorFormat("[客户端] PlayerBase 存储失败");
            }
        }

        /// <summary>
        /// 响应：获取玩家状态信息
        /// </summary>
        private void RpsPlayerStatsLoad(MsgBase msgBase)
        {
            MsgPlayerStatsLoad msg = (MsgPlayerStatsLoad)msgBase;

            if (msg.result == 0)
            {
                HADebug.Log("[客户端] PlayerStats 获取成功!");
                UpdatePlayerInfoByPlayerStats(msg.playerStatsEntity);
            }
            else
            {
                HADebug.LogWarning("[客户端] PlayerStats 获取失败, 生成默认数据并存储");
                PlayerStatsEntity entity = new PlayerStatsEntity(true);
                UpdatePlayerInfoByPlayerStats(entity);
                ReqPlayerStatsSave();
            }
        }

        /// <summary>
        /// 响应：保存玩家状态信息
        /// </summary>
        private void RpsPlayerStatsSave(MsgBase msgBase)
        {
            MsgPlayerStatsSave msg = (MsgPlayerStatsSave)msgBase;

            if (msg.result == 0)
            {
                HADebug.LogFormat("[客户端] PlayerStats 存储成功!");
            }
            else
            {
                HADebug.LogErrorFormat("[客户端] PlayerStats 存储失败");
            }
        }

        /// <summary>
        /// 响应：获取玩家背包信息
        /// </summary>
        private void RpsPlayerInventoryLoad(MsgBase msgBase)
        {
            MsgPlayerInventoryLoad msg = (MsgPlayerInventoryLoad)msgBase;

            if (msg.result == 0)
            {
                HADebug.Log("[客户端] PlayerInventory 获取成功!");
                UpdatePlayerInfoByPlayerInventory(msg.playerInventoryEntity);
            }
            else
            {
                HADebug.Log("[客户端] PlayerInventory 获取失败，生成默认数据并存储");
                PlayerInventoryEntity entity = new PlayerInventoryEntity(true);
                UpdatePlayerInfoByPlayerInventory(entity);
                ReqPlayerInventorySave();
            }

            
        }

        /// <summary>
        /// 响应：保存玩家背包信息
        /// </summary>
        private void RpsPlayerInventorySave(MsgBase msgBase)
        {
            MsgPlayerInventorySave msg = (MsgPlayerInventorySave)msgBase;

            if (msg.result == 0)
            {
                HADebug.LogFormat("[客户端] PlayerInventory 存储成功!");
            }
            else
            {
                HADebug.LogErrorFormat("[客户端] PlayerInventory 存储失败");
            }
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 获得类型的仓库容量
        /// 修改后：目前只有 ItemNun
        /// </summary>
        public int GetItemNumByType(int type)
        {
            int num = 0;

            switch(type)
            {
                case 1:
                    {
                        num = _playerInfo._inventoryItemNum;
                    }
                    break;
            }

            return num;
        }
        #endregion

        #region 数据刷新
        /// --------------------------
        /// - 刷新方法：             -
        /// - 1. 刷新当前数据        -
        /// - 2. 刷新对应 UI         -
        /// - 3. 刷新其余必要位置    -
        /// --------------------------

        /// <summary>
        /// 刷新 Level 数据
        /// </summary>
        private void UpdateLevel()
        {
            // 刷新当前数据
            int nowLevel = _playerInfo._level;
            int nowMaxExp = LevelDataManager.GetInstance().GetData(nowLevel).exp;
            if (_playerInfo._currentEXP >= nowMaxExp)
            {
                _playerInfo._level += 1;
                _playerInfo._currentEXP -= nowMaxExp;
                _playerInfo._maxEXP = LevelDataManager.GetInstance().GetData(_playerInfo._level).exp;
            }

            // 刷新对应 UI
            GameManager.Event.Broadcast<PlayerInfo>(GameEventType.UpdateMainPanelUI, _playerInfo);

            // 刷新其余必要位置
        }

        /// <summary>
        /// 通过 PlayerBaseEntity 更新 _playerInfo
        /// </summary>
        private void UpdatePlayerInfoByPlayerBase(PlayerBaseEntity entity)
        {
            // 刷新当前数据
            if (_playerInfo == null) _playerInfo = new PlayerInfo(false);
            _playerInfo._id = entity.id;
            _playerInfo._name = entity.name;
            _playerInfo._head = entity.head;
            _playerInfo._level = entity.level;
            _playerInfo._commonCurrency = entity.common_currency;
            _playerInfo._rareCurrency = entity.rare_currency;

            // 刷新对应 UI
            GameManager.Event.Broadcast<PlayerInfo>(GameEventType.UpdateMainPanelUI, _playerInfo);

            // 刷新其余必要位置
        }

        /// <summary>
        /// 通过 PlayerStatsEntity 更新 _playerInfo
        /// </summary>
        private void UpdatePlayerInfoByPlayerStats(PlayerStatsEntity entity)
        {
            // 刷新当前数据
            if (_playerInfo == null) _playerInfo = new PlayerInfo(false);
            _playerInfo._maxHP = entity.max_hp;
            _playerInfo._maxMP = entity.max_mp;
            _playerInfo._maxEXP = entity.max_exp;
            _playerInfo._currentHP = entity.current_hp;
            _playerInfo._currentMP = entity.current_mp;
            _playerInfo._currentEXP = entity.current_exp;
            _playerInfo._pAttack = entity.attack;
            _playerInfo._pArmorPenetration = entity.armor_penetration;
            _playerInfo._pDefense = entity.defense;
            _playerInfo._pDamageAvoidance = entity.damage_avoidance;
            _playerInfo._pCriticalProbability = entity.critical_probability;
            _playerInfo._pCriticalMultiplier = entity.critical_multiplier;
            _playerInfo._pSuckProbability = entity.suck_probability;
            _playerInfo._pSuckMultiplier = entity.suck_multiplier;

            // 刷新对应 UI
            GameManager.Event.Broadcast(GameEventType.UpdatePropertyPanelUI);

            // 刷新其余必要位置
        }

        /// <summary>
        /// 通过 PlayerInventoryEntity 更新 _playerInfo
        /// </summary>
        private void UpdatePlayerInfoByPlayerInventory(PlayerInventoryEntity entity)
        {
            // 刷新当前数据
            if (_playerInfo == null) _playerInfo = new PlayerInfo(false);
            _playerInfo._allItems = entity.items;
            _playerInfo._nowEquips = entity.now_equips;
            _playerInfo._inventoryItemNum = entity.inventory_num;
            _playerInfo._safeboxNum = entity.safebox_num;

            // 刷新对应 UI


            // 刷新其余必要位置
            GameManager.Event.Broadcast<PlayerInfo>(GameEventType.UpdateInventoryItemList, _playerInfo);
        }
        #endregion
    }
}
