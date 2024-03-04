using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Commands;
using Connect4.Scripts.Field;

namespace Connect4.Scripts.Services.CommandHistoryService
{
    public class CommandHistoryService : ICommandHistoryService
    {
        private readonly Stack<ICommand> _commandHistory = new Stack<ICommand>();

        public void Push(ICommand command) => 
            _commandHistory.Push(command);

        public ICommand Peek() => 
            _commandHistory.Peek();

        public ICommand Pop() => 
            _commandHistory.Pop();

        public bool HasCommands() => 
            _commandHistory.Count > 0;
    
        public int CommandsCount() => 
            _commandHistory.Count;

        public List<ICommand> GetMovesBy(PlayerId playerId)
        {
            return new List<ICommand>(_commandHistory
                .Select(command => command))
                .Where(x => x.ActivePlayer.PlayerId == playerId).ToList();
        }

        public void Clear() => _commandHistory.Clear();
    }

    public interface ICommandHistoryService
    {
        void Push(ICommand command);
        ICommand Peek();
        ICommand Pop();
        bool HasCommands();
        int CommandsCount();
        List<ICommand> GetMovesBy(PlayerId playerId);
        // List<T> GetMovesBy<T>(PlayerId playerId) where T : ICommand;
        void Clear();
    }
}