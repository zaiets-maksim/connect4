using Connect4.Scripts.Field;
using Connect4.Scripts.Services.GameCurator;
using Connect4.Scripts.Services.GridService;
using Connect4.Scripts.UI;
using UnityEngine;
using Zenject;

namespace Connect4.Scripts.Services.Factories.Game
{
    public class GameFactory : Factory, IGameFactory
    {
        private const string GridCreatorPath = "Prefabs/Grid";
        private const string CellCreatorPath = "Prefabs/Cell";
        private const string ColumnCreatorPath = "Prefabs/Column";
        private const string PiecePath = "Prefabs/Piece";
        private const string HudPath = "Prefabs/UI/Hud";
        private const string ShinePath = "Prefabs/Shine";
        private const string CameraPath = "Prefabs/MainCamera";

        
        private readonly IGridService _gridService;
        
        public Hud Hud { get; private set; }
        public CameraScript Camera { get; private set; }

        public GameFactory(IInstantiator instantiator, IGridService gridService) : base(instantiator)
        {
            _gridService = gridService;
        }
        
        public CameraScript CreateCamera() => 
            Camera = Object.Instantiate(Resources.Load<CameraScript>(CameraPath));

        public Shine CreateShine() => 
            Object.Instantiate(Resources.Load<Shine>(ShinePath));

        public Hud CreateHud() => 
            Hud = InstantiateOnActiveScene(HudPath).GetComponent<Hud>();

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

        public void CreateColumn(int height, float offsetX, float offsetY, int index, GameObject parent, IGameCurator gameCurator)
        {
            var obj = InstantiateOnActiveScene(ColumnCreatorPath);
            obj.transform.SetParent(parent.transform);
            obj.transform.localPosition = new Vector3(index * offsetX, -(height / 2f - 0.5f) * offsetY, 0);
            obj.transform.localScale = new Vector3(0.92f, 1.095f * height, 0);

            var column = obj.GetComponent<Column>();
            column.Initialize(gameCurator, index, height);
            _gridService.Columns.Add(column);
        }


        public void CreateGrid() => InstantiateOnActiveScene(GridCreatorPath);
    }
}