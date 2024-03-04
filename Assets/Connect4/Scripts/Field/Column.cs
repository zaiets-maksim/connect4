using System;
using Connect4.Scripts.Services.GameCurator;
using UnityEngine;

namespace Connect4.Scripts.Field
{
    public class Column : MonoBehaviour
    {
        private int _index;
        private int _lastElementIndex;

        private IGameCurator _gameCurator;
        
        public event Action<Vector2Int> OnClick;
        
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
            if (HumanCanTurn())
            {
                TakeElement();
                OnClick?.Invoke(new Vector2Int(LastElementIndex, _index));
            }
        }

        private bool HumanCanTurn() => 
            LastElementIndex > 0 && _gameCurator.ActivePlayer.IsHuman() && _gameCurator.ActivePlayer.IsReady;

        public void TakeElement() => 
            --LastElementIndex;
    
        public void ReleaseElement() => 
            ++LastElementIndex;
    }
}
