using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Task MoveTo(Vector2 position, float duration)
    {
        TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
        transform.DOMove(position, duration).onComplete += () =>
            tcs.SetResult(null);
        return tcs.Task;
    }
}
