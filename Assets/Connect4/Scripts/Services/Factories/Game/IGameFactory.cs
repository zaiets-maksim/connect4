using Connect4.Scripts.UI;
using UnityEngine;

namespace Infrastructure.Services.Factories.Game
{
    public interface IGameFactory
    {
        Hud Hud { get; }
        CameraScript Camera { get; }

        CameraScript CreateCamera();
        Hud CreateHud();
        Piece CreatePiece(Vector2 position, Color color);
        Cell CreateCell(int x, int y, float offsetX, float offsetY, float scale, Transform parent);
        void CreateColumn(int height, float offsetX, float offsetY, int index, GameObject parent, IGameCurator gameCurator);
        void CreateGrid();
        Shine CreateShine();
    }
}