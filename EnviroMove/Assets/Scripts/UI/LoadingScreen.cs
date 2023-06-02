using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class LoadingScreen : MonoBehaviour {
        [SerializeField] private Slider slider;
        [SerializeField] private Animator cloudsAnim = null;
        private UnityAction action;

        public void SetLoader(string loadName,float maxValue, UnityAction action) {
            cloudsAnim.SetBool("Clouds", true);
            cloudsAnim.Play("CloudsToOn");
            slider.maxValue = maxValue;
            slider.value = 0;
            this.action = action;
        }

        public void UnLoadLoadingScreen() => cloudsAnim.SetBool("Clouds", false);
        public void DisableLoadingScreen() => StartCoroutine(WaitToDisable());

        private IEnumerator WaitToDisable() {
            yield return new WaitForSeconds(0.25f);
            gameObject.SetActive(false);
        }

        public void ApplyAction() => action.Invoke();
    }
}