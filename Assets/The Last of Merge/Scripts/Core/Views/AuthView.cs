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
        loginInput.interactable = false;
        passwordInput.interactable = false;
        loginButton.interactable = false;
        registerButton.interactable = false;
    }

    private void RegisterButtonClicked()
    {
        // TODO: validate locally input field
        RegisterClicked?.Invoke(loginInput.text, passwordInput.text);
        loginInput.interactable = false;
        passwordInput.interactable = false;
        loginButton.interactable = false;
        registerButton.interactable = false;
    }

    public void OnLoginSuccess()
    {
        mainPanelGroup.Toggle(false);
    }

    public void OnLoginFailure()
    {
        loginInput.interactable = true;
        passwordInput.interactable = true;
        loginButton.interactable = true;
        registerButton.interactable = true;
    }

    public void OnRegisterSuccess()
    {
        registerButton.interactable = false;
    }

    public void OnRegisterFailure()
    {
        loginInput.interactable = true;
        passwordInput.interactable = true;
        loginButton.interactable = true;
        registerButton.interactable = true;
    }
}
