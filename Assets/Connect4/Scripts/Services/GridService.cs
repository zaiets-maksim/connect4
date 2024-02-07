public class GridService : IGridService
{
    public Cell[][] Grid { get; private set; }
    
    public Cell[] GetRow(int number) => Grid[number];
    
    public int Height => Grid.Length;
    public int Width => Grid[0].Length;

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
    
    public void Initialization(Cell[][] grid) => Grid = grid;
}

public interface IGridService
{
    int Height { get; }
    int Width { get; }
    Cell[][] Grid { get; }
    Cell[] GetRow(int number);
    Cell[] GetColumn(int number);
    void Initialization(Cell[][] grid);
    Cell GetCell(int x, int y);
    void SetCell(int x, int y, Cell cell);

    void TakeCell(int x, int y, PlayerId state);
}
