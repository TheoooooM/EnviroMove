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
        { Enums.blockType.ground1 , "groundBlock1"},
        { Enums.blockType.ground2 , "groundBlock2"},
        { Enums.blockType.ground3 , "groundBlock3"},
        { Enums.blockType.ground4 , "groundBlock4"},
    };
}
