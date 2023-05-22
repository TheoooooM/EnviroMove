
    namespace Levels
    {
        public class LevelInfo
        {
            public string levelName;
            public string creator;
            public string levelFilePath;
            public string id;
            
            public int like;
            public int weekLike;
            
            public int carrotAmount;
            public int goldValue;

            public int difficulty;

            public int Season;

            public bool wasTrending;
            public bool wasDaysMap;

            public int timesPlay;

            public LevelInfo(string levelName, string id, string creator, string levelFilePath)
            {
                this.levelName = levelName;
                this.creator = creator;
                this.levelFilePath = levelFilePath;
                this.id = id;
            }
        }
    }