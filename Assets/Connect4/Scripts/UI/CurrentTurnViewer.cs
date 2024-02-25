using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Connect4.Scripts.UI
{
    public class CurrentTurnViewer : MonoBehaviour
    {
        [SerializeField] private Text _text;
    
        public void Hide() => 
            _text.DOFade(0f, 0.3f);

        public void UpdateTurn(string text, Color color)
        {
            _text.text = text;
            UpdateColor(color);
        }

        private void UpdateColor(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            _text.color = Color.HSVToRGB(h, s * 0.75f, v);
        }
    }
}
