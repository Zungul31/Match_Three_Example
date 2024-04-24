using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridModel
{
    public EItemType[,] grid { get; private set; }

    private Dictionary<Vector2Int, List<Vector2Int>> _possibleMoves = new Dictionary<Vector2Int, List<Vector2Int>>();

    private readonly int _width;
    private readonly int _height;
    private readonly int _diversity;

    public GridModel(int width, int height, int diversity)
    {
        _width = width;
        _height = height;
        _diversity = diversity;
    }

    public void InitGrid()
    {
        grid = new EItemType[_width, _height];
        
        for (var y = 0; y < grid.GetLength(1); y++)
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                grid[x, y] = (EItemType) Random.Range(0, _diversity);
            }
        }
        
        var combinations = CheckMatching();

        if (combinations.Count != 0)
        {
            RebuildGrid(combinations);
        }

        CheckCombinations();

        if (_possibleMoves.Count == 0)
        {
            InitGrid();
        }
    }

    public List<List<CellModel>> CheckMatching()
    {
        var combinations = new List<List<CellModel>>();

        for (var y = 0; y < grid.GetLength(1); y++)
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == EItemType.None) { continue; }

                var horizontally = new List<CellModel>();
                var vertically = new List<CellModel>();

                if (x == 0 || grid[x, y] != grid[x - 1, y])
                {
                    for (var i = 0; x + i < grid.GetLength(0) && grid[x, y] == grid[x + i, y]; i++)
                    {
                        if (i == 0)
                        {
                            horizontally.Add(new CellModel(grid[x, y], x, y));
                            continue;
                        }
                        horizontally.Add(new CellModel(grid[x + i, y], x + i, y));
                    }
                }

                if (y == 0 || grid[x, y] != grid[x, y - 1])
                {
                    for (var j = 0; y + j < grid.GetLength(1) && grid[x, y] == grid[x, y + j]; j++)
                    {
                        if (j == 0)
                        {
                            vertically.Add(new CellModel(grid[x, y], x, y));
                            continue;
                        }
                        vertically.Add(new CellModel(grid[x, y + j], x, y + j));
                    }
                }

                if (horizontally.Count > 2)
                {
                    combinations.Add(horizontally);
                }

                if (vertically.Count > 2)
                {
                    combinations.Add(vertically);
                }
            }
        }

        return combinations;
    }

    private void RebuildGrid(List<List<CellModel>> combinations)
    {
        foreach (var combination in combinations)
        {
            var newItem = combination[0].type;

            while (combination[0].type == newItem)
            {
                newItem = (EItemType) Random.Range(0, _diversity);
            }
            var cell = combination[Random.Range(0, combination.Count)];
            grid[cell.x, cell.y] = newItem;
        }
        
        combinations = CheckMatching();
        if (combinations.Count != 0) { RebuildGrid(combinations); }
    }

    public void CheckCombinations()
    {
        _possibleMoves.Clear();
        for (var y = 0; y < grid.GetLength(1); y++)
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                var curType = grid[x, y];

                if (x + 1 < grid.GetLength(0) && curType == grid[x + 1, y])
                {
                    if (x + 2 != grid.GetLength(0))
                    {
                        if (y != 0 && curType == grid[x + 2, y - 1])
                        {
                            AddPossibleMoves(new Vector2Int(x + 2, y - 1), new Vector2Int(x + 2, y));
                        }

                        if (x + 3 != grid.GetLength(0) && curType == grid[x + 3, y])
                        {
                            AddPossibleMoves(new Vector2Int(x + 3, y), new Vector2Int(x + 2, y));
                        }

                        if (y + 1 != grid.GetLength(1) && curType == grid[x + 2, y + 1])
                        {
                            AddPossibleMoves(new Vector2Int(x + 2, y + 1), new Vector2Int(x + 2, y));
                        }
                    }

                    if (x != 0)
                    {
                        if (y != 0 && curType == grid[x - 1, y - 1])
                        {
                            AddPossibleMoves(new Vector2Int(x - 1, y - 1), new Vector2Int(x - 1, y));
                        }

                        if (x > 1 && curType == grid[x - 2, y])
                        {
                            AddPossibleMoves(new Vector2Int(x - 2, y), new Vector2Int(x - 1, y));
                        }

                        if (y + 1 != grid.GetLength(1) && curType == grid[x - 1, y + 1])
                        {
                            AddPossibleMoves(new Vector2Int(x - 1, y + 1), new Vector2Int(x - 1, y));
                        }
                    }
                }

                if (x + 2 < grid.GetLength(0) && curType == grid[x + 2, y])
                {
                    if (y != 0 && curType == grid[x + 1, y - 1])
                    {
                        AddPossibleMoves(new Vector2Int(x + 1, y - 1), new Vector2Int(x + 1, y));
                    }

                    if (y + 1 != grid.GetLength(1) && curType == grid[x + 1, y + 1])
                    {
                        AddPossibleMoves(new Vector2Int(x + 1, y + 1), new Vector2Int(x + 1, y));
                    }
                }

                if (y + 1 < grid.GetLength(1) && curType == grid[x, y + 1])
                {
                    if (y + 2 != grid.GetLength(1))
                    {
                        if (x != 0 && curType == grid[x - 1, y + 2])
                        {
                            AddPossibleMoves(new Vector2Int(x - 1, y + 2), new Vector2Int(x, y + 2));
                        }

                        if (y + 3 != grid.GetLength(1) && curType == grid[x, y + 3])
                        {
                            AddPossibleMoves(new Vector2Int(x, y + 3), new Vector2Int(x, y + 2));
                        }

                        if (x + 1 != grid.GetLength(0) && curType == grid[x + 1, y + 2])
                        {
                            AddPossibleMoves(new Vector2Int(x + 1, y + 2), new Vector2Int(x, y + 2));
                        }
                    }

                    if (y != 0)
                    {
                        if (x != 0 && curType == grid[x - 1, y - 1])
                        {
                            AddPossibleMoves(new Vector2Int(x - 1, y - 1), new Vector2Int(x, y - 1));
                        }

                        if (y > 1 && curType == grid[x, y - 2])
                        {
                            AddPossibleMoves(new Vector2Int(x, y - 2), new Vector2Int(x, y - 1));
                        }

                        if (x + 1 != grid.GetLength(0) && curType == grid[x + 1, y - 1])
                        {
                            AddPossibleMoves(new Vector2Int(x + 1, y - 1), new Vector2Int(x, y - 1));
                        }
                    }
                }

                if (y + 2 < grid.GetLength(1) && curType == grid[x, y + 2])
                {
                    if (x != 0 && curType == grid[x - 1, y + 1])
                    {
                        AddPossibleMoves(new Vector2Int(x - 1, y + 1), new Vector2Int(x, y + 1));
                    }

                    if (x + 1 != grid.GetLength(0) && curType == grid[x + 1, y + 1])
                    {
                        AddPossibleMoves(new Vector2Int(x + 1, y + 1), new Vector2Int(x, y + 1));
                    }
                }
            }
        }

        DebugShowPossibleMoves();
    }

    public bool TryMovieItems(Vector2Int first, Vector2Int second)
    {
        if ((_possibleMoves.ContainsKey(first) && _possibleMoves[first].Contains(second)) ||
            (_possibleMoves.ContainsKey(second) && _possibleMoves[second].Contains(first)))
        {
            (grid[first.x, first.y], grid[second.x, second.y]) = (grid[second.x, second.y], grid[first.x, first.y]);
            return true;
        }
        
        return false;
    }

    public void DeleteCombinations()
    {
        var combinations = CheckMatching();

        foreach (var combination in combinations)
        {
            foreach (var cell in combination)
            {
                grid[cell.x, cell.y] = EItemType.None;
            }
        }
    }

    public void MovieDown()
    {
        for (var x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = grid.GetLength(1) - 1, spaces = 0; y >= 0; y--)
            {
                if (grid[x, y] == EItemType.None)
                {
                    spaces++;
                }
                else if (spaces != 0)
                {
                    grid[x, y + spaces] = grid[x, y];
                    grid[x, y] = EItemType.None;
                }
            }
        }
    }

    public void RemoveSpaces()
    {
        for (var y = 0; y < grid.GetLength(1); y++)
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                if (grid[x, y] == EItemType.None)
                {
                    grid[x, y] = (EItemType) Random.Range(0, _diversity);
                }
            }
        }
    }
    
    private void AddPossibleMoves(Vector2Int currenCell, Vector2Int targetCell)
    {
        if (_possibleMoves.ContainsKey(currenCell))
        {
            _possibleMoves[currenCell].Add(targetCell);
        }
        else
        {
            _possibleMoves.Add(currenCell, new List<Vector2Int>() {targetCell});
        }
    }
    
    private void DebugShowGrid()
    {
        var str = "";
        for (var y = 0; y < grid.GetLength(1); y++)
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                var buf = TypeToString(grid[x, y]);
                str = str + buf;
            }
            str = str + "\n";
        }
        Debug.Log(str);
    }

    private void DebugShowPossibleMoves()
    {
        var str = _possibleMoves.Count.ToString();
        foreach (var move in _possibleMoves)
        {
            str = str + $"{TypeToString(grid[move.Key.x, move.Key.y])}{move.Key}: ";
            foreach (var item in move.Value)
            {
                str = str + $"{TypeToString(grid[item.x, item.y])}{item} ";
            }

            str = str + "\n";
        }

        Debug.Log(str);
    }

    private string TypeToString(EItemType type)
    {
        return type switch
        {
            EItemType.Red => "<color=red>☻</color>",
            EItemType.Yellow => "<color=yellow>☻</color>",
            EItemType.Green => "<color=green>☻</color>",
            EItemType.Blue => "<color=cyan>☻</color>",
            EItemType.Violet => "<color=purple>☻</color>",
            _ => "☻"
        };
    }
}