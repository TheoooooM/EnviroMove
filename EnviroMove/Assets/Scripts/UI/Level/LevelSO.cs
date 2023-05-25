using UnityEngine;

[CreateAssetMenu(menuName = "Levels/Level Data SO")]
public class LevelSO : ScriptableObject {
    [SerializeField, TextArea(15,15)] private string levelData = "";
    public string LevelData => levelData;

    [SerializeField] private LevelSO nextLevel = null;
    public LevelSO Nextlevel => nextLevel;
}
