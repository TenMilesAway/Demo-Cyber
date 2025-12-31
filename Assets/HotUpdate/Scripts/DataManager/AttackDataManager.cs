using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 用于计算攻击造成伤害的单例类
    /// </summary>
    public class AttackDataManager : BaseManager<AttackDataManager>
    {
        /// <summary>
        /// 玩家对敌方造成伤害
        /// </summary>
        public int CalculatePlayerAttack(PlayerInfo playerInfo, EnemyData enemyData)
        {
            int attack = 0;

            // 先用破甲值计算敌方实际防御力
            int trueDefense = enemyData._pDefense * (1 - playerInfo._pArmorPenetration);

            // 计算本次攻击是否触发暴击
            bool isCriticalAttack = IsCritialAttack(playerInfo._pCriticalProbability);

            // 攻击力减去实际防御力
            int trueAttack = 0;
            if (isCriticalAttack) trueAttack = (int)(playerInfo._pAttack * playerInfo._pCriticalMultiplier) - trueDefense;
            else trueAttack = playerInfo._pAttack - trueDefense;

            // 再通过敌方免伤值计算最终伤害
            attack = (int)(trueAttack * (1 - enemyData.DamageAvoidance));

            // 计算本次攻击是否触发吸血
            if (IsSuckAttack(playerInfo._pSuckProbability))
            {
                int suckHP = (int)(attack * playerInfo._pSuckMultiplier);
                // 广播
            }

            return attack;
        }

        /// <summary>
        /// 敌方对玩家造成伤害
        /// </summary>
        public int CalculateEnemyAttack(EnemyData enemyData, PlayerInfo playerInfo)
        {
            int attack = 0;

            // 先用破甲值计算玩家实际防御力
            int trueDefense = (int)(playerInfo._pDefense * (1 - enemyData.ArmorPenetration));

            // 攻击力减去实际防御力
            int trueAttack = enemyData._pAttack - trueDefense;

            // 再通过玩家免伤值计算最终伤害
            attack = trueAttack * (1 - playerInfo._pDamageAvoidance);

            return attack;
        }


        #region 辅助方法：计算暴击和吸血
        /// <summary>
        /// 暴击计算
        /// </summary>
        private bool IsCritialAttack(float criticalProbability)
        {
            int randomInt = Random.Range(0, 100);
            if (criticalProbability > randomInt) return true;
            else return false;
        }

        private bool IsSuckAttack(float suckProbability)
        {
            int randomInt = Random.Range(0, 100);
            if (suckProbability > randomInt) return true; 
            else return false;
        }
        #endregion
    }
}
