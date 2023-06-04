using Archi.Service.Interface;
using Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LevelData = Levels.LevelData;
namespace UI.Canvas
{
    public class ToolUtilities : CanvasUtilities
    {
        [ServiceDependency] private IToolService m_Tool;
        [ServiceDependency] private IDataBaseService m_data;
        [ServiceDependency] private ILevelService m_level;
        [ServiceDependency] private IInterfaceService m_thisInterface;
        [SerializeField] private TMP_InputField inputField;
        [ServiceDependency] private IAudioService m_thisSound;

        private LevelData dataToTest;
        private AudioClip returnSound;
        private AudioClip clickSound;

        public override void Init()
        {
            AdresseHelper.LoadAssetWithCallback<AudioClip>("ReturnSound", clip => returnSound = clip);
            AdresseHelper.LoadAssetWithCallback<AudioClip>("ClickSound", clip => clickSound = clip);
        }
        
        public void LaunchTool()
        {    
            SceneManager.LoadScene("Tool");
            m_Tool.ShowTool();
        }

        public void ReturnButton() {
            // SceneManager.LoadScene(0);
            m_thisSound.PlaySound(returnSound);
            m_Tool.DesactivateTool();
        }
    
        public void ChangePrefab(int index)
        {
            m_thisSound.PlaySound(clickSound);
            m_Tool.ChangePrefab(index);
        }

        private LevelData GetEditorData()
        {
            throw new System.NotImplementedException();
        }

        public void SaveDataOnDevice()
        {
            m_thisSound.PlaySound(clickSound);
            var data = m_Tool.GetDataCreation();
            Debug.Log((string)data);
            m_data.GenerateDataLevel(data, inputField.text);
            //m_Tool.SaveData(inputField.text);
        }

        public void UpdateData()
        {
            m_thisSound.PlaySound(clickSound);
            var data = m_Tool.GetDataCreation();
            m_data.UpdateDataLevel((string)data, data.id);
        }

        public void SaveDataOnDataBase()
        {
            m_thisSound.PlaySound(clickSound);
            var data = m_Tool.GetDataCreation();
            if (data.id == null) data.id = m_data.GetUniqueIdentifier();
            m_data.CreateData((string)data, data.id);
        }
        
        public void SliderCamera(GameObject slider)
        {
            var value = slider.GetComponent<Slider>().value;
            if (value == null) value = 0;
            m_Tool.SliderCamera(value);
        }
        
        public void SwitchMode(int index)
        {
            m_thisSound.PlaySound(clickSound);
            m_Tool.SwitchMode(index);
        }
        
        public void TestLevel(string sceneName) {
            m_thisSound.PlaySound(clickSound);
            m_thisInterface.GenerateLoadingScreen("", 1, () => {
                dataToTest = m_Tool.GetDataCreation();
                SceneManager.sceneLoaded += AsyncTestLevel;
                m_Tool.DesactivateTool();
                ChangeScene(sceneName);
            });
            
            
            //throw new NotImplementedException();
            /*
            m_Tool.TestLevel();
            m_Tool.SetServiceState(false);
            SceneManager.sceneLoaded += (_,_) => m_Tool.TestLevel();*/
        }

        private void AsyncTestLevel(Scene arg0, LoadSceneMode arg1)
        {
            m_thisSound.PlaySound(clickSound);
            m_level.LoadLevel(dataToTest);
            SceneManager.sceneLoaded -= AsyncTestLevel;
            
        }


        public void ToggleLevelElements()
        {
            m_thisSound.PlaySound(clickSound);
            m_Tool.ToggleLevelElements();
        }

        public void ChangeCameraAngle()
        {
            m_thisSound.PlaySound(clickSound);
            m_Tool.ChangeCameraAngle();
        }
        
        public void SwapSeason()
        {
            m_thisSound.PlaySound(clickSound);
            m_Tool.SwapSeason();
        }
        
        public void PlaceGrass()
        {
            m_thisSound.PlaySound(clickSound);
            m_Tool.PlaceGrass();
        }
        
        public void PlaceCaillou()
        {
            m_thisSound.PlaySound(clickSound);
            m_Tool.PlaceCaillou();
        }
        
        public void PlaceBreakable()
        {
            m_thisSound.PlaySound(clickSound);
            m_Tool.PlaceBreakable();
        }
    }
}
