using UnityEngine;

namespace Connect4.Scripts.UI
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private CurrentTurnViewer _currentTurnViewer;
        public CurrentTurnViewer CurrentTurnViewer => _currentTurnViewer;
    }
}
