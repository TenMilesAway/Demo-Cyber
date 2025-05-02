using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cyber
{
    public class LoadingPanel : BasePanel
    {
        // 面板组件
        private Text txtLoad;
        private Slider sliderLoad;

        // 需要加载的下一个地图，需在外界赋值
        public Maps maps = Maps.Start;

        #region Unity 生命周期
        protected override void Start()
        {
            base.Start();

            StartCoroutine(LoadSceneAsych());
        }
        #endregion

        #region Init Methods
        protected override void InitUI()
        {
            txtLoad = GetControl<Text>("txtLoad");
            sliderLoad = GetControl<Slider>("sliderLoad");
        }
        #endregion

        #region Main Methods
        public IEnumerator LoadSceneAsych()
        {
            // 这里自定义需要 Load 的场景
            AsyncOperation ao = SceneManager.LoadSceneAsync((int)maps);

            // 这里做 Map 的逻辑
            GameDataMgr.GetInstance().isEnterNewMap = true;

            ao.allowSceneActivation = false;

            while (!ao.isDone)
            {
                sliderLoad.value = ao.progress;

                txtLoad.text = (sliderLoad.value * 100).ToString();

                if (ao.progress >= 0.89)
                {
                    sliderLoad.value = 100;
                    txtLoad.text = "100";
                    ao.allowSceneActivation = true;
                }

                yield return null;
            }

            UIManager.GetInstance().HidePanel("LoadingPanel");
        }
        #endregion
    }
}
