using System.Collections.Generic;

public struct Blocks
{
    public static readonly Dictionary<Enums.blockType, string> BlockType = new()
    {
        { Enums.blockType.empty, "emptyBlock"},
        { Enums.blockType.ground , "groundBlock"},
        { Enums.blockType.playerStart , "playerStartBlock"},
        { Enums.blockType.playerEnd , "playerEndBlock"},
        { Enums.blockType.box , "box"},
        { Enums.blockType.breakableBlock , "breakableBlock"},
        { Enums.blockType.chariot , "chariot"},
        { Enums.blockType.frog , "frog"},
        { Enums.blockType.ice , "ice"},
        { Enums.blockType.penguin , "penguin"},
        { Enums.blockType.rabbit , "rabbit"},
        { Enums.blockType.directionBlock, "ground8"}
    };
    
    public static readonly Dictionary<string,Enums.blockType> BlockAdressType = new()
    {
        { "emptyBlock", Enums.blockType.empty},
        { "groundBlock", Enums.blockType.ground },
        { "playerStartBlock", Enums.blockType.playerStart},
        { "playerEndBlock", Enums.blockType.playerEnd},
        { "box", Enums.blockType.box},
        { "breakableBlock", Enums.blockType.breakableBlock},
        { "chariot", Enums.blockType.chariot},
        { "frog", Enums.blockType.frog},
        { "ice", Enums.blockType.ice},
        { "penguin", Enums.blockType.penguin},
        { "rabbit", Enums.blockType.rabbit},
        { "ground8", Enums.blockType.directionBlock}
    };
}
