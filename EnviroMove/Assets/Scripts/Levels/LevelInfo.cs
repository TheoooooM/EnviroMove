
    namespace Levels
    {
        public class LevelInfo
        {
            public string levelName;
            public string creator;
            public string levelFilePath;
            public string id;

            public LevelInfo(string levelName, string id, string creator, string levelFilePath)
            {
                this.levelName = levelName;
                this.creator = creator;
                this.levelFilePath = levelFilePath;
                this.id = id;
            }
        }
    }