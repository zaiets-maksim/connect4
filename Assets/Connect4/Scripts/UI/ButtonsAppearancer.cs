using System.Threading.Tasks;
using Connect4.Scripts.Infrastructure;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Connect4.Scripts.UI
{
    public class ButtonsAppearancer : MonoBehaviour
    {
        [SerializeField] private Button[] _buttons;
        [SerializeField] private Text[] _texts;

        private float _step = -150f;
        private float _height = 50;
    
        private ILoadingCurtain _loadingCurtain;

        [Inject]
        public void Constructor(ILoadingCurtain loadingCurtain)
        {
            _loadingCurtain = loadingCurtain;
        }

        private void OnEnable()
        {
            _loadingCurtain.OnComplete += AnimateButtons;
        }

        private void OnDisable()
        {
            _loadingCurtain.OnComplete -= AnimateButtons;
        }

        private void Start() => ResetButtons();

        private async void AnimateButtons()
        {
            var duration = 0.1f;
            var newColor = new Color(0.09f, 0.09f, 0.09f);
        
            for (var i = 0; i < _buttons.Length; i++) 
                await Show(_buttons[i], _texts[i], newColor, duration);
        }

        private async Task Show(Button button, Text text, Color color, float duration)
        {
            button.image.DOColor(color, duration);
            text.DOColor(Color.white, duration);
            button.transform.DOLocalMove(new Vector2(button.transform.localPosition.x, _height += _step), duration);

            await Task.Delay((int) (duration * 1000f));
        }

        private void ResetButtons()
        {
            for (var index = 0; index < _buttons.Length; index++)
            {
                _buttons[index].image.color = Color.clear;
                _texts[index].color = Color.clear;

                Vector3 newPos = _buttons[index].transform.localPosition;
                newPos.y = -750f;
                _buttons[index].transform.localPosition = newPos;
            }
        }
    }
}