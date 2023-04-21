namespace Interfaces
{
    public interface IInteractable
    {
        bool IsInteractible(); 
   
        void Select();
        void Deselect();
        void Swipe(Enums.Side side);
    }
}