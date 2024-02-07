using UnityEngine;

public class Column : MonoBehaviour
{
    private int _index;
    private int _lastElementIndex = 0;

    private IGameCurator _gameCurator;
    private IGridService _gridService;
    private IMoveVisualizer _moveVisualizer;

    public int LastElementIndex
    {
        get => _lastElementIndex;
        private set => _lastElementIndex = _lastElementIndex > 0 ? value : _lastElementIndex;
    }

    public void Initialize(IGameCurator gameCurator)
    {
        _gameCurator = gameCurator;
    }

    public void InitIndexes(int index, int lastElementIndex)
    {
        _index = index;
        _lastElementIndex = lastElementIndex;
    }

    private void OnMouseUp()
    {
        if(CanDoTurn())
            DoTurn();
    }

    private bool CanDoTurn() => 
        LastElementIndex > 0 && _gameCurator.ActivePlayer.IsReady;

    private async void DoTurn()
    {
        // _gameCurator.ActivePlayer.IsReady = false;
        //
        // Debug.Log("move");
        // await _moveVisualizer.ShowTurn(_gridService.GetCell(_lastElementIndex, _index).Position);
        // _gridService.TakeCell(_lastElementIndex, _index, _gameCurator.ActivePlayer.PlayerId);
        //
        // Debug.Log("end move");
        // _gameCurator.EndTurnHuman();

        --LastElementIndex;
        await _gameCurator.ActivePlayer.DoTurn(new Vector2Int(LastElementIndex, _index));
    }
}
