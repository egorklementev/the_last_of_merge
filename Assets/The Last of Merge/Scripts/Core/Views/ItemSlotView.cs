using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class ItemSlotView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    [Range(0f, 1f)]
    private float dragSmoothTime = 0.1f;

    [SerializeField]
    private Canvas canvas; // TODO: inject this

    private RectTransform rectTransform;
    private Vector2 dragStartPosition;
    private Vector2 clickStartPosition;
    private ItemSlotState state = ItemSlotState.RESTING;
    private Tween dragTween;
    private Image itemIcon;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        itemIcon = GetComponent<Image>();

        itemIcon.material = new Material(itemIcon.material);
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
                state = ItemSlotState.SNAPPING;
                // TODO: make needed calculations here (checks)
                break;
            case ItemSlotState.SNAPPING:
                state = ItemSlotState.RESTING; // TODO: wait for the snap animation and then change the state
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 screenDelta = eventData.position - clickStartPosition;
        Vector2 canvasDelta = screenDelta / canvas.scaleFactor;
        //rectTransform.anchoredPosition = dragStartPosition + canvasDelta;
        dragTween.Kill();
        dragTween = rectTransform.DOAnchorPos(dragStartPosition + canvasDelta, dragSmoothTime);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (state != ItemSlotState.RESTING)
            return;

        state = ItemSlotState.DRAGGING;
        dragStartPosition = rectTransform.anchoredPosition;
        clickStartPosition = eventData.position;

        itemIcon.materialForRendering.renderQueue += 1;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (state != ItemSlotState.DRAGGING)
            return;

        state = ItemSlotState.RELEASED;
        dragTween.Kill();

        itemIcon.material.renderQueue -= 1;
    }

    public enum ItemSlotState
    {
        RESTING,
        HOVERED,
        DRAGGING,
        RELEASED,
        SNAPPING,
    }
}
