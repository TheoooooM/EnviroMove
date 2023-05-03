namespace Interfaces
{
    public interface IInteractable
    {
        bool IsInteractible(); 
   
        void Select();
        void Deselect(IBoardable releaseBoardable);
        void Swipe(Enums.Side side);
    }
}