using Archi.Service.Interface;
using Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Canvas
{
    public abstract class CanvasUtilities : MonoBehaviour {
        [ServiceDependency] protected IInterfaceService m_Interface;
        
         public abstract void Init();

        public void ChangeScene(string sceneName) {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
        }

        protected void ChangeSceneWithClouds(string sceneName, IInterfaceService interfaceS) {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
            interfaceS.HideLoadingScreen();
        }
    }
}
