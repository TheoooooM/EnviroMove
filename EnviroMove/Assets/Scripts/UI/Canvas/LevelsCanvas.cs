using Archi.Service.Interface;
using Attributes;

namespace UI.Canvas
{
    public class LevelsCanvas : CanvasUtilities
    {
        private IDataBaseService m_data;
        [ServiceDependency] private ILevelService m_Level;
        
        public void LoadLevel(string levelName)
        {
            m_Level.LoadLevel(m_data.GetLevelByName(levelName));
        }
    }
}
