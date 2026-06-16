```mermaid
classDiagram
    direction TB

    %% Dependency Injection Layer
    namespace DI {
        class ServicesInstaller {
            +InstallBindings()
        }
    }

    %% Models Layer
    namespace Models {
        class BagItemData {
            +string itemId
            +string displayName
            +Sprite icon
        }
        class MergeRecipe
    }

    %% Views Layer
    namespace Views {
        class IBagSpaceView {
            <<interface>>
            +ShowItems()
        }
        class BagSpaceView {
            +UpdateUI()
        }
        class IItemSlotView {
            <<interface>>
            +OnItemDropped
        }
        class ItemSlotView {
            +ShowItem()
        }
    }

    %% Presenters Layer
    namespace Presenters {
        class BagSpaceInitializer {
            +Initialize()
        }
        class BagSpacePresenter {
            +OnItemDropped()
        }
        class ItemMergeController {
            +TryMerge()
        }
        class ItemSlot {
            +SetItem()
        }
        class IBagItemsProvider {
            <<interface>>
            +GetItems()
        }
    }

    %% Services Layer
    namespace Services {
        class Bootstrapper {
            +Construct()
        }
        class AddressablesManager {
            +LoadAssetAsync()
        }
        class DefaultBagItemsProvider {
            +GetItems()
        }
        class IMergeRecipeProvider {
            <<interface>>
            +GetRecipes()
        }
        class DefaultMergeRecipeProvider {
            +GetRecipes()
        }
    }

    %% Relationships
    ServicesInstaller ..> BagSpacePresenter : Injects
    ServicesInstaller ..> BagSpaceView : Injects
    ServicesInstaller ..> DefaultBagItemsProvider : Binds to IBagItemsProvider
    ServicesInstaller ..> DefaultMergeRecipeProvider : Binds to IMergeRecipeProvider
    
    Bootstrapper --> AddressablesManager : Initializes
    AddressablesManager --> DefaultBagItemsProvider : Provides Data
    AddressablesManager --> DefaultMergeRecipeProvider : Provides Data
    
    DefaultBagItemsProvider ..|> IBagItemsProvider
    DefaultMergeRecipeProvider ..|> IMergeRecipeProvider
    
    BagSpaceInitializer --> BagSpacePresenter : Creates & Sets up
    BagSpacePresenter --> IBagSpaceView : Updates UI
    BagSpacePresenter --> ItemMergeController : Delegates Logic
    
    ItemSlotView ..|> IItemSlotView
    ItemSlotView --> ItemSlot : Binds to Presenter
    ItemSlot --> IBagItemsProvider : Gets Item Data
    
    ItemMergeController --> IMergeRecipeProvider : Checks Rules
```