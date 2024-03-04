using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Connect4.Scripts.Field
{
    public class Piece : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public async Task MoveTo(Vector2 position, float duration)
        {
            transform.DOMove(position, duration);
            await Task.Delay((int)(duration * 1000f));
            
            // Task task = transform.DOMove(position, duration).AsyncWaitForCompletion();
            // await task;
        }

        public async Task Hide(float duration)
        {
            _spriteRenderer.DOColor(Color.clear, duration);
            await Task.Delay((int)(duration * 1000f));
            
            gameObject.SetActive(false);
        }

        public void SetColor(Color color) => 
            _spriteRenderer.color = color;
    }
}
