using UnityEngine;

public abstract class ViewModelBase
{
    protected GameCore gameCore;
    protected Canvas _canvas;
    public abstract void Engage();
    public abstract void Disengage();
    
    protected ViewModelBase(Canvas canvas, GameCore gameCore)
    {
        _canvas = canvas;
        this.gameCore = gameCore;
    }
}

public abstract class ViewModelBase<TView>: ViewModelBase where TView : ViewBase
{
    protected readonly TView view;

    protected ViewModelBase(Canvas canvas, GameCore gameCore) : base(canvas, gameCore)
    {
        view = ResourcesManager.InstantiatePrefab<TView>();
        view.transform.SetParent( _canvas.transform, false ); 
    }


    public override void Engage()
    {
        view.Show();
    }

    public override void Disengage()
    {
        view.Hide();
    }
    
    public TView GetView()
    {
        return view;
    }
}



