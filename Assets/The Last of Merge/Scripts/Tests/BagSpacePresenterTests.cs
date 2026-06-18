using System.Collections.Generic;
using System.Reflection;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class BagSpacePresenterTests
{
    private BagSpacePresenter _presenter;
    private IBagSpaceView _mockView;
    private BagSpaceModel _mockModel;

    [SetUp]
    public void SetUp()
    {
        _presenter = new BagSpacePresenter();
        _mockView = Substitute.For<IBagSpaceView>();

        // Note: To mock BagSpaceModel easily, its methods should be 'virtual'
        // or you should extract an IBagSpaceModel interface.
        _mockModel = Substitute.For<BagSpaceModel>();

        InjectField(_presenter, "bagSpaceView", _mockView);
        InjectField(_presenter, "bagSpaceModel", _mockModel);
    }

    private void InjectField(object target, string fieldName, object value)
    {
        var field = target
            .GetType()
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(target, value);
    }

    private ItemSlot CreateMockSlot(int slotId, BagItemData itemData)
    {
        var slot = new ItemSlot();
        slot.SlotId = slotId;
        slot.SlotView = Substitute.For<IItemSlotView>();
        if (itemData != null)
            slot.SetItem(itemData);
        else
            slot.SetEmpty();
        return slot;
    }

    [Test]
    public void GetRandomFreeSlot_NoFreeSlots_ReturnsNull()
    {
        // Arrange
        var slots = new List<ItemSlot>
        {
            CreateMockSlot(1, new BagItemData { Id = 1 }),
            CreateMockSlot(2, new BagItemData { Id = 2 }),
        };
        InjectField(_presenter, "itemSlots", slots);

        // Act
        var result = _presenter.GetRandomFreeSlot();

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void GetRandomFreeSlot_HasFreeSlots_ReturnsFreeSlot()
    {
        // Arrange
        var freeSlot = CreateMockSlot(3, null);
        var slots = new List<ItemSlot> { CreateMockSlot(1, new BagItemData { Id = 1 }), freeSlot };
        InjectField(_presenter, "itemSlots", slots);

        // Act
        var result = _presenter.GetRandomFreeSlot();

        // Assert
        Assert.AreEqual(freeSlot, result);
    }
}
