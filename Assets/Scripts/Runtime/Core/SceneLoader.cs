using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Musashi
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] Image fadeImage;
        [SerializeField] float fadeDuration = 2f;

        public int GetActiveSceneBuildIndex
        {
            get => SceneManager.GetActiveScene().buildIndex;
        }


        private void Start()
        {
            fadeImage.enabled = false;
        }

        public void LoadScene(SceneInBuildIndex buildIndex, UnityAction callback = null)
        {
            if (buildIndex == SceneInBuildIndex.Exit)
            {
                GameManager.Instance.ExitGame();
                return;
            }

            if (buildIndex == SceneInBuildIndex.ShowConfigure)
            {
                GameManager.Instance.Configure.Show();
                return;
            }

            if (buildIndex == SceneInBuildIndex.CloseConfigure)
            {
                GameManager.Instance.Configure.Close();
                return;
            }
            LoadSceneAsync((int)buildIndex, callback);
        }

        private void LoadSceneAsync(int buildIndex, UnityAction callback = null)
        {
            fadeImage.enabled = true;
            fadeImage.color = Color.clear;
            //fadeOut
            DOTween.To(() => fadeImage.color, (x) => fadeImage.color = x, Color.black, fadeDuration)
               .OnComplete(async() =>
               {
                   await SceneManager.LoadSceneAsync(buildIndex);
                   callback?.Invoke();
                   fadeImage.enabled = false;
               });
        }
  
        public void LoadAddtiveScene(SceneInBuildIndex buildIndex)
        {
            SceneManager.LoadScene((int)buildIndex, LoadSceneMode.Additive);
        }

        public void UnLoadScene(SceneInBuildIndex buildIndex)
        {
            SceneManager.UnloadSceneAsync((int)buildIndex);
        }


        /// <summary>
        /// シーンをただ、フェードイン、フェードアウトする関数。シーンロードしない
        /// </summary>
        /// <param name="fadetype"></param>
        /// <param name="duration"></param>
        /// <param name="callback"></param>
        public void FadeScreen(FadeType fadetype, float duration,bool endImageEnabled = false , UnityAction callback = null)
        {
            fadeImage.Fade(fadetype, duration,endImageEnabled ,callback);
        }
        
    }
}
