namespace Archi.Service.Interface
{
    public interface IService
    {
        void SetServiceState(bool state);
        bool SwitchServiceState();
    }
}