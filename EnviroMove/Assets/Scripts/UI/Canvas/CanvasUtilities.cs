using Archi.Service.Interface;
using Attributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace UI.Canvas
{
    public abstract class CanvasUtilities : MonoBehaviour {
        [ServiceDependency] protected IInterfaceService m_Interface;
        [ServiceDependency] private IAudioService m_Audio;

        private AudioClip returnSound;
        private AudioClip clickSound;

        public virtual void Init()
        {
            AdresseHelper.LoadAssetWithCallback<AudioClip>("ReturnSound", clip => returnSound = clip);
            AdresseHelper.LoadAssetWithCallback<AudioClip>("ClickSound", clip => clickSound = clip);
        }

        public void ChangeScene(string sceneName) {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
        }

        protected void ChangeSceneWithClouds(string sceneName, IInterfaceService interfaceS) {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
            interfaceS.HideLoadingScreen();
        }

        public void SoundReturn()=>m_Audio.PlaySound(returnSound);

        public void ClickSound() => m_Audio.PlaySound(clickSound);
    }
}
