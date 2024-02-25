using Connect4.Scripts.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace Connect4.Scripts.UI
{
    public class BackgroundLoader : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private string _bundleName;
        [SerializeField] private string _assetName;

        private void Start()
        {
            var textures = BundleLoader.LoadAssets<Texture2D>(_bundleName);
            var randomTexture = textures[Random.Range(0, textures.Count)];
        
            SetBackground(randomTexture);
            //SetBackground(BundleLoader.LoadAsset<Texture2D>(_bundleName, _assetName));
        }

        private void SetBackground(Texture2D loadedTexture)
        {
            var image = Instantiate(loadedTexture, Vector3.zero, Quaternion.identity);
            Sprite sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.one * 0.5f);
            
            _background.sprite = sprite;
            _background.color = Color.white;
        }
    }
}