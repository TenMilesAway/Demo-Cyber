namespace MvvmTest
{
    public sealed class CharacterMvvmViewModel
    {
        private readonly CharacterMvvmModel _model;

        public CharacterMvvmViewModel(CharacterMvvmModel model)
        {
            _model = model;

            HpRatio = new ReactiveProperty<float>();
            HpText = new ReactiveProperty<string>(string.Empty);
            GoldText = new ReactiveProperty<string>(string.Empty);
            LevelText = new ReactiveProperty<string>(string.Empty);
            StatusText = new ReactiveProperty<string>("就绪：可以点击下面按钮测试 MVVM 自动刷新。");

            model.Hp.Subscribe(_ => UpdateHpPresentation());
            model.MaxHp.Subscribe(_ => UpdateHpPresentation());
            model.Gold.Subscribe(gold => GoldText.Value = string.Format("Gold: {0}", gold));
            model.Level.Subscribe(level => LevelText.Value = string.Format("Level: {0}", level));
        }

        public ReactiveProperty<float> HpRatio { get; }

        public ReactiveProperty<string> HpText { get; }

        public ReactiveProperty<string> GoldText { get; }

        public ReactiveProperty<string> LevelText { get; }

        public ReactiveProperty<string> StatusText { get; }

        public void Buy(int cost)
        {
            if (_model.TrySpend(cost))
            {
                StatusText.Value = string.Format("购买成功，消耗 {0} Gold。", cost);
            }
            else
            {
                StatusText.Value = string.Format("购买失败，Gold 不足 {0}。", cost);
            }
        }

        public void TakeDamage(int damage)
        {
            _model.TakeDamage(damage);
            StatusText.Value = string.Format("受到 {0} 点伤害。", damage);
        }

        public void Heal(int amount)
        {
            _model.Heal(amount);
            StatusText.Value = string.Format("恢复 {0} 点生命。", amount);
        }

        public void GainGold(int amount)
        {
            _model.GainGold(amount);
            StatusText.Value = string.Format("获得 {0} Gold。", amount);
        }

        public void LevelUp()
        {
            _model.LevelUp();
            StatusText.Value = "角色升级，生命上限提升并回满生命。";
        }

        public void Reset()
        {
            _model.Reset();
            StatusText.Value = "已重置为初始状态。";
        }

        private void UpdateHpPresentation()
        {
            var maxHp = _model.MaxHp.Value;
            var hp = _model.Hp.Value;
            HpRatio.Value = maxHp <= 0 ? 0f : (float)hp / maxHp;
            HpText.Value = string.Format("HP: {0} / {1}", hp, maxHp);
        }
    }
}
