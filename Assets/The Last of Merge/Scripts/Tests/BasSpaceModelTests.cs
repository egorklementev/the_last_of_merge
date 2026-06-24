using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class BagSpaceModelTests
{
    private BagSpaceModel model;
    private IBagItemsProvider mockProvider;
    private AuthorizationHandler mockAuth;
    private BagSpaceNetworkManager mockNetwork;

    [SetUp]
    public void SetUp()
    {
        model = new BagSpaceModel();
        mockProvider = Substitute.For<IBagItemsProvider>();

        mockAuth = Substitute.For<AuthorizationHandler>();

        mockNetwork = Substitute.For<BagSpaceNetworkManager>();

        var field = typeof(BagSpaceModel).GetField(
            "bagItemsProvider",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        field?.SetValue(model, mockProvider);

        var authField = typeof(BagSpaceModel).GetField(
            "authorizationHandler",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        authField?.SetValue(model, mockAuth);

        var netField = typeof(BagSpaceModel).GetField(
            "bagSpaceNetworkManager",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        netField?.SetValue(model, mockNetwork);
    }

    private bool GetLoadedField()
    {
        var field = typeof(BagSpaceModel).GetProperty(
            "Loaded",
            BindingFlags.Public | BindingFlags.Instance
        );
        return (bool)field?.GetValue(model);
    }

    [Test]
    public async Task GetDataForSlot_Initialized_ReturnsCorrectMappedItem()
    {
        var items = new List<BagItemData>
        {
            new BagItemData { Id = 100 },
            new BagItemData { Id = 101 },
            new BagItemData { Id = 102 },
            new BagItemData { Id = 103 },
            new BagItemData { Id = 104 },
            new BagItemData { Id = 105 },
            new BagItemData { Id = 106 },
        };

        mockAuth.Authorized = true;
        mockProvider.GetBagItemsAsync().Returns(UniTask.FromResult<IList<BagItemData>>(items));
        mockNetwork
            .SendInventoryRequest()
            .Returns(UniTask.FromResult(new Dictionary<int, BagItemData>()));

        model.Initialize();
        await UniTask.WaitUntil(() => GetLoadedField()).AsTask();

        Assert.IsNull(model.GetDataForSlot(99));
    }
}
