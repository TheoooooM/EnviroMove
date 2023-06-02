using UI.Canvas;
using UnityEngine;
using UnityEngine.UI;

public class Rewards : MonoBehaviour {
    [SerializeField] private MainMenuCanvasUtilities menu = null;
    [SerializeField] private RewardType RewardType = RewardType.Gold;
    [SerializeField] private string rewardName = "";
    [SerializeField] private int rewardValue = 0;
    [SerializeField] private int goldCost = 0;
    [SerializeField] private GameObject notBuyable = null;
    private Button btn = null;

    private void Awake() => btn = GetComponent<Button>();

    public void SetRewards() => menu.GetRewards(RewardType, rewardName, rewardValue, goldCost);

    private void Update() {
        if (goldCost == 0) return;

        if (RewardType == RewardType.Skin && PlayerPrefs.GetString("Skins", "0346").Contains($"{rewardValue}")) {
            btn.interactable = false;
            notBuyable.SetActive(true);
        }
        else {
            btn.interactable = PlayerPrefs.GetInt("Gold", 0) < goldCost;
            notBuyable.SetActive(PlayerPrefs.GetInt("Gold", 0) < goldCost);
        }
    }
}