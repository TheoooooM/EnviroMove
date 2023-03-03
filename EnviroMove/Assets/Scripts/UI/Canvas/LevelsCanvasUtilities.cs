using Archi.Service.Interface;
using Attributes;

namespace UI.Canvas
{
    public class LevelsCanvasUtilities : CanvasUtilities
    {
        [ServiceDependency] private IDataBaseService m_Data;
        [ServiceDependency] private ILevelService m_Level;
        
        public override void Init() { }
        
        public void LoadLevel(string levelName)
        {
            m_Level.LoadLevel(m_Data.GetLevelByName(levelName));
        }

    }
}
