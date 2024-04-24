using UnityEngine;

public class ViewBase : MonoBehaviour
{
    protected Canvas _canvas;
    
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

}