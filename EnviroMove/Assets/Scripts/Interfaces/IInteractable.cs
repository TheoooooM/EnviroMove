namespace Interfaces
{
    public interface IInteractable
    {
        void Select();
        void Deselect();
        void Swipe(Enums.Side side);
    }
}