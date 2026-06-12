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
    private BagSpaceView bagSpaceView;

    [SerializeField]
    private BagSpaceInitializer bagSpaceInitializer;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<Bootstrapper>().FromNew().AsSingle().NonLazy();

        Container.Bind<Canvas>().WithId("main_canvas").FromInstance(mainCanvas).AsSingle();

        Container.Bind<BagSpaceInitializer>().FromInstance(bagSpaceInitializer).AsSingle();
        Container.Bind<BagSpacePresenter>().FromNew().AsSingle();
        Container.Bind<IBagSpaceView>().FromInstance(bagSpaceView).AsSingle();

        Container.BindInterfacesAndSelfTo<ItemMergeController>().FromNew().AsSingle();

        // DEBUG
        Container.Bind<IBagItemsProvider>().FromInstance(new DebugBagItemsProvider()).AsSingle();
        Container
            .Bind<IMergeRecipeProvider>()
            .FromInstance(new DebugMergeRecipeProvider())
            .AsSingle();
    }
}
