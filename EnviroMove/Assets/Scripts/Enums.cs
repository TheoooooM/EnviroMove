using System;
using System.Diagnostics;
using UnityEngine;

public class Enums
{
    public enum Side
    {
        none,
        forward,
        left,
        right,
        back,
        up,
        down
    }

    public static Side InverseSide(Side side)
    {
        switch (side)
        {
            case Side.forward: return Side.back;
            case Side.left: return Side.right;
            case Side.right: return Side.left;
            case Side.back: return Side.forward;
            case Side.up: return Side.down;
            case Side.down: return Side.up;
            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }

    public static Side[,,] IntToSideArray(int[,,] array)
    {
        Side[,,] sideArray = new Side[array.GetLength(0), array.GetLength(1), array.GetLength(2)];
        for (int _z = 0; _z < array.GetLength(2); _z++)
        {
            for (int _y = 0; _y < array.GetLength(1); _y++)
            {
                for (int _x = 0; _x < array.GetLength(0); _x++)
                {
                    sideArray[_x, _y, _z] = (Side)array[_x, _y, _z];
                }
            }
        }

        return sideArray;
    }

    public static int[,,] SideToIntArray(Side[,,] array)
    {
        int[,,] intArray = new int[array.GetLength(0), array.GetLength(1), array.GetLength(2)];
        for (int _z = 0; _z < array.GetLength(2); _z++)
        {
            for (int _y = 0; _y < array.GetLength(1); _y++)
            {
                for (int _x = 0; _x < array.GetLength(0); _x++)
                {
                    intArray[_x, _y, _z] = (int)array[_x, _y, _z];
                    /*if (array[_x, _y, _z] != Side.none) UnityEngine.Debug.Log("SideToIntArray: " + array[_x, _y, _z]);
                    else
                    {
                      UnityEngine.Debug.Log("Side is none");
                    }*/
                }
            }
        }

        return intArray;
    }

    public static Vector3[,,] SideToVector3Array(Side[,,] array)
    {
        Vector3[,,] vectorArray = new Vector3[array.GetLength(0), array.GetLength(1), array.GetLength(2)];
        for (int _z = 0; _z < array.GetLength(2); _z++)
        {
            for (int _y = 0; _y < array.GetLength(1); _y++)
            {
                for (int _x = 0; _x < array.GetLength(0); _x++)
                {
                    vectorArray[_x, _y, _z] = SideVector3(array[_x, _y, _z]);
                }
            }
        }

        return vectorArray;
    }

    public static Vector3 SideVector3(Side side)
    {
        switch (side)
        {
            case Side.none: return Vector3.zero;
            case Side.forward: return Vector3.forward;
            case Side.left: return Vector3.left;
            case Side.right: return Vector3.right;
            case Side.back: return Vector3.back;
            case Side.up: return Vector3.up;
            case Side.down: return Vector3.down;
            default: throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }

    public static Vector3Int SideVector3Int(Side side)
    {
        switch (side)
        {
            case Side.none: return Vector3Int.zero;
            case Side.forward: return Vector3Int.forward;
            case Side.left: return Vector3Int.left;
            case Side.right: return Vector3Int.right;
            case Side.back: return Vector3Int.back;
            case Side.up: return Vector3Int.up;
            case Side.down: return Vector3Int.down;
            default: throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }

    public enum Difficulty
    {
        Easy, 
        Medium,
        Hard
    }

    public enum Season
    {
        Spring,
        Autumn,
        Winter
    }

    public enum blockType
    {
        empty,      //0
        ground,     //1
        playerStart,//2
        playerEnd,  //3
        box,        //4
        breakableBlock, //5
        chariot,    //6
        frog,       //7
        ice,        //8
        penguin,    //9
        rabbit,     //10
        directionBlock, //11
        panelStart, //12
        panelEnd,   //13
        M1_Block1,  //14
        M1_Block2,  //15
        M1_Block3,  //16
        M1_Block4,  //17
        M1_Block5,  //18
        M1_Block6,  //19
        M1_Block7,  //20
        M1_Block8,  //21
        M1_Block9,  //22
        M1_Block10, //23
        M1_Block11, //24
        M1_Block12, //25
        M1_Block13, //26
        M1_Block14, //27
        M1_Block15, //28
        M1_Block16, //29
        M1_Block17, //30
        M1_Block18, //31
        M1_Border,  //32
        M2_Block1,  //33
        M2_Block2,  //34
        M2_Block3,  //35
        M2_Block4,  //36
        M2_Block5,  //37
        M2_Block6,  //38
        M2_Block7,  //39
        M2_Block8,  //40
        M2_Block9,  //41
        M2_Block10, //42
        M2_Block11, //43
        M2_Block12, //44
        M2_Block13, //45
        M2_Block14, //46
        M2_Block15, //47
        M2_Block16, //48
        M2_Block17, //49
        M2_Block18, //50
        M2_Block19, //51
        M1_Caillou, //52
        M1_Block19, //53
        M1_Block20, //54
        M1_Block21, //55
        M1_Block22, //56
        M1_Block23, //57
        M1_Block24, //58
        M1_Block25, //59
    }

    public enum MajorCanvas
    {
        menu,
        inGame,
        tool,
        toolLevels,
        levels,
        gameover
    }

    public enum BlockTag
    {
        Player,
        Penguin,
        FrogGrabbable
    }

    public enum SceneType
    {
        mainMenu,
        tool,
        levels,
        inGame
    }
}