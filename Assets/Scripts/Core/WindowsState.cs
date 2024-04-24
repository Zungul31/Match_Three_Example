using System;
using System.Collections.Generic;
using UnityEngine;

public class WindowsState : MonoBehaviour
{
    [SerializeField] private GameCore gameCore;
    [SerializeField] private Canvas windowCanvas;


    private readonly Dictionary<string, ViewModelBase> _windows = new Dictionary<string, ViewModelBase>();
    private ViewModelBase _activeWindow;
    private List<ViewModelBase> _activePopupWindows;

    private void AddWindow(ViewModelBase w)
    {
        _windows.Add(w.GetType().Name, w);
    }

    private void InitWindows()
    {
        AddWindow(new GameScreenVM(windowCanvas, gameCore));
    }

    private void DisengageAllWindows()
    {
        foreach (var item in _windows)
        {
            item.Value.Disengage();
        }
    }

    public T SetPopupWindow<T>() where T : ViewModelBase
    {
        var name = typeof(T).Name;
        try
        {
            T w = (T)_windows[name];

            //turn on new
            _activePopupWindows.Add(w);
            _activePopupWindows[_activePopupWindows.Count - 1].Engage();
            return w;
        }
        catch (Exception e)
        {
            Debug.LogError("Somth went wrong  in setWindow");
            Debug.LogError(e);
            return null;
        }
    }

    public T SetWindow<T>() where T : ViewModelBase
    {
        var name = typeof(T).Name;
        try
        {
            T w = (T)_windows[name];

            //turn off old
            _activeWindow?.Disengage();

            //turn on new
            _activeWindow = w;
            _activeWindow.Engage();
            return w;
        }
        catch (Exception e)
        {
            Debug.LogError("Somth went wrong  in setWindow");
            Debug.LogError(e);
            
            return null;
        }
    }

    public T GetWindow<T>() where T : ViewModelBase
    {
        try
        {
            var name = typeof(T).Name;
            T w = (T)_windows[name];
            return w;
        }
        catch (Exception e)
        {
            Debug.LogError("Somth went wrong  in getWindow");
            Debug.LogError(e);
            return null;
        }
    }

    private void Awake()
    {
        InitWindows();
        DisengageAllWindows();

        _activePopupWindows = new List<ViewModelBase>();
    }

}
