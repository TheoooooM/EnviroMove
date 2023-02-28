using System;
using Archi.Service.Interface;
using Attributes;
using Google.MiniJSON;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Archi.Service
{
    public class GameService : Service, IGameService
    {
        [DependeOnService] private IInterfaceService m_Interface;
        [DependeOnService] private ITickService m_Tick;
        
        
        protected override void Initialize()
        {
            m_Interface.DrawCanvas(Enums.MajorCanvas.menu);
        }

        #region Interface

        public void ChangeScene(Enums.SceneType type)
        {
            switch (type)
            {
                case Enums.SceneType.tool:
                    SceneManager.LoadScene("Tool");
                    m_Interface.DrawCanvas(Enums.MajorCanvas.tool);
                    break;
                case Enums.SceneType.mainMenu:
                    throw new NotImplementedException(); 
                    break;
                case Enums.SceneType.levels:
                    throw new NotImplementedException(); 
                    break;
                case Enums.SceneType.inGame:
                    throw new NotImplementedException(); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        
        public void CreateLoading()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateLoading()
        {
            throw new System.NotImplementedException();
        }

        public void FinishLoading()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}