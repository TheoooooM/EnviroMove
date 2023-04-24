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