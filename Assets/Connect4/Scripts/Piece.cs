using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    public Task MoveTo(Vector2 position, float duration)
    {
        TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
        transform.DOMove(position, duration).onComplete += () =>
            tcs.SetResult(null);
        
        return tcs.Task;
    }

    public void SetColor(Color color) => 
        _spriteRenderer.color = color;
}
