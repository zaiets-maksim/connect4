using UnityEngine;

namespace Infrastructure.Services.Factories.Game
{
    public interface IGameFactory
    {
        Piece CreatePiece(Vector2 position, Color color);
        Cell CreateCell(int x, int y, float offsetX, float offsetY, float scale, Transform parent);
        void CreateColumn(int height, float offsetX, float offsetY, int index, GameObject parent);
        void CreateGrid();
    }
}