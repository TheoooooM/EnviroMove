using System;
using System.Diagnostics;

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