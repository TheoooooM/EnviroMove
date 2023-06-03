using System;
using System.Collections.Generic;
using Archi.Service.Interface;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Archi.Service
{
    public class SoundService : Service, ISoundService
    {
        private List<AudioListener> _listeners = new();

        protected override void Initialize()
        {
            var go = Object.Instantiate(new GameObject());
            Object.DontDestroyOnLoad(go);
            for (int i = 0; i < 3; i++)
            {
                var listener = go.AddComponent<AudioListener>();
                _listeners.Add(listener);
            }
        }
    }
}