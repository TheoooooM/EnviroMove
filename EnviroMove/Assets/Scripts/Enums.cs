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

  public enum blockType
  {
    empty, ground, playerStart, playerEnd, box, breakableBlock, chariot, frog, ice, penguin, rabbit, directionBlock
  }

  public enum MajorCanvas
  {
    menu, inGame, tool, toolLevels, levels
  }

  public enum BlockTag
  {
    Player,Penguin 
  }

  public enum SceneType
  {
     mainMenu, tool, levels, inGame
  }
}