using System;
using System.Diagnostics;
using UnityEngine;

public class Enums
{
  public enum Side
  {
    none, forward, left, right, back, up, down
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

  public enum blockType
  {
    empty, ground, playerStart, playerEnd, box, breakableBlock, chariot, frog, ice, penguin, rabbit, directionBlock, panelStart, panelEnd,
    M1_Block1, M1_Block2, M1_Block3, M1_Block4, M1_Block5, M1_Block6, M1_Block7, M1_Block8, M1_Block9, M1_Block10, M1_Block11, M1_Block12,
    M1_Block13, M1_Block14, M1_Block15, M1_Block16, M1_Block17, M1_Block18
  }

  public enum MajorCanvas
  {
    menu, inGame, tool, toolLevels, levels, gameover
  }

  public enum BlockTag
  {
    Player,Penguin , FrogGrabbable
  }

  public enum SceneType
  {
     mainMenu, tool, levels, inGame
  }
}