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
        { Enums.blockType.directionBlock, "ground8"},
        { Enums.blockType.panelStart, "panelStart"},
        { Enums.blockType.panelEnd, "panelEnd"},
        { Enums.blockType.M1_Block1, "M1_Block1"},          //14
        { Enums.blockType.M1_Block2, "M1_Block2"},          //15
        { Enums.blockType.M1_Block3, "M1_Block3"},          //16
        { Enums.blockType.M1_Block4, "M1_Block4"},          //17
        { Enums.blockType.M1_Block5, "M1_Block5"},          //18
        { Enums.blockType.M1_Block6, "M1_Block6"},          //19
        { Enums.blockType.M1_Block7, "M1_Block7"},          //20
        { Enums.blockType.M1_Block8, "M1_Block8"},          //21
        { Enums.blockType.M1_Block9, "M1_Block9"},          //22
        { Enums.blockType.M1_Block10, "M1_Block10"},        //23
        { Enums.blockType.M1_Block11, "M1_Block11"},        //24
        { Enums.blockType.M1_Block12, "M1_Block12"},        //25
        { Enums.blockType.M1_Block13, "M1_Block13"},        //26
        { Enums.blockType.M1_Block14, "M1_Block14"},        //27
        { Enums.blockType.M1_Block15, "M1_Block15"},        //28
        { Enums.blockType.M1_Block16, "M1_Block16"},        //29
        { Enums.blockType.M1_Block17, "M1_Block17"},        //30
        { Enums.blockType.M1_Block18, "M1_Block18"},        //31
        { Enums.blockType.M1_Border, "M1_Border"},          //32
        { Enums.blockType.M2_Block1, "M2_Block1"},          //33
        { Enums.blockType.M2_Block2, "M2_Block2"},          //34
        { Enums.blockType.M2_Block3, "M2_Block3"},          //35
        { Enums.blockType.M2_Block4, "M2_Block4"},          //36
        { Enums.blockType.M2_Block5, "M2_Block5"},          //37
        { Enums.blockType.M2_Block6, "M2_Block6"},          //38
        { Enums.blockType.M2_Block7, "M2_Block7"},          //39
        { Enums.blockType.M2_Block8, "M2_Block8"},          //40
        { Enums.blockType.M2_Block9, "M2_Block9"},          //41
        { Enums.blockType.M2_Block10, "M2_Block10"},        //42
        { Enums.blockType.M2_Block11, "M2_Block11"},        //43
        { Enums.blockType.M2_Block12, "M2_Block12"},        //44
        { Enums.blockType.M2_Block13, "M2_Block13"},        //45
        { Enums.blockType.M2_Block14, "M2_Block14"},        //46
        { Enums.blockType.M2_Block15, "M2_Block15"},        //47
        { Enums.blockType.M2_Block16, "M2_Block16"},        //48
        { Enums.blockType.M2_Block17, "M2_Block17"},        //49
        { Enums.blockType.M2_Block18, "M2_Block18"},        //50
        { Enums.blockType.M2_Block19, "M2_Block19"},        //51
        { Enums.blockType.M1_Caillou, "M1_Caillou"},        //52
        { Enums.blockType.M1_Block19, "M1_Block19"},        //53
        { Enums.blockType.M1_Block20, "M1_Block20"},        //54
        { Enums.blockType.M1_Block21, "M1_Block21"},        //55
        { Enums.blockType.M1_Block22, "M1_Block22"},        //56
        { Enums.blockType.M1_Block23, "M1_Block23"},        //57
        { Enums.blockType.M1_Block24, "M1_Block24"},        //58
        { Enums.blockType.M1_Block25, "M1_Block25"},        //59
        { Enums.blockType.M2_Block20A, "M2_Block20A"},      //60
        { Enums.blockType.M2_Block20B, "M2_Block20B"},      //61
        { Enums.blockType.M2_Block20A, "M2_Block21A"},      //60
        { Enums.blockType.M2_Block20B, "M2_Block21B"},      //62
        { Enums.blockType.M2_Block20A, "M2_Block22A"},      //63
        { Enums.blockType.M2_Block20B, "M2_Block22B"},      //64
        { Enums.blockType.M2_Block20A, "M2_Block23A"},      //65
        { Enums.blockType.M2_Block20B, "M2_Block23B"},      //66
        { Enums.blockType.M2_Block20A, "M2_Block24A"},      //67
        { Enums.blockType.M2_Block20B, "M2_Block24B"},      //68
        { Enums.blockType.M2_Block20A, "M2_Block25A"},      //69
        { Enums.blockType.M2_Block20B, "M2_Block25B"},      //70
        
        
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
        { "ground8", Enums.blockType.directionBlock},
        { "panelStart", Enums.blockType.panelStart},
        { "panelEnd", Enums.blockType.panelEnd},
        { "M1_Block1", Enums.blockType.M1_Block1},
        { "M1_Block2", Enums.blockType.M1_Block2},
        { "M1_Block3", Enums.blockType.M1_Block3},
        { "M1_Block4", Enums.blockType.M1_Block4},
        { "M1_Block5", Enums.blockType.M1_Block5},
        { "M1_Block6", Enums.blockType.M1_Block6},
        { "M1_Block7", Enums.blockType.M1_Block7},
        { "M1_Block8", Enums.blockType.M1_Block8},
        { "M1_Block9", Enums.blockType.M1_Block9},
        { "M1_Block10", Enums.blockType.M1_Block10},
        { "M1_Block11", Enums.blockType.M1_Block11},
        { "M1_Block12", Enums.blockType.M1_Block12},
        { "M1_Block13", Enums.blockType.M1_Block13},
        { "M1_Block14", Enums.blockType.M1_Block14},
        { "M1_Block15", Enums.blockType.M1_Block15},
        { "M1_Block16", Enums.blockType.M1_Block16},
        { "M1_Block17", Enums.blockType.M1_Block17},
        { "M1_Block18", Enums.blockType.M1_Block18},
        { "M1_Border", Enums.blockType.M1_Border},
        { "M2_Block1", Enums.blockType.M2_Block1},
        { "M2_Block2", Enums.blockType.M2_Block2},
        { "M2_Block3", Enums.blockType.M2_Block3},
        { "M2_Block4", Enums.blockType.M2_Block4},
        { "M2_Block5", Enums.blockType.M2_Block5},
        { "M2_Block6", Enums.blockType.M2_Block6},
        { "M2_Block7", Enums.blockType.M2_Block7},
        { "M2_Block8", Enums.blockType.M2_Block8},
        { "M2_Block9", Enums.blockType.M2_Block9},
        { "M2_Block10", Enums.blockType.M2_Block10},
        { "M2_Block11", Enums.blockType.M2_Block11},
        { "M2_Block12", Enums.blockType.M2_Block12},
        { "M2_Block13", Enums.blockType.M2_Block13},
        { "M2_Block14", Enums.blockType.M2_Block14},
        { "M2_Block15", Enums.blockType.M2_Block15},
        { "M2_Block16", Enums.blockType.M2_Block16},
        { "M2_Block17", Enums.blockType.M2_Block17},
        { "M2_Block18", Enums.blockType.M2_Block18},
        { "M2_Block19", Enums.blockType.M2_Block19},
        { "M1_Caillou", Enums.blockType.M1_Caillou},
        { "M1_Block19", Enums.blockType.M1_Block19},
        { "M1_Block20", Enums.blockType.M1_Block20},
        { "M1_Block21", Enums.blockType.M1_Block21},
        { "M1_Block22", Enums.blockType.M1_Block22},
        { "M1_Block23", Enums.blockType.M1_Block23},
        { "M1_Block24", Enums.blockType.M1_Block24},
        { "M1_Block25", Enums.blockType.M1_Block25},
        { "M2_Block20A", Enums.blockType.M2_Block20A},
        { "M2_Block20B", Enums.blockType.M2_Block20B},
        { "M2_Block21A", Enums.blockType.M2_Block21A},
        { "M2_Block21B", Enums.blockType.M2_Block21B},
        { "M2_Block22A", Enums.blockType.M2_Block22A},
        { "M2_Block22B", Enums.blockType.M2_Block22B},
        { "M2_Block23A", Enums.blockType.M2_Block23A},
        { "M2_Block23B", Enums.blockType.M2_Block23B},
        { "M2_Block24A", Enums.blockType.M2_Block24A},
        { "M2_Block24B", Enums.blockType.M2_Block24B},
        { "M2_Block25A", Enums.blockType.M2_Block25A},
        { "M2_Block25B", Enums.blockType.M2_Block25B},
    };
}
