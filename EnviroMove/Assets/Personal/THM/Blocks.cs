using System.Collections.Generic;

public struct Blocks
{
    public static readonly Dictionary<Enums.blockType, string> BlockType = new()
    {
        { Enums.blockType.empty, "emptyBlock"},
        { Enums.blockType.ground , "groundBlock"},
        { Enums.blockType.wall , "wallBlock"},
        { Enums.blockType.moveBlock , "moveBlock"},
        { Enums.blockType.playerStart , "playerStartBlock"},
        { Enums.blockType.playerEnd , "playerEndBlock"},
        { Enums.blockType.ground1 , "groundBlockOne"},
        { Enums.blockType.ground2 , "groundBlockTwo"},
        { Enums.blockType.groundThree , "groundBlockThree"},
        { Enums.blockType.ground4 , "groundBlockFour"},
    };
    
    public static readonly Dictionary<string,Enums.blockType> BlockAdressType = new()
    {
        { "emptyBlock", Enums.blockType.empty},
        { "groundBlock", Enums.blockType.ground },
        { "wallBlock", Enums.blockType.wall },
        { "moveBlock", Enums.blockType.moveBlock },
        { "playerStartBlock", Enums.blockType.playerStart},
        { "playerEndBlock", Enums.blockType.playerEnd},
        { "groundBlockOne", Enums.blockType.ground1},
        { "groundBlockTwo", Enums.blockType.ground2 },
        { "groundBlockThree", Enums.blockType.groundThree},
        { "groundBlockFour", Enums.blockType.ground4 },
    };
}
