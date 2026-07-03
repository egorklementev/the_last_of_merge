using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AuthorizationHandler : IInitializable
{
    public bool Authorized { get; set; } = false;

    [Inject]
    private IAuthView authView;

    [Inject]
    private NetworkManager networkManager;

    [Inject]
    private BagSpacePresenter bagSpacePresenter;

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
                Authorized = true;

                Debug.Log("[AuthorizationHandler]: Registration success!");
            }
            else
            {
                authView.OnRegisterFailure();
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
                Authorized = true;
                Debug.Log("[AuthorizationHandler]: Login success!");

                await UniTask.WaitUntil(() => bagSpacePresenter.Loaded);
                authView.OnLoginSuccess();
            }
            else
            {
                authView.OnLoginFailure();
            }
        });
    }
}
