using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Musashi
{
    public class SceneLoder : MonoBehaviour
    {
        [SerializeField] Image fadeImage;
        [SerializeField] float fadeTime = 120f;

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
                GameManager.Instance.ShowConfigure();
                return;
            }

            if (buildIndex == SceneInBuildIndex.CloseConfigure)
            {
                GameManager.Instance.CloseConfigure();
                return;
            }
            StartCoroutine(LoadSceneAsync((int)buildIndex, callback));
        }

        IEnumerator LoadSceneAsync(int buildIndex, UnityAction callback = null)
        {
            fadeImage.color = Color.clear;
            fadeImage.enabled = true;
            float timer = 0;

            //FadeOutImage
            while (true)
            {
                timer += Time.deltaTime / fadeTime;
                fadeImage.color = Color.Lerp(fadeImage.color, Color.black, timer);
                if (fadeImage.color.a > 0.99f)
                {
                    break;
                }
                yield return null;
            }


            //LoadScene
            AsyncOperation async = SceneManager.LoadSceneAsync(buildIndex);

            while (async.progress < 0.99f)
            {
                yield return null;
            }
            fadeImage.enabled = false;
            async.allowSceneActivation = true;
            callback?.Invoke();
        }

        public void LoadAddtiveScene(SceneInBuildIndex buildIndex)
        {
            SceneManager.LoadScene((int)buildIndex, LoadSceneMode.Additive);
        }

        public void UnLoadScene(SceneInBuildIndex buildIndex)
        {
            SceneManager.UnloadSceneAsync((int)buildIndex);
        }
    }
}
