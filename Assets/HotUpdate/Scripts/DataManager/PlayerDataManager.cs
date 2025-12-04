using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 玩家信息管理器
    /// </summary>
    public class PlayerDataManager : BaseManager<PlayerDataManager>
    {
        private PlayerInfo _playerInfo;

        public void Init()
        {
            NetManager.AddMsgListener("HAMsgPlayerInfoLoad", RpsPlayerInfoLoad);
            NetManager.AddMsgListener("HAMsgPlayerInfoUpload", RpsPlayerInfoUpload);

            ReqPlayerInfoLoad();
        }

        #region 主要方法
        /// <summary>
        /// 获取玩家信息
        /// </summary>
        public PlayerInfo GetPlayerInfo()
        {
            return _playerInfo;
        }

        public async Task<PlayerInfo> GetPlayerInfoAsync(int timeoutSeconds, float pollInterval = 1.0f)
        {
            var startTime = Time.time;

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
                ReqPlayerInfoUpload();
            }
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
    }
}
