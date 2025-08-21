using UnityEngine;
using UI; // For keyBoardMenuList

/// <summary>
/// 战斗中的指令菜单（攻击、技能等）。
/// 继承自 keyBoardMenuList 以复用菜单导航和输入逻辑。
/// </summary>
public class CommandMenu : keyBoardMenuList
{
    public enum Command { Attack, Skill, Item, Flee }

    // 当玩家选择一个指令时触发的回调
    public System.Action<Command> onCommandSelected;

    /// <summary>
    /// 当玩家确认选择时调用。
    /// </summary>
    public override void doOption()
    {
        base.doOption(); // 播放音效

        // 获取当前选择的索引，并转换为指令枚举
        int selectedIndex = getCursorPosIndex();
        Command selectedCommand = (Command)selectedIndex;

        // 触发回调
        if (onCommandSelected != null)
        {
            onCommandSelected(selectedCommand);
        }

        // 通常选择后会隐藏此菜单
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 设置菜单的初始状态并显示。
    /// </summary>
    public void Setup()
    {
        // 假设指令菜单有4个固定的选项
        optionNum = new Vector2Int(1, 4);

        // 确保列表项已生成
        if (listItems.Count < 4)
        {
            spawanItems();
        }

        // 设置每个列表项的文本
        setData(0, new ListItemData(new string[] { "攻击" }));
        setData(1, new ListItemData(new string[] { "技能" }));
        setData(2, new ListItemData(new string[] { "道具" }));
        setData(3, new ListItemData(new string[] { "逃跑" }));

        // 激活菜单
        gameObject.SetActive(true);
        doFunc();
    }
}
