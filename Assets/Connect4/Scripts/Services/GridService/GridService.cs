using System;
using System.Collections.Generic;
using System.Linq;
using Connect4.Scripts.Field;
using UnityEngine;

namespace Connect4.Scripts.Services.GridService
{
    public class GridService : IGridService
    {
        public Cell[][] Grid { get; private set; }
 
        public Cell[][] CloneGrid()
        {
            Cell[][] clone = new Cell[Grid.Length][];

            for (int i = 0; i < Grid.Length; i++)
            {
                clone[i] = new Cell[Grid[i].Length];
                Array.Copy(Grid[i], clone[i], Grid[i].Length);
            }

            return clone;
        }

        public int Height => Grid.Length;
        public int Width => Grid[0].Length;

        public List<Column> Columns { get; set; } = new List<Column>();

        public Cell[] GetRow(int number) => Grid[number];

        public Cell[] GetColumn(int number)
        {
            int rows = Grid.Length;
            Cell[] column = new Cell[rows];

            for (int i = 0; i < rows; i++)
                column[i] = Grid[i][number];

            return column;
        }

        public Cell GetCell(int x, int y) => Grid[x][y];
        public Cell GetCell(Vector2Int index) => Grid[index.x][index.y];

        public void TakeCell(int x, int y, PlayerId id) => Grid[x][y].Take(id);

        public void ReleaseCell(int x, int y) => Grid[x][y].Release();

        public Vector2Int TakeColumn(int index)
        {
            Columns[index].TakeElement();
            return new Vector2Int(Columns[index].LastElementIndex, Columns[index].Index);
        }

        public void ReleaseColumn(int index) => Columns[index].ReleaseElement();

        public void Initialization(Cell[][] grid) => Grid = grid;

        public bool HasEmptyUnder(Cell cell, int count = 1) =>
            Columns[cell.Index.y].LastElementIndex - cell.Index.x > count;

        public bool RawHasFreeCell(Cell[] raw) => 
            raw.Any(cell => cell.CellId == PlayerId.Empty);

        public bool GridHasFreeCell() => Columns.Any(x => x.HasFreeCell);
        public void Clear() => Columns.Clear();
    }

    public interface IGridService
    {
        Cell[][] Grid { get; }
        Cell[][] CloneGrid();
        int Height { get; }
        int Width { get; }
        public List<Column> Columns { get; set; }

        Cell[] GetRow(int number);
        Cell[] GetColumn(int number);
        Vector2Int TakeColumn(int index);
        void ReleaseColumn(int index);

        void Initialization(Cell[][] grid);

        Cell GetCell(int x, int y);
        Cell GetCell(Vector2Int index);

        void TakeCell(int x, int y, PlayerId id);
        void ReleaseCell(int x, int y);

        bool HasEmptyUnder(Cell cell, int count = 1);
        bool RawHasFreeCell(Cell[] raw);
        bool GridHasFreeCell();
        void Clear();
    }
}