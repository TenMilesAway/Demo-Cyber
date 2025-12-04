using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HA
{
    /// <summary>
    /// UI 层级
    /// </summary>
    public enum UILayer
    { 
        Bot,
        Mid,
        Top,
        System,
    }

    /// <summary>
    /// UI 管理器
    /// </summary>
    public class UIManager : BaseManager<UIManager>
    {
        private Dictionary<string, UIBasePanel> _panelDic = new Dictionary<string, UIBasePanel>();
        private List<string> _loadingPanels = new List<string>();

        private const string _canvasPath = "Assets/UI/Canvas/Prefabs/Canvas.prefab";
        private const string _eventSystemPath = "Assets/UI/Canvas/Prefabs/EventSystem.prefab";
        private const float _waitDestoryTime = 20f;

        private Transform _bot;
        private Transform _mid;
        private Transform _top;
        private Transform _system;

        private GameObject _canvasPrefab;
        private GameObject _eventSystemPrefab;

        public RectTransform _canvas;

        /// <summary>
        /// 初始化 Canvas 和 EventSystem
        /// </summary>
        public async Task Init()
        {
            // 初始化面板
            AsyncOperationHandle canvasHandle = Addressables.LoadAssetAsync<GameObject>(_canvasPath);
            await canvasHandle.Task;
            _canvasPrefab = canvasHandle.Result as GameObject;
            GameObject _canvasGO = GameObject.Instantiate(_canvasPrefab);
            _canvas = _canvasGO.transform as RectTransform;
            GameObject.DontDestroyOnLoad(_canvasGO);
            
            // 初始化事件系统
            AsyncOperationHandle eventSystemHandle = Addressables.LoadAssetAsync<GameObject>(_eventSystemPath);
            await eventSystemHandle.Task;
            _eventSystemPrefab = eventSystemHandle.Result as GameObject;
            GameObject _eventSystemGO = GameObject.Instantiate(_eventSystemPrefab);
            GameObject.DontDestroyOnLoad(_eventSystemGO);

            // 各层
            _bot = _canvas.Find("Bot");
            _mid = _canvas.Find("Mid");
            _top = _canvas.Find("Top");
            _system = _canvas.Find("System");
        }

        /// <summary>
        /// 打开 UI 面板 (目前未走定时逻辑, 后续修改)
        /// </summary>
        /// <param name="panelName">AA 路径</param>
        /// <param name="layer">UI 层级</param>
        /// <param name="param">透传参数</param>
        /// <param name="action">回调函数</param>
        /// <returns></returns>
        public async void OpenPanel(string panelName, UILayer layer = UILayer.Mid, OpenUIParam param = null, Action action = null)
        {
            // 如果此面板正在加载
            if (_loadingPanels.Contains(panelName)) return;

            _loadingPanels.Add(panelName);

            // 如果字典中存在此面板
            if (_panelDic.ContainsKey(panelName))
            {
                UIBasePanel panel = _panelDic[panelName];

                GetPanelCompletedLogic(panelName, panel, param, action);

                return;
            }

            // 字典中不存在此面板, 从 AA 中加载
            AsyncOperationHandle panelHandle = Addressables.LoadAssetAsync<GameObject>(panelName);

            await panelHandle.Task;

            GameObject panelGO = GameObject.Instantiate(panelHandle.Result as GameObject);

            // 设置父对象, 设置相对位置和大小
            switch (layer)
            {
                case UILayer.Bot:
                    panelGO.transform.SetParent(_bot);
                    break;
                case UILayer.Mid:
                    panelGO.transform.SetParent(_mid);
                    break;
                case UILayer.Top:
                    panelGO.transform.SetParent(_top);
                    break;
                case UILayer.System:
                    panelGO.transform.SetParent(_system);
                    break;
            }
            panelGO.transform.localPosition = Vector3.zero;
            panelGO.transform.localScale = Vector3.one;
            (panelGO.transform as RectTransform).offsetMax = Vector2.zero;
            (panelGO.transform as RectTransform).offsetMin = Vector2.zero;

            UIBasePanel panelComponent = panelGO.GetComponent<UIBasePanel>();

            GetPanelCompletedLogic(panelName, panelComponent, param, action);

            _panelDic.Add(panelName, panelComponent);
        }

        /// <summary>
        /// 处理获取面板后的打开逻辑
        /// </summary>
        private void GetPanelCompletedLogic(string panelName, UIBasePanel panel, OpenUIParam param, Action action)
        {
            panel.OnInit(param);

            panel.OnShow();

            if (action != null) action();

            _loadingPanels.Remove(panelName);
        }

        /// <summary>
        /// 关闭面板 (目前未走定时逻辑, 后续修改)
        /// </summary>
        public void ClosePanel(string panelName)
        {
            if (_panelDic.ContainsKey(panelName))
            {
                _panelDic[panelName].OnClose();
                GameObject.Destroy(_panelDic[panelName].gameObject);
                _panelDic.Remove(panelName);
            }
        }
    }
}
