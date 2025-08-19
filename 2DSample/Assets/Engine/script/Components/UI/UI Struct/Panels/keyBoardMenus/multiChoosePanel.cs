using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 一个用于显示多个选项并等待用户选择的UI面板。
    /// 主要用于实现游戏中的对话选择、分支剧情等功能。
    /// </summary>
    public class multiChoosePanel : keyBoardMenuList
    {
        // 存储当玩家做出选择后需要执行的回调函数
        private Action<int> onChoiceMade;

        /// <summary>
        /// 显示并设置选项面板。
        /// </summary>
        /// <param name="choices">要显示的选项文本列表。</param>
        /// <param name="callback">玩家做出选择后要调用的回调函数，参数为所选项的索引。</param>
        public void showChoices(List<string> choices, Action<int> callback)
        {
            this.onChoiceMade = callback;

            // 根据选项数量动态设置菜单的尺寸
            optionNum = new Vector2Int(1, choices.Count);

            // 如果当前的UI列表项数量不足，则生成新的列表项
            if (listItems.Count < choices.Count)
            {
                spawanItems();
            }

            // 使用选项文本填充UI列表
            List<ListItemData> datas = new List<ListItemData>();
            List<string> keys = new List<string>();
            for (int i = 0; i < choices.Count; i++)
            {
                // 将字符串包装在 ListItemData 中
                datas.Add(new ListItemData(new string[] { choices[i] }));
                keys.Add(i.ToString()); // 使用索引作为内部Key
            }

            // 更新UI显示 (这部分逻辑从 keyBoardMenuList 继承和调整)
            allPage = (int)Math.Ceiling((float)datas.Count / getMaxNum());
            if (page > allPage)
            {
                page = 0;
                resetCurosr();
            }
            int startIndex = page * getMaxNum();
            clearAll();
            if (datas != null && keys != null)
            {
                for (int i = 0; i < listItems.Count; i++)
                {
                    listItems[i].gameObject.SetActive(true);
                    if (i < datas.Count && i < keys.Count)
                    {
                        setKey(i, keys[i + startIndex]);
                        setData(i, datas[i + startIndex]);
                    }
                    else
                    {
                        // 隐藏未使用的列表项
                        listItems[i].gameObject.SetActive(false);
                    }
                }
            }

            // 激活面板并接管用户输入
            gameObject.SetActive(true);
            doFunc(); // 调用父类方法以激活菜单状态
        }

        /// <summary>
        /// 重写确认操作的逻辑。
        /// </summary>
        public override void doOption()
        {
            base.doOption(); // 播放确认音效

            int selectedIndex = getCursorPosIndex();

            // 如果回调函数已设置，则调用它，并传入所选的索引
            if (onChoiceMade != null)
            {
                onChoiceMade(selectedIndex);
            }

            // 选择后销毁面板
            Destroy(gameObject);
        }

        /// <summary>
        /// 重写取消操作的逻辑。
        /// </summary>
        public override void cancel()
        {
            // 当前设计为不允许取消，以强制玩家做出选择。
            // 可以根据需要修改此处的逻辑，例如允许取消并返回一个特定值(-1)。
            data.playSE(unable, true);
        }
    }
}
