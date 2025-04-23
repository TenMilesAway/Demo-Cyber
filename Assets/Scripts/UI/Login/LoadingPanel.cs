using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cyber
{
    public class LoadingPanel : BasePanel
    {
        public string id = "";

        public Image imgBack;
        public Text txtLoad;
        public Slider sliderLoad;

        private void Start()
        {
            imgBack = GetControl<Image>("imgBack");
            txtLoad = GetControl<Text>("txtLoad");
            sliderLoad = GetControl<Slider>("sliderLoad");

            StartCoroutine(LoadSceneAsych());
        }

        public IEnumerator LoadSceneAsych()
        {
            // 这里自定义需要 Load 的场景
            AsyncOperation ao = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

            GameDataMgr.GetInstance().isEnterNewMap = true;

            ao.allowSceneActivation = false;

            while (!ao.isDone)
            {
                sliderLoad.value = ao.progress * 100;

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
    }
}
