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
        IItemSlotView
{
    [SerializeField]
    [Range(0f, 1f)]
    private float dragSmoothTime = 0.1f;

    [field: SerializeField]
    public int Id { get; set; } = -1;

    [Inject(Id = "main_canvas")]
    private Canvas canvas;

    private RectTransform rectTransform;
    private Vector2 dragStartPosition;
    private Vector2 clickStartPosition;
    private ItemSlotState state = ItemSlotState.RESTING;
    private CanvasGroup itemIconCanvasGroup;
    private Tween dragTween;
    private Image itemIcon;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        itemIcon = GetComponent<Image>();
        itemIconCanvasGroup = GetComponent<CanvasGroup>();

        itemIcon.material = new Material(itemIcon.material);

        SetItem(BagItemData.NO_TIEM);
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
                // TODO: make needed calculations here (checks)
                //
                // if (okay to merge)
                // state = ItemSlotState.MERGING;
                ShowMergingVisuals();
                // else
                state = ItemSlotState.MERGING;
                // SnapToSlot();
                break;
            case ItemSlotState.SNAPPING:
                state = ItemSlotState.RESTING; // TODO: wait for the snap animation and then change the state
                break;
            case ItemSlotState.MERGING:
                state = ItemSlotState.RESTING; // TODO: wait for the merge animation and then change the state
                SetItem(new());
                break;
            case ItemSlotState.HOVERED:
                break;
        }
    }

    public void SetItem(BagItemData data)
    {
        var hasItem = data.Id > -1;

        itemIconCanvasGroup.alpha = hasItem ? 1f : 0f;
        itemIconCanvasGroup.interactable = hasItem;
        itemIconCanvasGroup.blocksRaycasts = hasItem;

        if (!hasItem)
            return;

        itemIcon.color = data.Color;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 screenDelta = eventData.position - clickStartPosition;
        Vector2 canvasDelta = screenDelta / canvas.scaleFactor;
        dragTween.Complete();
        dragTween = rectTransform.DOAnchorPos(dragStartPosition + canvasDelta, dragSmoothTime);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (state != ItemSlotState.RESTING)
            return;

        state = ItemSlotState.DRAGGING;

        dragStartPosition = rectTransform.anchoredPosition;
        clickStartPosition = eventData.position;

        ShowDraggingVisuals();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (state != ItemSlotState.DRAGGING)
            return;

        state = ItemSlotState.RELEASED;
        dragTween.Complete();
    }

    private void ShowDraggingVisuals()
    {
        itemIcon.materialForRendering.renderQueue += 1;
        itemIcon.DOColor(Color.deepPink, .5f);
    }

    private void ShowMergingVisuals()
    {
        itemIcon.material.renderQueue -= 1;
        itemIcon.DOColor(Color.white, .5f);
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
