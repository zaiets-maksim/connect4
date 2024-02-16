using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private void Start() => CorrectedSize();

    public void AlignCamera(float offsetX, float offsetY, int width, int height) => 
        transform.position = new Vector3((width / 2f - 0.5f) * offsetX, -(height / 2f - 0.5f) * offsetY, -10f);

    private void CorrectedSize()
    {
        float standardAspectRatio = 1080f / 1920f;
        float currentAspectRatio = (float)Screen.width / Screen.height;

        float coefficient = standardAspectRatio / currentAspectRatio;
        _camera.orthographicSize *= coefficient;
    }
    
}
