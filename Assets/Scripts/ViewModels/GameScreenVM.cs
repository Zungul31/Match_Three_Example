using UnityEngine;

public class GameScreenVM : ViewModelBase<GameScreenView>
{
    public GameScreenVM(Canvas canvas, GameCore gameCore) : base(canvas, gameCore) { }

    public void InitWindow(EItemType[,] grid)
    {
        view.InitView(grid);
    }

    public void UpdateView(EItemType[,] grid)
    {
        view.UpdateView(grid);
    }
    
    public void DeleteCombinations(EItemType[,] grid)
    {
        view.UpdateView(grid);
        view.HideSpaces(grid);
        gameCore.RemoveSpaces();
    }
    
    private void MoveItems(Vector2Int first, Vector2Int second)
    {
        if (gameCore.TryMoveItems(first, second))
        {
            gameCore.DeleteCombinations();
        }
        else
        {
            view.ShakeItems(first, second);
            view.ClearSelectedItems();
        }
    }

    private void CheckMatching()
    {
        gameCore.CheckMatching();
    }
    
    public override void Engage()
    {
        base.Engage();
        
        view.OnMoveItems += MoveItems;
        view.OnMoviesCompleted += CheckMatching;
    }

    public override void Disengage()
    {
        view.OnMoveItems = null;
        view.OnMoviesCompleted = null;
        
        base.Disengage();
    }
}