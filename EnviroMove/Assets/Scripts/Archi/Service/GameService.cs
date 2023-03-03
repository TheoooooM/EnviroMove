using System;
using Archi.Service.Interface;
using Attributes;
using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Archi.Service
{
    public class GameService : Service, IGameService
    {
        [DependeOnService] private IInterfaceService m_Interface;
        [DependeOnService] private IDataBaseService m_Database;
        [DependeOnService] private ITickService m_Tick;
        
        
        protected override void Initialize()
        { }

        protected override void OnSceneInit(Scene scene, LoadSceneMode loadMode)
        { }

        public void OpenLevel(LevelData levelToOpen)
        {
            var levelGo = Object.Instantiate(new GameObject());
            var level = levelGo.AddComponent<Level>();
            level.GenerateLevel(levelToOpen);
            
            
        }
        
        #region Loading

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