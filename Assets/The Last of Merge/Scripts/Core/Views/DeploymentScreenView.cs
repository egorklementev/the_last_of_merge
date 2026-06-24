using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DeploymentScreenView : MonoBehaviour, IDeploymentScreenView
{
    public event Action DeployButtonClicked;

    [SerializeField]
    private Button deployButton;

    [SerializeField]
    private GameObject inDeploymentLabel;

    [SerializeField]
    private Slider timeLeftBar;

    [SerializeField]
    private Transform foundItemsHolder;

    [SerializeField]
    private FoundItemEntryView foundItemEntryViewPrefab;

    private Tween timeLeftBarTween;

    void Start()
    {
        deployButton.onClick.AddListener(() => DeployButtonClicked?.Invoke());
    }

    public void SetInDeployment(float timeleft)
    {
        inDeploymentLabel.SetActive(true); // TODO: maybe animation?

        foreach (var child in foundItemsHolder)
        {
            Destroy((child as Transform).gameObject); // TODO: change to pooling + canvas groups
        }

        timeLeftBarTween.Kill();
        timeLeftBar.value = 1f; // TODO: change to actual % rather than "this" beast
        timeLeftBarTween = timeLeftBar
            .DOValue(0f, timeleft)
            .SetEase(Ease.Linear)
            .OnComplete(FinishDeployment);
    }

    public void FinishDeployment()
    {
        timeLeftBar.value = 0f;
        inDeploymentLabel.SetActive(false);
    }

    public void SetFoundItems(IEnumerable<BagItemData> items)
    {
        foreach (var data in items)
        {
            var itemView = Instantiate(foundItemEntryViewPrefab, foundItemsHolder); // TODO: change to pooling + canvas groups
            itemView.Init(data);
        }
    }
}

public interface IDeploymentScreenView
{
    public event Action DeployButtonClicked;

    public void SetInDeployment(float timeleft);

    public void FinishDeployment();

    public void SetFoundItems(IEnumerable<BagItemData> items);
}
