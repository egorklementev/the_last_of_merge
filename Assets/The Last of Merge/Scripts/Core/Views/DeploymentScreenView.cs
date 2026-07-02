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

    [SerializeField]
    private RectTransform loadingSpinner;

    [SerializeField]
    private CanvasGroup foundItemsGroup;

    [SerializeField]
    private CanvasGroup journeyJournalGroup;

    [SerializeField]
    private Image journeyJournalBackground;

    [SerializeField]
    private Image playerImage;

    [SerializeField]
    private Sprite[] playerSprites; // 0 => default, 1 => leaving

    private Tween timeLeftBarTween;

    void Start()
    {
        deployButton.onClick.AddListener(() => DeployButtonClicked?.Invoke());
    }

    public void SetInDeployment(float timeleft, bool instantly = false)
    {
        foundItemsGroup.Toggle(false);
        journeyJournalGroup.Toggle(true);

        var journalShowDuration = instantly ? 0f : 2f;
        journeyJournalBackground.materialForRendering.SetFloat("_Strength", 1f);
        journeyJournalBackground.materialForRendering.DOFloat(0f, "_Strength", journalShowDuration);

        inDeploymentLabel.SetActive(true); // TODO: maybe animation?
        loadingSpinner.gameObject.SetActive(true);
        loadingSpinner
            .DORotate(new Vector3(0f, 0f, timeleft * -120f), timeleft, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);
        deployButton.interactable = false;

        foreach (var child in foundItemsHolder)
        {
            Destroy((child as Transform).gameObject); // TODO: change to pooling + canvas groups
        }

        timeLeftBarTween.Kill();
        timeLeftBar.value = 1f; // TODO: change to actual % rather than "this" beast
        timeLeftBarTween = timeLeftBar.DOValue(0f, timeleft).SetEase(Ease.Linear);

        var playerImageAnimDuration = instantly ? 0f : 2f;
        playerImage.sprite = playerSprites[1];
        playerImage.DOFade(0f, playerImageAnimDuration);
        playerImage.GetComponent<RectTransform>().DOScale(Vector3.zero, playerImageAnimDuration);
    }

    public void FinishDeployment(bool instantly = false)
    {
        var journalShowDuration = instantly ? 0f : 2f;
        journeyJournalBackground
            .materialForRendering.DOFloat(1f, "_Strength", journalShowDuration)
            .OnComplete(() =>
            {
                foundItemsGroup.Toggle(true);
                journeyJournalGroup.Toggle(false);
            });

        timeLeftBar.value = 0f;
        inDeploymentLabel.SetActive(false);
        loadingSpinner.gameObject.SetActive(false);
        DOTween.Kill(loadingSpinner);
        deployButton.interactable = true;

        var playerImageAnimDuration = instantly ? 0f : 2f;
        playerImage.sprite = playerSprites[0];
        playerImage.DOFade(1f, playerImageAnimDuration);
        playerImage.GetComponent<RectTransform>().DOScale(Vector3.one, playerImageAnimDuration);
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

    public void SetInDeployment(float timeleft, bool instantly = false);

    public void FinishDeployment(bool instantly = false);

    public void SetFoundItems(IEnumerable<BagItemData> items);
}
