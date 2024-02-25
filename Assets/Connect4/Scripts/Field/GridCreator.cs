using Connect4.Scripts.Services.Factories.Game;
using Connect4.Scripts.Services.GameCurator;
using Connect4.Scripts.Services.GridService;
using UnityEngine;
using Zenject;

namespace Connect4.Scripts.Field
{
    public class GridCreator : MonoBehaviour
    {
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private float _scale;
        [SerializeField] private SpriteRenderer _cell;

        private Cell[][] _grid;
    
        private IGridService _gridService;
        private IGameFactory _gameFactory;
        private IGameCurator _gameCurator;

        [Inject]
        public void Constructor(IGridService gridService, IGameFactory gameFactory, IGameCurator gameCurator)
        {
            _gameCurator = gameCurator;
            _gameFactory = gameFactory;
            _gridService = gridService;
        }

        private void Awake()
        {
            Create(_width, _height);
        }

        public void Create(int width, int height)
        {
            var offsetX = _cell.sprite.rect.width / 100f * _scale;
            var offsetY = _cell.sprite.rect.height / 100f * _scale;

            _grid = new Cell[height][];

            CreateCells(width, height, offsetX, offsetY);
            CreateColumns(width, height, offsetX, offsetY);
        
            _gridService.Initialization(_grid);
        
            _gameFactory.Camera.AlignCamera(offsetX, offsetY, width, height);
        }

        private void CreateCells(int width, int height, float offsetX, float offsetY)
        {
            for (var x = 0; x < height; x++)
            {
                _grid[x] = new Cell[width];

                for (int y = 0; y < width; y++)
                {
                    var cell = _gameFactory.CreateCell(x, y, offsetX, offsetY, _scale, transform);
                    _grid[x][y] = cell;
                }
            }
        }

        private void CreateColumns(int width, int height, float offsetX, float offsetY)
        {
            var columns = new GameObject("columns");
            columns.transform.SetParent(transform);
        
            for (int index = 0; index < width; index++) 
                _gameFactory.CreateColumn(height, offsetX, offsetY, index, columns, _gameCurator);
        }
    }
}
