using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ItemMergeControllerTests
{
    private ItemMergeController _controller;
    private IMergeRecipeProvider _mockRecipeProvider;
    private IBagItemsProvider _mockBagItemsProvider;

    [SetUp]
    public void SetUp()
    {
        _mockRecipeProvider = Substitute.For<IMergeRecipeProvider>();
        _mockBagItemsProvider = Substitute.For<IBagItemsProvider>();

        _controller = new ItemMergeController();

        // Inject mocks into private fields using reflection
        InjectField(_controller, "mergeRecipeProvider", _mockRecipeProvider);
        InjectField(_controller, "bagItemsProvider", _mockBagItemsProvider);
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

    private async Task InitializeControllerWithRecipes(IList<MergeRecipe> recipes)
    {
        _mockRecipeProvider.GetRecipesAsync().Returns(UniTask.FromResult(recipes));
        _controller.Initialize();

        // Wait for the async UniTask.Void initialization to finish
        await UniTask.WaitUntil(() => GetRecipesField() != null).AsTask();
    }

    private IList<MergeRecipe> GetRecipesField()
    {
        var field = typeof(ItemMergeController).GetField(
            "recipes",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        return (IList<MergeRecipe>)field?.GetValue(_controller);
    }

    [Test]
    public async Task TryMergeSlots_SameSlot_ReturnsSameSlot()
    {
        await InitializeControllerWithRecipes(new List<MergeRecipe>());
        var slot1 = CreateMockSlot(1, new BagItemData { Id = 1 });
        var slot2 = CreateMockSlot(1, new BagItemData { Id = 2 }); // Same SlotId

        var result = await _controller.TryMergeSlots(slot1, slot2).AsTask();

        Assert.AreEqual(ItemMergeController.MergeResultType.SAME_SLOT, result.MergeResultType);
    }

    [Test]
    public async Task TryMergeSlots_EmptySlot_ReturnsSingleItem()
    {
        await InitializeControllerWithRecipes(new List<MergeRecipe>());
        var slot1 = CreateMockSlot(1, new BagItemData { Id = 1 });
        var slot2 = CreateMockSlot(2, null); // Empty slot

        var result = await _controller.TryMergeSlots(slot1, slot2).AsTask();

        Assert.AreEqual(ItemMergeController.MergeResultType.SINGLE_ITEM, result.MergeResultType);
    }

    [Test]
    public async Task TryMergeSlots_NoRecipe_ReturnsNoRecipeFound()
    {
        await InitializeControllerWithRecipes(new List<MergeRecipe>());
        var slot1 = CreateMockSlot(1, new BagItemData { Id = 1 });
        var slot2 = CreateMockSlot(2, new BagItemData { Id = 99 });

        var result = await _controller.TryMergeSlots(slot1, slot2).AsTask();

        Assert.AreEqual(
            ItemMergeController.MergeResultType.NO_RECIPE_FOUND,
            result.MergeResultType
        );
    }

    [Test]
    public async Task TryMergeSlots_RecipeFound_ReturnsSuccess()
    {
        var item1 = new BagItemData { Id = 1 };
        var item2 = new BagItemData { Id = 2 };
        var resultItem = new BagItemData { Id = 3 };

        var recipe = new MergeRecipe
        {
            Item1 = item1,
            Item2 = item2,
            ResultingItem = resultItem,
        };
        await InitializeControllerWithRecipes(new List<MergeRecipe> { recipe });

        _mockBagItemsProvider.GetBagItemById(resultItem.Id).Returns(UniTask.FromResult(resultItem));

        Assert.AreEqual(1, (await _mockRecipeProvider.GetRecipesAsync()).Count);

        var slot1 = CreateMockSlot(1, item1);
        var slot2 = CreateMockSlot(2, item2);

        var result = await _controller.TryMergeSlots(slot1, slot2).AsTask();

        Assert.AreEqual(ItemMergeController.MergeResultType.SUCCESS, result.MergeResultType);
        Assert.AreEqual(resultItem, result.MergeResultItem);
    }
}
