using UI.Canvas;
using UnityEngine;
using UnityEngine.Serialization;

public class Rewards : MonoBehaviour {
    [SerializeField] private MainMenuCanvasUtilities menu = null;
    [SerializeField] private RewardType RewardType = RewardType.Gold;
    [SerializeField] private string rewardName = "";
    [SerializeField] private int rewardValue = 0;
    
    public void SetRewards() => menu.GetRewards(RewardType, rewardName, rewardValue);
}