using UnityEngine;

public class Column : MonoBehaviour
{
    private int _index;
    private int _lastElementIndex = 0;

    private IGameCurator _gameCurator;
    public int Index => _index;

    public int LastElementIndex
    {
        get => _lastElementIndex;
        private set => _lastElementIndex = value < 0 ? _lastElementIndex : value;
    }

    public bool HasFreeCell => LastElementIndex > 0;

    public void Initialize(IGameCurator gameCurator, int index, int lastElementIndex)
    {
        _gameCurator = gameCurator;
        _index = index;
        _lastElementIndex = lastElementIndex;
    }

    private void OnMouseUp()
    {
        if(CanTurn())
            DoTurn();
    }

    private bool CanTurn() => 
        LastElementIndex > 0 && _gameCurator.ActivePlayer.IsHuman() && _gameCurator.ActivePlayer.IsReady;

    private void DoTurn()
    {
        AddElement();
        _gameCurator.ActivePlayer.DoTurn(new Vector2Int(LastElementIndex, _index));
    }

    public void AddElement() => 
        --LastElementIndex;
    
    public void RemoveElement() => 
        ++LastElementIndex;
}
