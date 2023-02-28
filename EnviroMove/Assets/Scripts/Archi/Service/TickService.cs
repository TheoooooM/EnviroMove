using System;
using Archi.Service.Interface;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Archi.Service
{
    public class TickService : Service,ITickService
    {
        private bool timeState = false;
        private float lastTime;
        private float deltaTime;
        
        protected override void Initialize()
        {
            Update();
        }

        public async void Update()
        {
            lastTime = Time.time;
            while (currentServiceState && timeState)
            {
                OnUpdate?.Invoke();
                deltaTime = Time.time - lastTime;
                await UniTask.DelayFrame(0);
                
            }
        }
        public event Action OnUpdate;
        
        public void StopTime(bool time = true, bool update = true)
        {
            if(time) Time.timeScale = 0;
            if (update) timeState = false;
        }

        public void PlayTime()
        {
            if (timeState == false)
            {
                timeState = true;
                Update();
            }
        }

        public void ChangeTimeSpeed(float speedValue)
        {
            Time.timeScale = speedValue;
        }

        public void ResetTimeSpeed()
        {
            Time.timeScale = 1;
        }

        public override void SetServiceState(bool state)
        {
            base.SetServiceState(state);
            if(state)Update();
        }
    }
}