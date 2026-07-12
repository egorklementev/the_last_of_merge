using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using static IItemSlotView;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class ItemSlotView
    : MonoBehaviour,
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IItemSlotView
{
    public event Action Moved;
    public event Action LongPress;

    public ItemSlot ContainingSlot { get; set; }

    [SerializeField]
    [Range(0f, 1f)]
    private float dragSmoothTime = 0.1f;

    [SerializeField]
    [Range(0f, 2f)]
    private float longPressTime = .5f;

    [Inject(Id = "main_canvas")]
    private Canvas canvas;

    [Inject(Id = "drag_layer")]
    private Transform dragLayer;

    private Transform originParent;
    private RectTransform rectTransform;
    private RectTransform dragLayerRect;
    private Vector2 dragStartPosition;
    private Vector2 clickStartPosition;
    private ItemSlotState state = ItemSlotState.RESTING;
    private CanvasGroup itemIconCanvasGroup;
    private Tween dragTween;
    private Tween colorTween;
    private Image itemIcon;
    private float longPressTimer;
    private bool pointerIsDown;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        itemIcon = GetComponent<Image>();
        itemIconCanvasGroup = GetComponent<CanvasGroup>();
        originParent = transform.parent;
        dragLayerRect = dragLayer.GetComponent<RectTransform>();

        itemIcon.material = new Material(itemIcon.material);

        SetEmpty();
    }

    void Update()
    {
        switch (state)
        {
            case ItemSlotState.RESTING:
            case ItemSlotState.DRAGGING:
            case ItemSlotState.HOVERED:
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
            case ItemSlotState.LONG_PRESS:
                LongPress?.Invoke();
                ShowSnappingVisuals();
                state = ItemSlotState.RESTING;
                break;
        }

        longPressTimer += pointerIsDown ? Time.deltaTime : 0f;
        if (longPressTimer > longPressTime)
        {
            longPressTimer = 0f;
            pointerIsDown = false; // Reset this so that no duplication occurs
            state = ItemSlotState.LONG_PRESS;
        }
    }

    public void SetItem(BagItemData data)
    {
        if (data == null)
        {
            SetEmpty();
            return;
        }

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

    public ItemSlotState GetState() => state;

    public void OnDrag(PointerEventData eventData)
    {
        if (state != ItemSlotState.DRAGGING)
            return;

        Vector2 screenDelta = eventData.position - clickStartPosition;
        Vector2 canvasDelta = screenDelta / canvas.scaleFactor;
        dragTween.Kill();
        dragTween = rectTransform.DOAnchorPos(dragStartPosition + canvasDelta, dragSmoothTime);

        pointerIsDown = false;
        longPressTimer = 0f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (state != ItemSlotState.RESTING && state != ItemSlotState.HOVERED)
            return;

        if (ContainingSlot.IsEmpty())
            return;

        state = ItemSlotState.DRAGGING;

        itemIconCanvasGroup.blocksRaycasts = false;

        ShowDragStartVisuals();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragLayerRect,
            eventData.position,
            canvas.worldCamera,
            out var localPoint
        );

        rectTransform.anchoredPosition = localPoint;
        dragStartPosition = rectTransform.anchoredPosition;
        clickStartPosition = eventData.position;
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
        itemIcon.color = Color.gray6;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (state != ItemSlotState.HOVERED)
            return;

        state = ItemSlotState.RESTING;
        itemIcon.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerIsDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerIsDown = false;
    }

    private void ShowDragStartVisuals()
    {
        colorTween = itemIcon.DOColor(Color.lightGoldenRod, .5f);
        rectTransform.SetParent(dragLayer);
        rectTransform.localScale = .222f * Vector3.one;
    }

    private void ShowMergingVisuals()
    {
        colorTween.Kill();
        itemIcon.color = Color.white;
        rectTransform.SetParent(originParent);
        rectTransform.localScale = Vector3.one;
    }

    private void ShowSnappingVisuals()
    {
        colorTween.Kill();
        itemIcon.color = Color.white;
        rectTransform.SetParent(originParent);
        rectTransform.localScale = Vector3.one;
    }
}
