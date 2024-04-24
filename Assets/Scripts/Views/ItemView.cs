using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image itemImage;
    [SerializeField] private RectTransform thisTransform;
    [SerializeField] private Button button;

    private Vector2 _moveTo;
    private bool _isMovie;

    public Action OnClickItem;
    public Action OnMovieCompleted;

    private void Awake()
    {
        button.onClick.AddListener(() => OnClickItem?.Invoke());
    }

    public void SetItem(EItemType itemType)
    {
        var index = (int) itemType;

        if (index > -1 && index < sprites.Length)
        {
            itemImage.sprite = sprites[index];
            gameObject.SetActive(true);
        }
        else
        {
            SetDefault();
            gameObject.SetActive(false);
        }
    }

    public void SetPosition(Vector2 pos)
    {
        thisTransform.anchoredPosition = pos;
    }

    public void SetSelected()
    {
        thisTransform.DOScale(Vector3.one * 1.2f, 0.2f);
        thisTransform.DORotate(Vector3.forward * 10f, 0.25f).OnComplete(() =>
            thisTransform.DORotate(Vector3.forward * -10f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine));
    }

    public void SetDefault()
    {
        thisTransform.DOKill();
        thisTransform.localScale = Vector3.one;
        thisTransform.rotation = Quaternion.identity;
    }
    
    public void MovieToPosition(Vector2 pos)
    {
        thisTransform.DOAnchorPos(pos, 0.15f).OnComplete(() => OnMovieCompleted?.Invoke());
    }

    private void OnDisable()
    {
        SetDefault();
    }
}
