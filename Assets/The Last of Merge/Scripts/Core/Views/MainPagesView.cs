using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MainPagesView : MonoBehaviour
{
    [SerializeField]
    private Button leftPageArrow;

    [SerializeField]
    private Button rightPageArrow;

    [SerializeField]
    private RectTransform[] pageHolders;

    [SerializeField]
    private int currentPage = 1;

    [SerializeField]
    private float switchDuration = .5f;

    void Start()
    {
        leftPageArrow.onClick.AddListener(() =>
        {
            SwitchToPage(currentPage - 1);
        });

        rightPageArrow.onClick.AddListener(() =>
        {
            SwitchToPage(currentPage + 1);
        });
    }

    private void SwitchToPage(int newPage)
    {
        newPage = math.clamp(newPage, 0, pageHolders.Length - 1);
        if (newPage == currentPage)
            return;

        for (int i = 0; i < pageHolders.Length; i++)
        {
            var holder = pageHolders[i];
            var minX = i - newPage;
            var maxX = i - newPage + 1;
            holder.DOAnchorMin(new Vector2(minX, 0), switchDuration);
            holder.DOAnchorMax(new Vector2(maxX, 1), switchDuration);
        }

        currentPage = newPage;
    }
}
