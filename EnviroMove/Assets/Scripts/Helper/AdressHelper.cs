using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AdresseHelper
{
    public static void LoadAssetWithCallback<T>(string adress, Action<T> callbackAction)
    {
        Debug.Log("Alala");
        var callback = Addressables.LoadAssetAsync<T>(adress);
        Debug.Log($"Callback : {callback.DebugName}");
        callback.Completed += (_) => OnLoadedAssetAsync(adress, _, callbackAction);
    }

    static void OnLoadedAssetAsync<T>(string key, AsyncOperationHandle<T> handle, Action<T> callbackAction)
    {
        Debug.Log("Assync Load");
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            callbackAction.Invoke(handle.Result);
        }
        else Debug.LogError($"Failed Trying to Async Load {key} item");
    }
}