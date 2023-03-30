using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshPro LoadName;
        [SerializeField] private Slider slider;

        public void SetLoader(string loadName,float maxValue)
        {
            LoadName.text = loadName;
            slider.maxValue = maxValue;
            slider.value = 0;
        }
    }
}