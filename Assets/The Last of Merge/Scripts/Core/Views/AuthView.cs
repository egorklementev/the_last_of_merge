using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthView : MonoBehaviour, IAuthView
{
    public event Action<string, string> LoginClicked;
    public event Action<string, string> RegisterClicked;

    [SerializeField]
    private Button loginButton;

    [SerializeField]
    private Button registerButton;

    [SerializeField]
    private TMP_InputField loginInput;

    [SerializeField]
    private TMP_InputField passwordInput;

    [SerializeField]
    private CanvasGroup mainPanelGroup;

    void Start()
    {
        loginButton.onClick.AddListener(LoginButtonClicked);
        registerButton.onClick.AddListener(RegisterButtonClicked);
    }

    private void LoginButtonClicked()
    {
        // TODO: validate locally input field
        LoginClicked?.Invoke(loginInput.text, passwordInput.text);
    }

    private void RegisterButtonClicked()
    {
        // TODO: validate locally input field
        RegisterClicked?.Invoke(loginInput.text, passwordInput.text);
    }

    public void OnLoginSuccess()
    {
        mainPanelGroup.interactable = false;
        mainPanelGroup.blocksRaycasts = false;
        mainPanelGroup.alpha = 0f;
    }

    public void OnLoginFailure()
    {
        // TODO:
    }

    public void OnRegisterSuccess()
    {
        registerButton.interactable = false;
    }

    public void OnRegisterFailure()
    {
        // TODO:
    }
}
