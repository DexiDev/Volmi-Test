namespace Game.Core
{
    public interface IBehaviour
    {
        public void Execute();

        public bool CanExecute();
    }
}