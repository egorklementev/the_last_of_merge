using Cysharp.Threading.Tasks;
using Zenject;

public class AuthorizationHandler : IInitializable
{
    [Inject]
    private IAuthView authView;

    [Inject]
    private NetworkManager networkManager;

    public void Initialize()
    {
        authView.LoginClicked += OnLoginRequest;
        authView.RegisterClicked += OnRegisterRequest;
    }

    private void OnRegisterRequest(string login, string pass)
    {
        UniTask.Void(async () =>
        {
            var success = await networkManager.SendRegisterRequest(login, pass);
            if (success)
            {
                authView.OnRegisterSuccess();
            }
        });
    }

    private void OnLoginRequest(string login, string pass)
    {
        UniTask.Void(async () =>
        {
            var result = await networkManager.SendLoginRequest(login, pass);
            if (result.Success)
            {
                authView.OnLoginSuccess();
            }
        });
    }
}
