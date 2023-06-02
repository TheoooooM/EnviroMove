using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class LoadScreenHelper : MonoBehaviour {
    [SerializeField] private LoadingScreen loadScreen = null;

    public void DisableLoadingScreen() => loadScreen.DisableLoadingScreen();
    public void ApplyAction() => loadScreen.ApplyAction();
}
