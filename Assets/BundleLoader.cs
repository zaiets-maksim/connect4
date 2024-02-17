using Connect4.Scripts.Services.BundleLoader;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BundleLoader : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private string _bundleName;
    [SerializeField] private string _assetName;  
    
    private IBundleLoader _bundleLoader;

    [Inject]
    public void Constructor(IBundleLoader bundleLoader)
    {
        _bundleLoader = bundleLoader;
    }
    
    private void Start()
    {
        var textures = _bundleLoader.LoadBundle<Texture2D>(_bundleName);
        var randomTexture = textures[Random.Range(0, textures.Count)];
        
        SetBackground(randomTexture);
        // SetBackground(_bundleLoader.LoadAsset<Texture2D>(_bundleName, _assetName));
    }

    private void SetBackground(Texture2D loadedTexture)
    {
        var image = Instantiate(loadedTexture, Vector3.zero, Quaternion.identity);
        Sprite sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.one * 0.5f);
            
        _background.sprite = sprite;
        _background.color = Color.white;
    }
}