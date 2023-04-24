using System;
using System.Diagnostics;

public class Enums
{
  public enum Side
  {
    none, top, left, right, back
  }

  public static Side InverseSide(Side side)
  {
    switch (side)
    {
      case Side.top: return Side.back;
      case Side.left: return Side.right;
      case Side.right: return Side.left;
      case Side.back: return Side.top;
      default:
        throw new ArgumentOutOfRangeException(nameof(side), side, null);
    }
  }

  public enum blockType
  {
    empty, ground, playerStart, playerEnd, box, breakableBlock, chariot, frog, ice, penguin, rabbit, ground8
  }

  public enum MajorCanvas
  {
    menu, inGame, tool, toolLevels, levels
  }

  public enum BlockTag
  {
    Penguin, Player
  }

  public enum SceneType
  {
     mainMenu, tool, levels, inGame
  }
}