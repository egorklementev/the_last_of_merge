using DG.Tweening;
using UnityEngine;

public class BagSpaceView : MonoBehaviour, IBagSpaceView
{
    [SerializeField]
    private CanvasGroup equipSpaceGroup;

    private Tween deployTween;

    public void MergeItems(ItemSlot movingSlot, ItemSlot restingSlot, BagItemData resultingItem)
    {
        movingSlot.MergeWithSlot();
        restingSlot.SetItem(resultingItem);
    }

    public void SnapItems(ItemSlot movingSlot, ItemSlot restingSlot)
    {
        restingSlot.SetItem(movingSlot.ItemData);
        movingSlot.SnapToSlot(restingSlot);
    }

    public void SetInDeployment(bool instantly = false)
    {
        var duration = instantly ? 0f : 2f;
        equipSpaceGroup.blocksRaycasts = false;
        equipSpaceGroup.interactable = false;

        deployTween.Kill();
        deployTween = equipSpaceGroup.DOFade(0f, duration);
    }

    public void FinishDeployment(bool instantly = false)
    {
        var duration = instantly ? 0f : 2f;
        deployTween.Kill();
        deployTween = equipSpaceGroup
            .DOFade(1f, duration)
            .OnComplete(() =>
            {
                equipSpaceGroup.blocksRaycasts = true;
                equipSpaceGroup.interactable = true;
            });
    }
}
