using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Shine : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    
    private readonly Vector3 _targetRotation = new Vector3(0f, 0f, -360f);
    private readonly float _rotationDuration = 4f;
    
    public void Play(Vector2 position, Color color)
    {
        transform.localScale = Vector3.one * 0.8f;
        transform.position = position;
        _spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);

        color.a = 0.5f;
        Color.RGBToHSV(color, out float h, out float s, out float v);
        var targetColor = Color.HSVToRGB(h, s * 0.7f, v);
        
        transform.DOScale(Vector3.one * 0.4f, 0.75f);
        _spriteRenderer.DOColor(targetColor, 0.5f);
        
        transform.DORotate(_targetRotation, _rotationDuration, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        
        // _spriteRenderer.DOFade(0.35f, 2f).onComplete += () =>
        //     _spriteRenderer.DOFade(0.7f, 2f).SetLoops(-1).onComplete += () =>
        //         transform.DoLoops(-1);

        // StartCoroutine(PlaySequentialAnimations());
    }
    
    IEnumerator PlaySequentialAnimations()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            yield return _spriteRenderer.DOFade(0.5f, 0.4f).WaitForCompletion();
            yield return _spriteRenderer.DOFade(1f, 1f).WaitForCompletion();
        }
    }
}
