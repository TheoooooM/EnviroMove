using System.Collections.Generic;

public struct Blocks
{
    public static readonly Dictionary<Enums.blockType, string> strings = new Dictionary<Enums.blockType, string>
    {
        { Enums.blockType.ground , "groundBlock"},
        { Enums.blockType.wall , "wallBlock"},
        { Enums.blockType.moveBlock , "moveBlock"}
    };
}
