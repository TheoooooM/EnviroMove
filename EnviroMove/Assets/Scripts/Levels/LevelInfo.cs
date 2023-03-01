
    namespace Levels
    {
        public class LevelInfo
        {
            public string levelName;
            public string creator;
            public string levelFilePath;

            public LevelInfo(string levelName, string creator, string levelFilePath)
            {
                this.levelName = levelName;
                this.creator = creator;
                this.levelFilePath = levelFilePath;
            }
        }
    }