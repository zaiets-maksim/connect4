using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    public Task MoveTo(Vector2 position, float duration)
    {
        TaskCompletionSource<Transform> tcs = new TaskCompletionSource<Transform>();
        transform.DOMove(position, duration).onComplete += () =>
            tcs.SetResult(null);
        
        return tcs.Task;
    }

    public async Task Hide()
    {
        TaskCompletionSource<Transform> tcs = new TaskCompletionSource<Transform>();
        _spriteRenderer.DOColor(Color.clear, 0.25f).onComplete += () =>
            tcs.SetResult(null);

        await tcs.Task;
        gameObject.SetActive(false);
    }

    public void Highlight()
    {
        
    }

    public void SetColor(Color color) => 
        _spriteRenderer.color = color;
}
