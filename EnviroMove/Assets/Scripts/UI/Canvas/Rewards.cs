using UI.Canvas;
using UnityEngine;

public class Rewards : MonoBehaviour {
    [SerializeField] private MainMenuCanvasUtilities menu = null;
    [SerializeField] private Sprite rewardSprite = null;
    [SerializeField] private string rewardName = null;
    
    public void SetRewards() => menu.GetRewards(rewardSprite, rewardName);
}