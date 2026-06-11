using UnityEngine;
using Zenject;

public class ServicesInstaller : MonoInstaller
{
    [SerializeField]
    private Canvas mainCanvas;

    public override void InstallBindings()
    {
        Container.Bind<Canvas>().WithId("main_canvas").FromInstance(mainCanvas).AsSingle();
    }
}
