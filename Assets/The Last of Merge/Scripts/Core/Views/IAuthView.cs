using System;

public interface IAuthView
{
    public event Action<string, string> LoginClicked;
    public event Action<string, string> RegisterClicked;

    public void OnLoginSuccess();
    public void OnLoginFailure();
    public void OnRegisterSuccess();
    public void OnRegisterFailure();
}
