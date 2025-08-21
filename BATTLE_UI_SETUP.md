# 战斗UI搭建指南

本指南将指导您如何使用我们创建的脚本，在Unity编辑器中搭建一个功能性的回合制战斗UI。

## 1. 创建UI Canvas

1.  在一个新场景或您希望测试战斗的场景中，右键点击 **层级(Hierarchy)** 窗口 -> `UI -> Canvas`。这会自动创建一个 `Canvas` 和一个 `EventSystem`。
2.  选中 `Canvas`，在 **检视(Inspector)** 窗口中，找到 `Canvas Scaler` 组件。
3.  将其 `UI Scale Mode` 设置为 `Scale With Screen Size`。
4.  将其 `Reference Resolution` 设置为 `X = 960`, `Y = 540` (或您希望的设计分辨率)。

## 2. 搭建战斗主界面 (BattleScene UI)

1.  在 `Canvas` 下创建一个空的 `GameObject`，命名为 `BattleScene_UI`。
2.  在 `BattleScene_UI` 下创建以下子对象：
    - **`PlayerHUDs`**: 一个空的 `GameObject`，用于存放所有玩家角色的HUD。
    - **`EnemyHUDs`**: 一个空的 `GameObject`，用于存放所有敌人的HUD。
    - **`CommandPanel`**: 一个空的 `GameObject`，用于存放指令菜单。

## 3. 制作HUD预制件

我们需要一个可以重复使用的HUD模板。

1.  在 `PlayerHUDs` 下，右键点击 -> `UI -> Panel`。将其命名为 `HUD_Template`。
2.  在 `HUD_Template` 下，添加 `UI -> Text` 和 `UI -> Slider` 来创建名称、HP和MP的显示。
3.  将我们创建的 `BattleHUD.cs` 脚本附加到 `HUD_Template` 对象上。
4.  将对应的 `Text` 和 `Slider` 组件拖拽到 `BattleHUD` 脚本的相应字段中。
5.  将设置好的 `HUD_Template` 从层级窗口拖拽到 **项目(Project)** 窗口的一个文件夹中，以将其创建为 **预制件(Prefab)**。然后可以删除场景中的 `HUD_Template`。

## 4. 制作指令菜单 (Command Menu)

1.  在 `CommandPanel` 下，右键点击 -> `UI -> Panel`，命名为 `CommandMenu`。
2.  为 `CommandMenu` 添加 `UI -> Vertical Layout Group` 组件来自动排列指令。
3.  在 `CommandMenu` 下，创建若干个 `UI -> Button` 对象，分别命名为 `Attack`, `Skill`, `Item`, `Flee`。
4.  将我们创建的 `CommandMenu.cs` 脚本附加到 `CommandMenu` 父对象上。
5.  `CommandMenu.cs` 继承自 `keyBoardMenuList`，您需要像设置其他列表一样，为其设置 `listItemPrefab` 等属性。您可以创建一个简单的按钮作为 `listItemPrefab`。

## 5. 整合到 BattleManager

最后，我们需要一个地方来统一管理这些UI。

1.  在场景中创建一个空的 `GameObject`，命名为 `BattleSystem`。
2.  将我们创建的 `BattleManager.cs` 和 `TargetSelection.cs` 脚本附加到 `BattleSystem` 对象上。
3.  打开 `BattleManager.cs` 脚本，添加公共字段来引用我们刚才创建的UI对象：
    ```csharp
    public GameObject battleSceneUI;
    public GameObject playerHUDsContainer;
    public GameObject enemyHUDsContainer;
    public GameObject hudPrefab;
    public CommandMenu commandMenu;
    public TargetSelection targetSelector;
    public GameObject selectionCursor; // 一个简单的图片，用于指示目标
    ```
4.  回到Unity编辑器，将场景中对应的对象拖拽到 `BattleSystem` 的这些新字段中。

通过以上步骤，您就有了一个基本的战斗UI结构。`BattleManager` 现在可以在战斗开始时，通过代码来激活/禁用 `battleSceneUI`，并使用 `hudPrefab` 来为每个战斗单位动态创建HUD，以及调用 `commandMenu.Setup()` 来显示指令菜单。
