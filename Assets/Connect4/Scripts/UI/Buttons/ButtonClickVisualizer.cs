using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Connect4.Scripts.UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickVisualizer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Range(0, 1)]
        [SerializeField] private float _coefficientScale = 0.97f;

        private Button _button;
        private Vector2 _buttonScale;

        private void Awake() => _button = GetComponent<Button>();
        public void OnPointerDown(PointerEventData eventData) => ZoomOutScale();
        public void OnPointerUp(PointerEventData eventData) => RevertScale();

        private void ZoomOutScale()
        {
            if(!IsActive())
                return;
            
            _buttonScale = new Vector2(transform.localScale.x, transform.localScale.y);
            transform.localScale = _buttonScale * _coefficientScale;
        }

        private void RevertScale()
        {
            if(!IsActive())
                return;
            
            transform.localScale = _buttonScale;
        }

        private bool IsActive() => _button.interactable;
    }
}