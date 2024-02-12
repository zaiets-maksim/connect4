using System.Collections.Generic;
using System.Linq;

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

    // public List<T> GetMovesBy<T>(PlayerId playerId) where T : ICommand
    // {
    //     return new Stack<T>(_commandHistory
    //             .Select(command => (T) command))
    //         .Where(x => x.ActivePlayer.PlayerId == playerId).ToList();
    // }
    
    public List<ICommand> GetMovesBy(PlayerId playerId)
    {
        return new List<ICommand>(_commandHistory
                .Select(command => command))
            .Where(x => x.ActivePlayer.PlayerId == playerId).ToList();
    }
}

public interface ICommandHistoryService
{
    void Push(ICommand command);
    ICommand Peek();
    ICommand Pop();
    bool HasCommands();
    List<ICommand> GetMovesBy(PlayerId playerId);
    // List<T> GetMovesBy<T>(PlayerId playerId) where T : ICommand;
}
