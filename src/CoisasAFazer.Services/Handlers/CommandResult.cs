namespace CoisasAFazer.Services.Handlers
{
    public class CommandResult
    {
        public bool IsSucess { get;}

        public CommandResult(bool isSucess)
        {
            IsSucess = isSucess;
        }
    }
}