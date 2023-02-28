using Archi.Service.Interface;
using Attributes;
using UnityEngine;

namespace UI
{
    public class CanvasUtilities : MonoBehaviour
    {
        [ServiceDependency]public IGameService m_Game;
    
        public void ChangeScene(int sceneIndex)
        {
            m_Game.ChangeScene((Enums.SceneType)sceneIndex);
        }
    }
}
