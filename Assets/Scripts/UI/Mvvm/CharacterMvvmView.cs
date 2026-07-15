using HA;
using UnityEngine;
using UnityEngine.UI;

namespace MvvmTest
{
    public sealed class CharacterMvvmView : UIBasePanel
    {
        private Slider _hpBar;
        private Text _hpText;
        private Text _goldText;
        private Text _levelText;
        private Text _statusText;
        private Button _buyButton;
        private Button _damageButton;
        private Button _healButton;
        private Button _gainGoldButton;
        private Button _levelUpButton;
        private Button _resetButton;

        private CharacterMvvmViewModel _viewModel;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isInitialized;

        public void Initialize(
            Slider hpBar,
            Text hpText,
            Text goldText,
            Text levelText,
            Text statusText,
            Button buyButton,
            Button damageButton,
            Button healButton,
            Button gainGoldButton,
            Button levelUpButton,
            Button resetButton)
        {
            _hpBar = hpBar;
            _hpText = hpText;
            _goldText = goldText;
            _levelText = levelText;
            _statusText = statusText;
            _buyButton = buyButton;
            _damageButton = damageButton;
            _healButton = healButton;
            _gainGoldButton = gainGoldButton;
            _levelUpButton = levelUpButton;
            _resetButton = resetButton;

            if (_isInitialized)
            {
                return;
            }

            _buyButton.onClick.AddListener(OnBuyClicked);
            _damageButton.onClick.AddListener(OnDamageClicked);
            _healButton.onClick.AddListener(OnHealClicked);
            _gainGoldButton.onClick.AddListener(OnGainGoldClicked);
            _levelUpButton.onClick.AddListener(OnLevelUpClicked);
            _resetButton.onClick.AddListener(OnResetClicked);

            _isInitialized = true;
            SetButtonsInteractable(false);
        }

        public void Bind(CharacterMvvmViewModel viewModel)
        {
            DisposeSubscriptions();

            _viewModel = viewModel;
            _disposables.Add(_viewModel.HpRatio.Subscribe(value => _hpBar.value = value));
            _disposables.Add(_viewModel.HpText.Subscribe(value => _hpText.text = value));
            _disposables.Add(_viewModel.GoldText.Subscribe(value => _goldText.text = value));
            _disposables.Add(_viewModel.LevelText.Subscribe(value => _levelText.text = value));
            _disposables.Add(_viewModel.StatusText.Subscribe(value => _statusText.text = value));
            SetButtonsInteractable(true);
        }

        public void Dispose()
        {
            DisposeSubscriptions();
            _viewModel = null;
            SetButtonsInteractable(false);
        }

        protected override void OnDestroy() 
        {
            Dispose();

            if (!_isInitialized)
            {
                return;
            }

            _buyButton.onClick.RemoveListener(OnBuyClicked);
            _damageButton.onClick.RemoveListener(OnDamageClicked);
            _healButton.onClick.RemoveListener(OnHealClicked);
            _gainGoldButton.onClick.RemoveListener(OnGainGoldClicked);
            _levelUpButton.onClick.RemoveListener(OnLevelUpClicked);
            _resetButton.onClick.RemoveListener(OnResetClicked);
        }

        public override string GetPanelName()
        {
            return nameof(CharacterMvvmView);
        }

        private void OnBuyClicked()
        {
            _viewModel.Buy(100);
        }

        private void OnDamageClicked()
        {
            _viewModel.TakeDamage(15);
        }

        private void OnHealClicked()
        {
            _viewModel.Heal(10);
        }

        private void OnGainGoldClicked()
        {
            _viewModel.GainGold(200);
        }

        private void OnLevelUpClicked()
        {
            _viewModel.LevelUp();
        }

        private void OnResetClicked()
        {
            _viewModel.Reset();
        }

        private void DisposeSubscriptions()
        {
            _disposables.Dispose();
            _disposables = new CompositeDisposable();
        }

        private void SetButtonsInteractable(bool interactable)
        {
            _buyButton.interactable = interactable;
            _damageButton.interactable = interactable;
            _healButton.interactable = interactable;
            _gainGoldButton.interactable = interactable;
            _levelUpButton.interactable = interactable;
            _resetButton.interactable = interactable;
        }
    }
}
