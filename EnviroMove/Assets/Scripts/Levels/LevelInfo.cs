
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

            public int season;

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

            public LevelInfo(string levelName, string creator, string id, int like, int weekLike, int carrotAmount,
                int goldValue, int difficulty, int season, bool wasTrending, bool wasDaysMap, int timesPlay)
            {
                this.levelName = levelName;
                this.creator = creator;
                this.id = id;
                this.like = like;
                this.weekLike = weekLike;
                this.carrotAmount = carrotAmount;
                this.goldValue = goldValue;
                this.difficulty = difficulty;
                this.season = season;
                this.wasTrending = wasTrending;
                this.wasDaysMap = wasDaysMap;
                this.timesPlay = timesPlay;
            }
        }
    }