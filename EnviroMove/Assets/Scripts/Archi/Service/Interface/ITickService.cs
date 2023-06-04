using System;

namespace Archi.Service.Interface
{
    public interface ITickService : IService
    {
        event Action OnUpdate;
        
        void StopTime(bool time=true, bool update=true);
        void PlayTime();
        void ChangeTimeSpeed(float speedValue);
        void ResetTimeSpeed();
    }
}