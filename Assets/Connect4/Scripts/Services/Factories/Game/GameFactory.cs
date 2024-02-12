using UnityEngine;
using Zenject;

namespace Infrastructure.Services.Factories.Game
{
    public class GameFactory : Factory, IGameFactory
    {
        private const string GridCreatorPath = "Prefabs/Grid";
        private const string CellCreatorPath = "Prefabs/Cell";
        private const string ColumnCreatorPath = "Prefabs/Column";
        private const string PiecePath = "Prefabs/Piece";
        
        private readonly IGameCurator _gameCurator;
        private IGridService _gridService;

        public GameFactory(IInstantiator instantiator, IGameCurator gameCurator, IGridService gridService) : base(instantiator)
        {
            _gridService = gridService;
            _gameCurator = gameCurator;
        }
        
        public Piece CreatePiece(Vector2 position, Color color)
        {
            var prefab = InstantiateOnActiveScene(PiecePath);

            prefab.transform.position = position;
            var piece = prefab.GetComponent<Piece>();
            piece.SetColor(color);

            return piece;
        }
        
        public Cell CreateCell(int x, int y, float offsetX, float offsetY, float scale, Transform parent)
        {
            var prefab = InstantiateOnActiveScene(CellCreatorPath);
            prefab.transform.SetParent(parent);
            prefab.transform.localScale = Vector3.one * scale;
            prefab.transform.localPosition = new Vector3(y * offsetX, -x * offsetY, 0);
            prefab.name = $"{x} {y}";

            var cell = new Cell();
            cell.Initialize(new Vector2Int(x, y), prefab.transform.localPosition);

            return cell;
        }
        
        public void CreateColumn(int height, float offsetX, float offsetY, int index, GameObject parent)
        {
            var obj = InstantiateOnActiveScene(ColumnCreatorPath);
            obj.transform.SetParent(parent.transform);
            obj.transform.localPosition = new Vector3(index * offsetX, -(height / 2f - 0.5f) * offsetY, 0);
            obj.transform.localScale = new Vector3(0.92f, 1.095f * height, 0);

            var column = obj.GetComponent<Column>();
            column.Initialize(_gameCurator, index, height);
            _gridService.Columns.Add(column);
        }
        

        public void CreateGrid() => InstantiateOnActiveScene(GridCreatorPath);
    }
}