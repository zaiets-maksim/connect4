using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridService : IGridService
{
    public Cell[][] Grid { get; private set; }
    
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
    public void SetCell(int x, int y, Cell cell) => Grid[x][y] = cell;
    public void TakeCell(int x, int y, PlayerId state) => Grid[x][y].Take(state);
    public void ReleaseCell(int x, int y) => Grid[x][y].Release();
    public void Initialization(Cell[][] grid) => Grid = grid;
    
    public bool HasEmptyUnder(Cell cell, int count = 1) => 
        Columns[cell.Index.y].LastElementIndex - cell.Index.x > count;

    public bool RawHasFreeCell(Cell[] raw) => raw.Any(cell => cell.CellId == PlayerId.Empty);
    public bool GridHasFreeCell() => Columns.Any(x => x.HasFreeCell);
}

public interface IGridService
{
    Cell[][] Grid { get; }
    int Height { get; }
    int Width { get; }
    public List<Column> Columns { get; set; }
    
    Cell[] GetRow(int number);
    Cell[] GetColumn(int number);
    
    void Initialization(Cell[][] grid);
    
    Cell GetCell(int x, int y);
    void SetCell(int x, int y, Cell cell);
    
    void TakeCell(int x, int y, PlayerId state);
    void ReleaseCell(int x, int y);

    bool HasEmptyUnder(Cell cell, int count = 1);
    bool RawHasFreeCell(Cell[] raw);
    bool GridHasFreeCell();
}
