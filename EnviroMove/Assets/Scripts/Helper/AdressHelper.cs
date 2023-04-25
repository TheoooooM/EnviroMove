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

    public static void LoadAssetWithCallbackIndexed<T>(string adress, Action<T, int> callbackAction, int index)
    {
        var callback = Addressables.LoadAssetAsync<T>(adress);
        callback.Completed += (_) =>
        {
            Debug.Log("address " + adress + " callbackAction " + callbackAction + " index " + index);
            OnLoadedAssetAsyncIndexed(adress, _, callbackAction, index);
        };
    }

    static void OnLoadedAssetAsyncIndexed<T>(string key, AsyncOperationHandle<T> handle, Action<T, int> callbackAction,
        int index)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            callbackAction.Invoke(handle.Result, index);
        }
        else Debug.LogError($"Failed Trying to Async Load {key} item");
    }
}