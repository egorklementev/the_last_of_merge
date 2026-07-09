using UnityEngine;
using Zenject;

/// <summary>
/// Performs DI on the main services in the game
/// </summary>
public class ServicesInstaller : MonoInstaller
{
    [SerializeField]
    private Canvas mainCanvas;

    [SerializeField]
    private Transform itemDragLayer;

    [SerializeField]
    private BagSpaceView bagSpaceView;

    [SerializeField]
    private RecipeScreenView recipeScreenView;

    [SerializeField]
    private DeploymentScreenView deploymentScreenView;

    [SerializeField]
    private AuthView authView;

    [SerializeField]
    private BagSpaceInitializer bagSpaceInitializer;

    [SerializeField]
    private NotificationContainerView notificationContainerView;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<Bootstrapper>().FromNew().AsSingle().NonLazy();

        Container.Bind<Canvas>().WithId("main_canvas").FromInstance(mainCanvas).AsSingle();
        Container.Bind<Transform>().WithId("drag_layer").FromInstance(itemDragLayer).AsSingle();

        Container.Bind<BagSpaceInitializer>().FromInstance(bagSpaceInitializer).AsSingle();
        Container.Bind<BagSpacePresenter>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<BagSpaceModel>().FromNew().AsSingle();
        Container.Bind<IBagSpaceView>().FromInstance(bagSpaceView).AsSingle();

        Container.Bind<RecipeScreenPresenter>().FromNew().AsSingle();
        Container.Bind<IRecipeScreenView>().FromInstance(recipeScreenView).AsSingle();

        Container.Bind<DeploymentManager>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<DeploymentScreenPresenter>().FromNew().AsSingle();
        Container.Bind<IDeploymentScreenView>().FromInstance(deploymentScreenView).AsSingle();

        Container.BindInterfacesAndSelfTo<ItemMergeController>().FromNew().AsSingle();

        Container.Bind<NetworkManager>().FromNew().AsSingle();
        Container.Bind<IAuthView>().FromInstance(authView).AsSingle();
        Container.BindInterfacesAndSelfTo<AuthorizationHandler>().FromNew().AsSingle().NonLazy();

        Container.Bind<BagSpaceNetworkManager>().FromNew().AsSingle();
        Container.Bind<DeploymentsNetworkManager>().FromNew().AsSingle();

        Container.BindInterfacesAndSelfTo<AddressablesManager>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<DefaultBagItemsProvider>().FromNew().AsSingle().NonLazy();
        Container
            .BindInterfacesAndSelfTo<DefaultMergeRecipeProvider>()
            .FromNew()
            .AsSingle()
            .NonLazy();

        Container.BindInterfacesAndSelfTo<NotificationManager>().FromNew().AsSingle().NonLazy();
        Container
            .BindInterfacesAndSelfTo<NotificationContainerView>()
            .FromInstance(notificationContainerView)
            .AsSingle()
            .NonLazy();

        // DEBUG
        //Container.Bind<IBagItemsProvider>().FromInstance(new DebugBagItemsProvider()).AsSingle();
        //Container
        //    .Bind<IMergeRecipeProvider>()
        //    .FromInstance(new DebugMergeRecipeProvider())
        //    .AsSingle();
    }
}
