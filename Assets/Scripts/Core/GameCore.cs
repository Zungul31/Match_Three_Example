using UnityEngine;

public class GameCore : MonoBehaviour
{
    [SerializeField] private WindowsState windowsState;

    private GridModel _gridModel;

    private const int _widthGrid = 7;
    private const int _heightGrid = 8;
    // private const int _widthGrid = 5;
    // private const int _heightGrid = 5;
    private const int _diversityGrid = 5;

    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        _gridModel = new GridModel(_widthGrid, _heightGrid, _diversityGrid);
        _gridModel.InitGrid();
        OpenMainWindow();
    }

    private void OpenMainWindow()
    {
        windowsState.SetWindow<GameScreenVM>().InitWindow(_gridModel.grid);
    }

    public bool TryMoveItems(Vector2Int first, Vector2Int second)
    {
        return _gridModel.TryMovieItems(first, second);
    }

    public void CheckMatching()
    {
        var matching = _gridModel.CheckMatching();
        if (matching.Count != 0)
        {
            DeleteCombinations();
        }
        else
        {
            _gridModel.CheckCombinations();
            windowsState.GetWindow<GameScreenVM>().UpdateView(_gridModel.grid);
        }
    }
    
    public void DeleteCombinations()
    {
        _gridModel.DeleteCombinations();
        windowsState.GetWindow<GameScreenVM>().DeleteCombinations(_gridModel.grid);
    }

    public void RemoveSpaces()
    {
        _gridModel.MovieDown();
        _gridModel.RemoveSpaces();
        windowsState.GetWindow<GameScreenVM>().UpdateView(_gridModel.grid);
    }
}
