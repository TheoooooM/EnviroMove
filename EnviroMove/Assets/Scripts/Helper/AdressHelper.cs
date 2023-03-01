using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AdresseHelper
{
    public static void LoadAssetWithCallback<T>(string adress, Action<T> callbackAction)
    {
        var callback = Addressables.LoadAssetAsync<T>(adress);
        callback.Completed += (_) => OnLoadedAssetAsync(adress, _, callbackAction);
    }

    static void OnLoadedAssetAsync<T>(string key, AsyncOperationHandle<T> handle, Action<T> callbackAction)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            callbackAction.Invoke(handle.Result);
        }
        else Debug.LogError($"Failed Trying to Async Load {key} item");
    }
}