using System;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenView : ViewBase
{
    [SerializeField] private Transform itemsContainer;
    
    private ItemView[,] items;

    [SerializeField] private List<ParticleView> poolParticles = new List<ParticleView>();

    private const int _cellSize = 195;
    private const int _cellSpacing = 10;
    private Vector2 _startCellPos = new Vector2(97.5f, -97.5f);

    private Vector2Int _firstItemPos = Vector2Int.down;
    private Vector2Int _secondItemPos = Vector2Int.down;

    private int _activeItemsCount = 0;

    public Action<Vector2Int, Vector2Int> OnMoveItems;
    public Action OnMoviesCompleted; 

    public void InitView(EItemType[,] grid)
    {
        items = new ItemView[grid.GetLength(0), grid.GetLength(1)];
        
        for (var y = 0; y < grid.GetLength(1); y++)
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                var item = ResourcesManager.InstantiatePrefab<ItemView>(itemsContainer);
                item.SetItem(grid[x, y]);
                var pos = new Vector2(_startCellPos.x + (_cellSize + _cellSpacing) * x,
                    _startCellPos.y - (_cellSize + _cellSpacing) * y);
                item.SetPosition(pos);

                var tmpX = x;
                var tmpY = y;

                item.OnClickItem += () => ClickItem(tmpX, tmpY);

                items[x, y] = item;
            }
        }
    }

    public void UpdateView(EItemType[,] grid)
    {
        ClearSelectedItems();
        for (var y = 0; y < grid.GetLength(1); y++)
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                var tmpX = x;
                var tmpY = y;
                
                items[x, y].OnClickItem = null;

                if (grid[x, y] != EItemType.None)
                {
                    items[x, y].OnClickItem += () => ClickItem(tmpX, tmpY);

                    items[x, y].SetItem(grid[x, y]);
                }
                else
                {
                    items[x, y].SetItem(EItemType.None);
                    var pos = new Vector2(_startCellPos.x + (_cellSize + _cellSpacing) * x,
                        _startCellPos.y - (_cellSize + _cellSpacing) * y);
                    SetParticle(pos);
                }
            }
        }
    }

    public void HideSpaces(EItemType[,] grid)
    {
        ClearSelectedItems();
        _activeItemsCount = 0;
        for (var x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = grid.GetLength(1) - 1, spaces = 0; y >= 0; y--)
            {
                if (grid[x, y] == EItemType.None)
                {
                    spaces++;
                }
                if (spaces != 0)
                {
                    items[x, y].SetItem(y - spaces >= 0 ? grid[x, y - spaces] : EItemType.None);
                    var pos = new Vector2(_startCellPos.x + (_cellSize + _cellSpacing) * x,
                        _startCellPos.y - (_cellSize + _cellSpacing) * (y - spaces));
                    items[x, y].SetPosition(pos);
                    pos = new Vector2(_startCellPos.x + (_cellSize + _cellSpacing) * x,
                        _startCellPos.y - (_cellSize + _cellSpacing) * y);
                    items[x, y].MovieToPosition(pos);
                    
                    _activeItemsCount++;
                    items[x, y].OnMovieCompleted = null;
                    items[x, y].OnMovieCompleted += CompletedMovieItem;
                }
            }
        }
    }

    private void CompletedMovieItem()
    {
        if (_activeItemsCount > 1)
        {
            _activeItemsCount--;
            return;
        }

        if (_activeItemsCount == 0)
        {
            return;
        }
        
        _activeItemsCount = 0;
        OnMoviesCompleted?.Invoke();
    }
    
    private void SetParticle(Vector2 pos)
    {
        foreach (var particle in poolParticles)
        {
            if (!particle.IsAlive())
            {
                particle.gameObject.SetActive(true);
                particle.SetPosition(pos);
                return;
            }
        }
        
        var item = ResourcesManager.InstantiatePrefab<ParticleView>(itemsContainer);
        item.SetPosition(pos);
        poolParticles.Add(item);
    }

    private void ClearPool()
    {
        foreach (var particle in poolParticles)
        {
            particle.gameObject.SetActive(false);
        }
    }
    
    private void ClickItem(int x, int y)
    {
        if (_firstItemPos == Vector2Int.down)
        {
            _firstItemPos = new Vector2Int(x, y);
            items[x,y].SetSelected();
            return;
        }

        _secondItemPos = new Vector2Int(x, y);

        if (Vector2Int.Distance(_firstItemPos, _secondItemPos) != 1f)
        {
            items[_firstItemPos.x, _firstItemPos.y].SetDefault();
            _firstItemPos = Vector2Int.down;
            return;
        }
        
        ShakeItems(_firstItemPos, _secondItemPos);

        items[_secondItemPos.x, _secondItemPos.y].OnMovieCompleted = null;
        items[_secondItemPos.x, _secondItemPos.y].OnMovieCompleted =
            () =>
            {
                items[_secondItemPos.x, _secondItemPos.y].OnMovieCompleted = null;
                OnMoveItems?.Invoke(_firstItemPos, _secondItemPos);
            };
    }

    public void ShakeItems(Vector2Int firs, Vector2Int second)
    {
        items[firs.x, firs.y].SetDefault();
        items[second.x, second.y].SetDefault();

        items[firs.x, firs.y].OnClickItem = null;
        items[firs.x, firs.y].OnClickItem += () => ClickItem(second.x, second.y);
        
        items[second.x, second.y].OnClickItem = null;
        items[second.x, second.y].OnClickItem += () => ClickItem(firs.x, firs.y);
        
        var firstPos = new Vector2(_startCellPos.x + (_cellSize + _cellSpacing) * second.x,
            _startCellPos.y - (_cellSize + _cellSpacing) * second.y);
        
        var secondPos = new Vector2(_startCellPos.x + (_cellSize + _cellSpacing) * firs.x,
            _startCellPos.y - (_cellSize + _cellSpacing) * firs.y);

        items[firs.x, firs.y].MovieToPosition(firstPos);
        items[second.x, second.y].MovieToPosition(secondPos);

        (items[firs.x, firs.y], items[second.x, second.y]) = (items[second.x, second.y], items[firs.x, firs.y]);
    }
    
    public void ClearSelectedItems()
    {
        _firstItemPos = Vector2Int.down;
        _secondItemPos = Vector2Int.down;
    }
    
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
