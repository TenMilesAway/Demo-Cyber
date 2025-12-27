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
        private PlayerInfo _playerInfo;
        private PlayerInput _input;
        private Transform _player;

        private const string _playerTag = "Player";

        public void Init()
        {
            NetManager.AddMsgListener(GameEventType.HAMsgPlayerInfoLoad.ToString(), RpsPlayerInfoLoad);
            NetManager.AddMsgListener(GameEventType.HAMsgPlayerInfoUpload.ToString(), RpsPlayerInfoUpload);
            GameManager.Event.AddListener(GameEventType.ReqPlayerInfoLoad, ReqPlayerInfoLoad);
            GameManager.Event.AddListener(GameEventType.ReqPlayerInfoUpload, ReqPlayerInfoUpload);

            // 请求玩家数据
            GameManager.Event.Broadcast(GameEventType.ReqPlayerInfoLoad);

            _input = GameObject.FindGameObjectWithTag(_playerTag).GetComponent<PlayerInput>();
        }

        #region 主要方法
        public void SetPlayer(Transform player)
        {
            _player = player;
        }

        public void SetPlayerToPlace(Vector3 position)
        {
            _player.position = position;
        }

        public bool GetPlayerMainCamera()
        {
            if (_player.GetComponentInChildren<Player>().MainCameraTransform == null) return false;
            else return true;
        }

        public void SetPlayerMainCamera()
        {
            _player.GetComponentInChildren<Player>().MainCameraTransform = Camera.main.transform;
        }

        /// <summary>
        /// 获取玩家信息
        /// </summary>
        public PlayerInfo GetPlayerInfo()
        {
            return _playerInfo;
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

                if (playerInfo != null && !string.IsNullOrEmpty(playerInfo._id))
                {
                    HADebug.LogFormat("玩家信息加载成功[{0}], 耗时[{1}]秒", playerInfo.ToString(), Time.time - startTime);
                    return playerInfo;
                }

                await Task.Delay((int)(pollInterval * 1000));
            }

            HADebug.LogErrorFormat("玩家信息加载超时, 超过[{0}]秒", timeoutSeconds);
            return default;
        }

        /// <summary>
        /// 玩家获得经验值
        /// </summary>
        public void SendEXPToPlayer(int exp)
        {
            _playerInfo._currentEXP += exp;
            RefreshLevel();
            ReqPlayerInfoUpload();
        }

        private void RefreshLevel()
        {
            int nowLevel = _playerInfo._level;
            int nowMaxExp = LevelDataManager.GetInstance().GetData(nowLevel).exp;

            if (_playerInfo._currentEXP > nowMaxExp)
            {
                _playerInfo._level += 1;
                _playerInfo._currentEXP -= nowMaxExp;
            }

            GameManager.Event.Broadcast<PlayerInfo>(GameEventType.UpdateMainPanelUI, _playerInfo);
        }

        /// <summary>
        /// 向服务器请求获取玩家信息
        /// </summary>
        private void ReqPlayerInfoLoad()
        {
            HAMsgPlayerInfoLoad msg = new HAMsgPlayerInfoLoad();

            msg.playerInfo = new PlayerInfo(false);
            msg.playerInfo._id = GameManager.GlobalData.PlayerID;

            NetManager.Send(msg);
        }

        /// <summary>
        /// 向服务器上传玩家信息以保存
        /// </summary>
        private void ReqPlayerInfoUpload()
        {
            HAMsgPlayerInfoUpload msg = new HAMsgPlayerInfoUpload();

            msg.playerInfo = _playerInfo;

            NetManager.Send(msg);
        }
        #endregion

        #region 监听方法
        /// <summary>
        /// 监听 ReqPlayerInfoLoad 返回消息
        /// </summary>
        private void RpsPlayerInfoLoad(MsgBase msgBase)
        {
            HAMsgPlayerInfoLoad msg = (HAMsgPlayerInfoLoad)msgBase;

            if (msg.result == 0)
            {
                HADebug.Log("[客户端] 角色信息获取成功!");
                _playerInfo = msg.playerInfo;
            }
            else
            {
                HADebug.LogWarning("[客户端] 角色信息获取失败，生成默认数据并进行存储");
                _playerInfo = new PlayerInfo(true);
                GameManager.Event.Broadcast(GameEventType.ReqPlayerInfoUpload);
            }

            // 分发数据至必须位置
            GameManager.Event.Broadcast<PlayerInfo>(GameEventType.UpdateInventoryItemList, _playerInfo);
        }

        /// <summary>
        /// 监听 ReqPlayerInfoUpload 返回消息
        /// </summary>
        private void RpsPlayerInfoUpload(MsgBase msgBase)
        {
            HAMsgPlayerInfoUpload msg = (HAMsgPlayerInfoUpload)msgBase;

            if (msg.result == 0)
            {
                Debug.Log("[客户端] 角色信息存储成功!");
            }
            else
            {
                Debug.LogError("[客户端] 角色信息存储失败");
            }
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 获得类型的仓库容量，目前只有 ItemNun 了
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
                case 2:
                    {
                        num = _playerInfo._inventoryEquipNum;
                    }
                    break;
                case 3:
                    {
                        num = _playerInfo._inventoryPotionNum;
                    }
                    break;
            }

            return num;
        }
        #endregion
    }
}
