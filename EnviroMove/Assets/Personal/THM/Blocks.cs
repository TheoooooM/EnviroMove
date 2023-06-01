using System.Collections.Generic;
using static Enums.blockType;

public struct Blocks
{
    public static readonly Dictionary<Enums.blockType, string> BlockType = new()
    {
        { empty, "emptyBlock" },
        { ground, "groundBlock" },
        { playerStart, "playerStartBlock" },
        { playerEnd, "playerEndBlock" },
        { box, "box" },
        { breakableBlock, "breakableBlock" },
        { chariot, "chariot" },
        { frog, "frog" },
        { ice, "ice" },
        { penguin, "penguin" },
        { rabbit, "rabbit" },
        { directionBlock, "ground8" },
        { panelStart, "panelStart" },
        { panelEnd, "panelEnd" },
        { M1_Block1, "M1_Block1" }, //14
        { M1_Block2, "M1_Block2" }, //15
        { M1_Block3, "M1_Block3" }, //16
        { M1_Block4, "M1_Block4" }, //17
        { M1_Block5, "M1_Block5" }, //18
        { M1_Block6, "M1_Block6" }, //19
        { M1_Block7, "M1_Block7" }, //20
        { M1_Block8, "M1_Block8" }, //21
        { M1_Block9, "M1_Block9" }, //22
        { M1_Block10, "M1_Block10" }, //23
        { M1_Block11, "M1_Block11" }, //24
        { M1_Block12, "M1_Block12" }, //25
        { M1_Block13, "M1_Block13" }, //26
        { M1_Block14, "M1_Block14" }, //27
        { M1_Block15, "M1_Block15" }, //28
        { M1_Block16, "M1_Block16" }, //29
        { M1_Block17, "M1_Block17" }, //30
        { M1_Block18, "M1_Block18" }, //31
        { M1_Border, "M1_Border" }, //32
        { M2_Block1, "M2_Block1" }, //33
        { M2_Block2, "M2_Block2" }, //34
        { M2_Block3, "M2_Block3" }, //35
        { M2_Block4, "M2_Block4" }, //36
        { M2_Block5, "M2_Block5" }, //37
        { M2_Block6, "M2_Block6" }, //38
        { M2_Block7, "M2_Block7" }, //39
        { M2_Block8, "M2_Block8" }, //40
        { M2_Block9, "M2_Block9" }, //41
        { M2_Block10, "M2_Block10" }, //42
        { M2_Block11, "M2_Block11" }, //43
        { M2_Block12, "M2_Block12" }, //44
        { M2_Block13, "M2_Block13" }, //45
        { M2_Block14, "M2_Block14" }, //46
        { M2_Block15, "M2_Block15" }, //47
        { M2_Block16, "M2_Block16" }, //48
        { M2_Block17, "M2_Block17" }, //49
        { M2_Block18, "M2_Block18" }, //50
        { M2_Block19, "M2_Block19" }, //51
        { M1_Caillou, "M1_Caillou" }, //52
        { M1_Block19, "M1_Block19" }, //53
        { M1_Block20, "M1_Block20" }, //54
        { M1_Block21, "M1_Block21" }, //55
        { M1_Block22, "M1_Block22" }, //56
        { M1_Block23, "M1_Block23" }, //57
        { M1_Block24, "M1_Block24" }, //58
        { M1_Block25, "M1_Block25" }, //59
        { M2_Block20A, "M2_Block20A" }, //60
        { M2_Block20B, "M2_Block20B" }, //61
        { M2_Block21A, "M2_Block21A" }, //62
        { M2_Block21B, "M2_Block21B" }, //63
        { M2_Block22A, "M2_Block22A" }, //64
        { M2_Block22B, "M2_Block22B" }, //65
        { M2_Block23A, "M2_Block23A" }, //66
        { M2_Block23B, "M2_Block23B" }, //67
        { M2_Block24A, "M2_Block24A" }, //68
        { M2_Block24B, "M2_Block24B" }, //69
        { M2_Block25A, "M2_Block25A" }, //70
        { M2_Block25B, "M2_Block25B" }, //71
        { M3_Block1, "M3_Block1" }, //72
        { M3_Block2, "M3_Block2" }, //73
        { M3_Block3, "M3_Block3" }, //74
        { M3_Block4, "M3_Block4" }, //75
        { M3_Block5, "M3_Block5" }, //76
        { M3_Block6, "M3_Block6" }, //77
        { M3_Block7, "M3_Block7" }, //78
        { M3_Block8, "M3_Block8" }, //79
        { M3_Block9, "M3_Block9" }, //80
        { M3_Block10, "M3_Block10" }, //81  
        { M3_Block11, "M3_Block11" }, //82  
        { M3_Block12, "M3_Block12" }, //83  
        { M3_Block13, "M3_Block13" }, //84  
        { M3_Block14, "M3_Block14" }, //85  
        { M3_Block15, "M3_Block15" }, //86  
        { M3_Block16, "M3_Block16" }, //87  
        { M3_Block17, "M3_Block17" }, //88  
        { M3_Block18, "M3_Block18" }, //89  
        { M3_Block19, "M3_Block19" }, //90  
        { M3_Block20, "M3_Block20" }, //91  
        { M3_Block21, "M3_Block21" }, //92  
        { M3_Block22, "M3_Block22" }, //93  
        { M3_Block23, "M3_Block23" }, //94  
        { M3_Block24, "M3_Block24" }, //95  
        { M3_Block25, "M3_Block25" }, //96  
        { M3_Block26, "M3_Block26" }, //97  
        { OutBottomLeftCorner, "Box555" },      //98
        { OutTopLeftCorner, "Box685" },         //99
        { OutBottomRightCorner, "Box686" },     //100
        { OutTopRightCorner, "Box687" },        //101
        { InsideBottomLeftCorner, "Box736" },   //102
        { InsideBottomRightCorner, "Box737" },  //103
        { InsideTopRightCorner, "Box738" },     //104
        { InsideTopLeftCorner, "Box739" },      //105
        { OutLeft1, "Box688" },                 //106
        { OutLeft2, "Box689" },
        { OutLeft3, "Box690" },
        { OutLeft4, "Box779" },
        { OutLeft5, "Box692" },
        { OutLeft6, "Box794" },
        { OutLeft7, "Box694" },
        { OutLeft8, "Box780" },
        { OutLeft9, "Box696" },
        { OutLeft10, "Box793" },
        { OutLeft11, "Box698" },
        { OutLeft12, "Box778" },
        { OutLeft13, "Box699" },
        { OutLeft14, "Box705" },
        { OutRight1, "Box719" },                //120
        { OutRight2, "Box713" },
        { OutRight3, "Box796" },
        { OutRight4, "Box712" },
        { OutRight5, "Box717" },
        { OutRight6, "Box777" },
        { OutRight7, "Box716" },
        { OutRight8, "Box797" },
        { OutRight9, "Box791" },
        { OutRight10, "Box709" },
        { OutRight11, "Box776" },
        { OutRight12, "Box708" },
        { OutRight13, "Box707" },
        { OutRight14, "Box706" },
        { OutTop1, "Box720" },                  //134
        { OutTop2, "Box721" },
        { OutTop3, "Box722" },
        { OutTop4, "Box792" },
        { OutTop5, "Box798" },
        { OutTop6, "Box725" },
        { OutTop7, "Box726" },
        { OutTop8, "Box727" },
        { OutBottom1, "Box735" },               //142
        { OutBottom2, "Box734" },
        { OutBottom3, "Box795" },
        { OutBottom4, "Box799" },
        { OutBottom5, "Box781" },
        { OutBottom6, "Box730" },
        { OutBottom7, "Box729" },
        { OutBottom8, "Box728" },
        { InsideTop1, "Box740" },               //150
        { InsideTop2, "Box782" },
        { InsideTop3, "Box749" },
        { InsideTop4, "Box750" },
        { InsideTop5, "Box751" },
        { InsideTop6, "Box743" },
        { InsideLeft1, "Box748" },              //156
        { InsideLeft2, "Box761" },
        { InsideLeft3, "Box760" },
        { InsideLeft4, "Box784" },
        { InsideLeft5, "Box758" },
        { InsideLeft6, "Box789" },
        { InsideLeft7, "Box756" },
        { InsideLeft8, "Box755" },
        { InsideLeft9, "Box754" },
        { InsideLeft10, "Box783" },
        { InsideLeft11, "Box752" },
        { InsideLeft12, "Box742" },
        { InsideRight1, "Box745" },             //168
        { InsideRight2, "Box766" },
        { InsideRight3, "Box767" },
        { InsideRight4, "Box787" },
        { InsideRight5, "Box769" },
        { InsideRight6, "Box770" },
        { InsideRight7, "Box771" },
        { InsideRight8, "Box772" },
        { InsideRight9, "Box788" },
        { InsideRight10, "Box774" },
        { InsideRight11, "Box790" },
        { InsideRight12, "Box744" },
        { InsideBottom1, "Box747" },            //181
        { InsideBottom2, "Box785" },
        { InsideBottom3, "Box763" },
        { InsideBottom4, "Box764" },
        { InsideBottom5, "Box786" },
        { InsideBottom6, "Box746" },            //186
        { empty2, "emptyBlock2" },
        {SM_BorderVfinal_Roof, "SM_BorderVfinal_Roof"},
        {SM_BorderVfinal_Inside , "SM_BorderVfinal_Inside"},
    };

    public static readonly Dictionary<string, Enums.blockType> BlockAdressType = new()
    {
        { "emptyBlock", empty },
        { "groundBlock", ground },
        { "playerStartBlock", playerStart },
        { "playerEndBlock", playerEnd },
        { "box", box },
        { "breakableBlock", breakableBlock },
        { "chariot", chariot },
        { "frog", frog },
        { "ice", ice },
        { "penguin", penguin },
        { "rabbit", rabbit },
        { "ground8", directionBlock },
        { "panelStart", panelStart },
        { "panelEnd", panelEnd },
        { "M1_Block1", M1_Block1 },
        { "M1_Block2", M1_Block2 },
        { "M1_Block3", M1_Block3 },
        { "M1_Block4", M1_Block4 },
        { "M1_Block5", M1_Block5 },
        { "M1_Block6", M1_Block6 },
        { "M1_Block7", M1_Block7 },
        { "M1_Block8", M1_Block8 },
        { "M1_Block9", M1_Block9 },
        { "M1_Block10", M1_Block10 },
        { "M1_Block11", M1_Block11 },
        { "M1_Block12", M1_Block12 },
        { "M1_Block13", M1_Block13 },
        { "M1_Block14", M1_Block14 },
        { "M1_Block15", M1_Block15 },
        { "M1_Block16", M1_Block16 },
        { "M1_Block17", M1_Block17 },
        { "M1_Block18", M1_Block18 },
        { "M1_Border", M1_Border },
        { "M2_Block1", M2_Block1 },
        { "M2_Block2", M2_Block2 },
        { "M2_Block3", M2_Block3 },
        { "M2_Block4", M2_Block4 },
        { "M2_Block5", M2_Block5 },
        { "M2_Block6", M2_Block6 },
        { "M2_Block7", M2_Block7 },
        { "M2_Block8", M2_Block8 },
        { "M2_Block9", M2_Block9 },
        { "M2_Block10", M2_Block10 },
        { "M2_Block11", M2_Block11 },
        { "M2_Block12", M2_Block12 },
        { "M2_Block13", M2_Block13 },
        { "M2_Block14", M2_Block14 },
        { "M2_Block15", M2_Block15 },
        { "M2_Block16", M2_Block16 },
        { "M2_Block17", M2_Block17 },
        { "M2_Block18", M2_Block18 },
        { "M2_Block19", M2_Block19 },
        { "M1_Caillou", M1_Caillou },
        { "M1_Block19", M1_Block19 },
        { "M1_Block20", M1_Block20 },
        { "M1_Block21", M1_Block21 },
        { "M1_Block22", M1_Block22 },
        { "M1_Block23", M1_Block23 },
        { "M1_Block24", M1_Block24 },
        { "M1_Block25", M1_Block25 },
        { "M2_Block20A", M2_Block20A },
        { "M2_Block20B", M2_Block20B },
        { "M2_Block21A", M2_Block21A },
        { "M2_Block21B", M2_Block21B },
        { "M2_Block22A", M2_Block22A },
        { "M2_Block22B", M2_Block22B },
        { "M2_Block23A", M2_Block23A },
        { "M2_Block23B", M2_Block23B },
        { "M2_Block24A", M2_Block24A },
        { "M2_Block24B", M2_Block24B },
        { "M2_Block25A", M2_Block25A },
        { "M2_Block25B", M2_Block25B },
        { "emptyBlock2" , empty2},
        { "SM_BorderVfinal_Roof", SM_BorderVfinal_Roof}, 
        { "SM_BorderVfinal_Inside", SM_BorderVfinal_Inside},
    };
}