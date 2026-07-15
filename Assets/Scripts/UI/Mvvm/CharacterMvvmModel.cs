using UnityEngine;

namespace MvvmTest
{
    /// <summary>
    /// MVVM Model 数据类
    /// 只涉及数值的加减, 不涉及业务逻辑
    /// </summary>
    public sealed class CharacterMvvmModel
    {
        private readonly int initialMaxHp;
        private readonly int initialGold;
        private readonly int initialLevel;

        public CharacterMvvmModel(int maxHp = 100, int gold = 500, int level = 1)
        {
            initialMaxHp = maxHp;
            initialGold = gold;
            initialLevel = level;

            MaxHp = new ReactiveProperty<int>(initialMaxHp);
            Hp = new ReactiveProperty<int>(initialMaxHp);
            Gold = new ReactiveProperty<int>(initialGold);
            Level = new ReactiveProperty<int>(initialLevel);
        }

        public ReactiveProperty<int> Hp { get; }

        public ReactiveProperty<int> MaxHp { get; }

        public ReactiveProperty<int> Gold { get; }

        public ReactiveProperty<int> Level { get; }

        public bool TrySpend(int cost)
        {
            if (Gold.Value < cost)
            {
                return false;
            }

            Gold.Value -= cost;
            return true;
        }

        public void TakeDamage(int damage)
        {
            Hp.Value = Mathf.Max(0, Hp.Value - damage);
        }

        public void Heal(int amount)
        {
            Hp.Value = Mathf.Min(MaxHp.Value, Hp.Value + amount);
        }

        public void GainGold(int amount)
        {
            Gold.Value += amount;
        }

        public void LevelUp()
        {
            Level.Value += 1;
            MaxHp.Value += 20;
            Hp.Value = MaxHp.Value;
        }

        public void Reset()
        {
            MaxHp.Value = initialMaxHp;
            Hp.Value = initialMaxHp;
            Gold.Value = initialGold;
            Level.Value = initialLevel;
        }
    }
}
