using Infrastructure.Services.Factories.Game;
using UnityEngine;
using Zenject;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private float _scale;
    [SerializeField] private SpriteRenderer _cell;

    private Cell[][] _grid;
    
    private IGridService _gridService;
    private IGameFactory _gameFactory;


    [Inject]
    public void Constructor(IGridService gridService, IGameFactory gameFactory)
    {
        _gameFactory = gameFactory;
        _gridService = gridService;
    }

    private void Start()
    {
        Create(_width, _height);
    }

    public void Create(int width, int height)
    {
        var offsetX = _cell.sprite.rect.width / 100f * _scale;
        var offsetY = _cell.sprite.rect.height / 100f * _scale;

        _grid = new Cell[height][];

        for (var x = 0; x < height; x++)
        {
            _grid[x] = new Cell[width];

            for (int y = 0; y < width; y++)
            {
                var cell = _gameFactory.CreateCell(x, y, offsetX, offsetY, _scale, transform);
                _grid[x][y] = cell;
            }
        }

        CreateColumns(width, height, offsetX, offsetY);
        
        _gridService.Initialization(_grid);
        
        AlignCamera(offsetX, offsetY);
    }

    private void CreateColumns(int width, int height, float offsetX, float offsetY)
    {
        var columns = new GameObject("columns");
        columns.transform.SetParent(transform);
        
        for (int index = 0; index < width; index++) 
            _gameFactory.CreateColumn(height, offsetX, offsetY, index, columns);
    }

    private void AlignCamera(float offsetX, float offsetY) => // to camera
        Camera.main.transform.position = new Vector3((_width / 2f - 0.5f) * offsetX, -(_height / 2f - 0.5f) * offsetY, -10f);
}
