using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class ItemSlotView
    : MonoBehaviour,
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IItemSlotView
{
    public event Action Moved;

    public ItemSlot ContainingSlot { get; set; }

    [SerializeField]
    [Range(0f, 1f)]
    private float dragSmoothTime = 0.1f;

    [Inject(Id = "main_canvas")]
    private Canvas canvas;

    private RectTransform rectTransform;
    private Vector2 dragStartPosition;
    private Vector2 clickStartPosition;
    private ItemSlotState state = ItemSlotState.RESTING;
    private CanvasGroup itemIconCanvasGroup;
    private Tween dragTween;
    private Tween colorTween;
    private Image itemIcon;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        itemIcon = GetComponent<Image>();
        itemIconCanvasGroup = GetComponent<CanvasGroup>();

        itemIcon.material = new Material(itemIcon.material);

        SetEmpty();
    }

    void Update()
    {
        switch (state)
        {
            case ItemSlotState.RESTING:
                break;
            case ItemSlotState.DRAGGING:
                break;
            case ItemSlotState.RELEASED:
                Moved?.Invoke();
                break;
            case ItemSlotState.SNAPPING:
                ShowSnappingVisuals();
                state = ItemSlotState.RESTING; // TODO: wait for the snap animation and then change the state
                break;
            case ItemSlotState.MERGING:
                ShowMergingVisuals();
                state = ItemSlotState.RESTING; // TODO: wait for the merge animation and then change the state
                break;
            case ItemSlotState.HOVERED:
                break;
        }
    }

    public void SetItem(BagItemData data)
    {
        itemIconCanvasGroup.alpha = 1f;
        itemIcon.sprite = data.Sprite;
    }

    public void SetEmpty()
    {
        itemIconCanvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = Vector2.zero;
        itemIcon.sprite = null;
    }

    public void OnMergeWithSlot()
    {
        SetEmpty();
        state = ItemSlotState.MERGING;
    }

    public void OnSnapToSlot(bool isSameSlot)
    {
        if (isSameSlot)
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            SetEmpty();
        }

        state = ItemSlotState.SNAPPING;
    }

    public bool IsHovered() => state == ItemSlotState.HOVERED;

    public void OnDrag(PointerEventData eventData)
    {
        if (state != ItemSlotState.DRAGGING)
            return;

        Vector2 screenDelta = eventData.position - clickStartPosition;
        Vector2 canvasDelta = screenDelta / canvas.scaleFactor;
        dragTween.Kill();
        dragTween = rectTransform.DOAnchorPos(dragStartPosition + canvasDelta, dragSmoothTime);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (state != ItemSlotState.RESTING && state != ItemSlotState.HOVERED)
            return;

        if (ContainingSlot.IsEmpty())
            return;

        state = ItemSlotState.DRAGGING;

        dragStartPosition = rectTransform.anchoredPosition;
        clickStartPosition = eventData.position;

        itemIconCanvasGroup.blocksRaycasts = false;

        ShowDraggingVisuals();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (state != ItemSlotState.DRAGGING)
            return;

        state = ItemSlotState.RELEASED;
        dragTween.Complete();

        itemIconCanvasGroup.blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (state != ItemSlotState.RESTING)
            return;

        state = ItemSlotState.HOVERED;
        itemIcon.color = Color.black;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (state != ItemSlotState.HOVERED)
            return;

        state = ItemSlotState.RESTING;
        itemIcon.color = Color.white;
    }

    private void ShowDraggingVisuals()
    {
        itemIcon.materialForRendering.renderQueue += 1;
        colorTween = itemIcon.DOColor(Color.deepPink, .5f);
    }

    private void ShowMergingVisuals()
    {
        colorTween.Kill();
        itemIcon.material.renderQueue -= 1;
        itemIcon.color = Color.white;
    }

    private void ShowSnappingVisuals()
    {
        colorTween.Kill();
        itemIcon.material.renderQueue -= 1;
        itemIcon.color = Color.white;
    }

    public enum ItemSlotState
    {
        RESTING,
        HOVERED,
        DRAGGING,
        RELEASED,
        SNAPPING,
        MERGING,
    }
}
