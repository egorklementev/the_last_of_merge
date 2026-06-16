using System;
using UnityEngine;
using UnityEngine.UI;

public class DeploymentScreenView : MonoBehaviour, IDeploymentScreenView
{
    public event Action DeployButtonClicked;

    [SerializeField]
    private Button deployButton;

    void Start()
    {
        deployButton.onClick.AddListener(() => DeployButtonClicked?.Invoke());
    }
}

public interface IDeploymentScreenView
{
    public event Action DeployButtonClicked;
}
