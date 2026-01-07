using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HA
{
    public class LoadingPanelParam : OpenUIParam
    {
        public string _name;
        public string _state;
    }

    public class LoadingPanel : UIBasePanel
    {
        [SerializeField] private Slider _sliderLoad;
        [SerializeField] private Text _txtLoad;

        private string _nextState;
        private string _sceneName;
        private string _stateName;
        private bool _isLoading;
        private bool _isLoaded;
        private bool _isChanged;

        public override string GetPanelName()
        {
            return GlobalDefine.LoadingPanel;
        }

        private void Update()
        {
            // 开始加载后，但未加载完成
            if (_isLoading && !_isLoaded)
            {
                RefreshProgress();
            }

            // 加载完成
            if (_isLoaded)
            {
                ChangeScene();
            }
        }

        protected override async void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            _isBlockingWindow = false;
            _isLoading = false;
            _isLoaded = false;
            _isChanged = false;

            LoadingPanelParam loadingPanelParam = param as LoadingPanelParam;
            _sceneName = loadingPanelParam._name;
            _nextState = loadingPanelParam._state;

            _txtLoad.text = "0";
            _sliderLoad.value = 0;

            AsyncOperationHandle<SceneInstance> asyncOperation = Addressables.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
            _isLoading = true;

            await asyncOperation.Task;

            SceneManager.SetActiveScene(asyncOperation.Result.Scene);
            _isLoaded = true;
        }

        #region 主要方法
        /// <summary>
        /// 进度条更新
        /// </summary>
        /// <param name="value"></param>
        private void RefreshProgress(float value = 0.005f)
        {
            if (_sliderLoad.value >= 0.98f) return;
            _sliderLoad.value += value;
            _txtLoad.text = Mathf.FloorToInt(_sliderLoad.value * 100).ToString();
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        private void ChangeScene()
        {
            if (_isChanged) return;

            _isChanged = true;

            // 创建动画序列
            Sequence progressSequence = DOTween.Sequence();
            progressSequence.Append(_sliderLoad.DOValue(1f, 2.0f).SetEase(Ease.OutQuad));
            progressSequence.Join(DOTween.To(() => _sliderLoad.value * 100,
                x => _txtLoad.text = Mathf.FloorToInt(x).ToString(),
                100, 2.0f).SetEase(Ease.OutQuad));
            progressSequence.OnComplete(() =>
            {
                GameManager.Fsm.StartFsmState(_nextState);

                UIManager.GetInstance().ClosePanel(GetPanelName());
            });
        }
        #endregion
    }
}
