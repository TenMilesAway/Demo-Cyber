using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Cyber
{
    public partial class Service : BaseManager<Service>
    {
        // SyncPlayerMovementStateMachine 的反射
        private Type t = typeof(SyncPlayerMovementStateMachine);

        // 临时 SyncPlayerMovementStateMachine
        private SyncPlayerMovementStateMachine tempSyncStateMachine;

        #region 客户端消息发送的方法
        // 在每次进入新地图的时候加载一次
        // 在服务端接收此消息时，会把消息分发给所有客户端
        public void UpdateSyncPlayer()
        {
            GameDataMgr.GetInstance().syncPlayers.Clear();

            MsgUpdatePlayerEntities msg = new MsgUpdatePlayerEntities();

            NetManager.Send(msg);

            GameDataMgr.GetInstance().isEnterNewMap = false;
        }
        #endregion

        #region 客户端监听方法
        /// <summary>
        /// 监听消息，同步玩家的创建
        /// </summary>
        /// <param name="msgBase"></param>
        public void OnMsgUpdatePlayerEntities(MsgBase msgBase)
        {
            MsgUpdatePlayerEntities msg = (MsgUpdatePlayerEntities)msgBase;

            foreach (PlayerInfo playerInfo in msg.list)
            {
                // 判断是否是角色
                if (playerInfo.id == GameDataMgr.GetInstance().id) continue;
                // 如果字典中已存在该角色
                if (GameDataMgr.GetInstance().syncPlayers.ContainsKey(playerInfo.id)) continue;

                GameObject syncPlayer = ResMgr.GetInstance().Load<GameObject>("Prefabs/SyncPlayer/SyncPlayer");

                GameDataMgr.GetInstance().syncPlayers.Add(playerInfo.id, syncPlayer.GetComponent<SyncPlayer>());
            }
        }

        /// <summary>
        /// 监听消息，同步玩家下线
        /// </summary>
        /// <param name="msgBase"></param>
        public void OnMsgPlayerDisconnect(MsgBase msgBase)
        {
            MsgPlayerDisconnect msg = (MsgPlayerDisconnect)msgBase;

            SyncPlayer syncPlayer = GameDataMgr.GetInstance().syncPlayers[msg.id];

            UnityEngine.Object.Destroy(syncPlayer.gameObject);

            GameDataMgr.GetInstance().syncPlayers.Remove(msg.id);
        }

        /// <summary>
        /// 监听消息，更新当前地图同步玩家的位置和状态
        /// </summary>
        /// <param name="msgBase"></param>
        public void OnMsgUpdatePlayerTempInfo(MsgBase msgBase)
        {
            MsgUpdatePlayerTempInfo msg = (MsgUpdatePlayerTempInfo)msgBase;

            // 拿到所有玩家的 tempInfos
            foreach (PlayerTempInfo tempInfo in msg.tempInfos)
            {
                // 判断是否是角色
                if (tempInfo.id == GameDataMgr.GetInstance().id) continue;

                Transform syncPlayer = GameDataMgr.GetInstance().syncPlayers[tempInfo.id].transform;
                syncPlayer.position = new Vector3(tempInfo.x, tempInfo.y, tempInfo.z);
                syncPlayer.eulerAngles = new Vector3(tempInfo.rx, tempInfo.ry, tempInfo.rz);

                // 利用反射改变同步角色的状态机
                tempSyncStateMachine = GameDataMgr.GetInstance().syncPlayers[tempInfo.id].movementStateMachine;
                PropertyInfo info = t.GetProperty(tempInfo.state);
                GameDataMgr.GetInstance().syncPlayers[tempInfo.id].movementStateMachine.ChangeState(info.GetValue(tempSyncStateMachine) as IState);
            }
        }
        #endregion
    }
}
