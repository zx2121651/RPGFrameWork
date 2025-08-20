using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 定义列表的翻页方向。
    /// </summary>
    public enum listAxis
    {
        none = -1,      // 无翻页，单页循环
        Horizontal = 0, // 横向翻页
        Vertical = 1    // 纵向翻页
    }

    /// <summary>
    /// 列表菜单的核心实现，继承自 keyBoardMenu。
    /// 实现了可翻页、可动态更新内容、可嵌套的列表功能。
    /// </summary>
    public class keyBoardMenuList : keyBoardMenu, funcable
    {
        // 用于实例化列表项的预制件
        public listItemUI listItemPrefab;

        // 是否在Awake时就生成第一个item，而不是等待spawanItems调用
        public bool spawnFirst = false;

        // 列表的翻页方向
        public listAxis axis = listAxis.Vertical;

        // 列表的显示类型
        public showType showType = showType.all;

        // ===================== 事件和委托 =====================
        public delegate void moveItemFunc(string index);
        // 当取消选择时触发的回调
        public event Func cancelCallback;
        // 当列表项更新后触发的事件
        public event moveItemFunc afterUpdateItems;

        public delegate void moveCallFunc(string i);
        // 当光标移动时触发的回调
        public moveCallFunc callbackLeft, callbackRight, callbackUp, callbackDown;

        // ===================== 内部状态变量 =====================
        // 存储所有列表项UI组件的列表
        protected List<listItemUI> listItems = new List<listItemUI>();
        // 父节点的游戏对象引用
        protected GameObject parent;
        // 子菜单的引用，用于实现嵌套菜单
        [SerializeField]
        protected keyBoardMenuList child;
        // 用于从父节点接收的参数
        protected string param = "";
        // 当前页码
        protected int page = 0;
        // 总页数
        protected int allPage = 0;

        //=============================== 初始化操作 =================================
        protected override void onAwake()
        {
            base.onAwake();
            cursor.SetActive(false); // 默认隐藏光标
        }

        /// <summary>
        /// 根据 optionNum 和 listItemPrefab 生成列表项UI。
        /// </summary>
        virtual public void spawanItems()
        {
            if (listItemPrefab != null)
            {
                for (int j = 0; j < optionNum.y; j++)
                    for (int i = 0; i < optionNum.x; i++)
                    {
                        if(!spawnFirst)
                        {
                            if (i == 0 && j == 0)
                            {
                                listItemPrefab.NoneImg = data.NoneImg;
                                listItemPrefab.NoneText = data.NoneText;
                                listItems.Add(listItemPrefab);
                                continue;
                            }
                        }
                        GameObject g = Instantiate(listItemPrefab.gameObject) as GameObject;
                        g.transform.SetParent(gameObject.transform);
                        g.transform.localScale = listItemPrefab.transform.localScale;
                        g.transform.localPosition = startItemsPos + new Vector3(i * horizontalMove, j * verticalMove);
                        var f = g.GetComponent<listItemUI>();
                        f.NoneImg = data.NoneImg;
                        f.NoneText = data.NoneText;

                        // 为实例化的列表项挂载鼠标输入处理器
                        MouseInputHandler handler = g.GetComponent<MouseInputHandler>();
                        if (handler == null)
                        {
                            handler = g.AddComponent<MouseInputHandler>();
                        }
                        // 将其父菜单设置为本实例，以便回调
                        handler.parentMenu = this;

                        listItems.Add(f);
                    }
            }
        }

        //============================== 功能操作 =================================
        public override void cancel()
        {
            base.cancel();

            cursor.SetActive(false);
            changeState(nowState.window, parent); // 将控制权交还给父节点
            if (showType == showType.tree)
                ableDis(afterDis.hide); // 如果是树形结构，则隐藏自身
            parent.GetComponent<funcable>().doFunc();

            if (cancelCallback != null)
                cancelCallback();
        }

        public override void doOption()
        {
            base.doOption();

            // 如果有子菜单，则激活子菜单
            if (child != null)
                child.doFunc(getItemKey(getCursorPosIndex()), gameObject);
            // 执行当前选项的功能
            doItemFunc(getCursorPosIndex());
        }

        public override void left()
        {
            base.left();
            changeCursorPos(-1);
            if(callbackLeft!=null)
                callbackLeft(getItemKey(getCursorPosIndex()));
            updateItems();
        }

        public override void right()
        {
            base.right();
            changeCursorPos(1);
            if(callbackRight!=null)
                callbackRight(getItemKey(getCursorPosIndex()));
            updateItems();
        }

        public override void up()
        {
            base.up();
            changeCursorPos(0, -1);
            if(callbackUp!=null)
                callbackUp(getItemKey(getCursorPosIndex()));
            updateItems();
        }

        public override void down()
        {
            base.down();
            changeCursorPos(0, 1);
            if(callbackDown!=null)
                callbackDown(getItemKey(getCursorPosIndex()));
            updateItems();
        }

        /// <summary>
        /// 计算下一个光标位置，包含翻页逻辑。
        /// </summary>
        override protected Vector2Int getNextPos(int horizontal, int vertical = 0)
        {
            int[] p = { 0, 0 };
            int[] move = { horizontal, vertical };
            int[] cp = { cursorPos.x, cursorPos.y };
            int[] on = { optionNum.x, optionNum.y };

            if(axis == listAxis.none) // 单页循环
            {
                p[0] = (cp[0] + move[0] + on[0]) % on[0];
                p[1] = (cp[1] + move[1] + on[1]) % on[1];
            }
            else // 处理翻页
            {
                int main = (int)axis;
                int other = (main == 0 ? 1 : 0);

                p[other] = (cp[other] + move[other] + on[other]) % on[other];
                int next = cp[main] + move[main];
                p[main] = (next + on[main]) % on[main];
                if (next < 0) // 向上或向左超出边界，尝试向前翻页
                {
                    if (page == 0)
                    {
                        p[main] = 0; // 已经是第一页，不动
                    }
                    else
                    {
                        page -= 1; // 翻到上一页
                    }
                }
                else if (next >= on[main]) // 向下或向右超出边界，尝试向后翻页
                {
                    if (page == allPage - 1)
                    {
                        p[main] = cursorPos[main]; // 已经是最后一页，不动
                    }
                    else
                    {
                        page += 1; // 翻到下一页
                    }
                }
            }

            return new Vector2Int(p[0], p[1]);
        }

        //=============================== 被父节点操作 =================================
        /// <summary>
        /// 由父节点调用的入口函数，用于激活和传递参数。
        /// </summary>
        public void doFunc(string _param, GameObject _parent)
        {
            parent = _parent;
            param = _param;
            doFunc();
        }

        /// <summary>
        /// 更新列表项的内容。
        /// </summary>
        public void updateItems(string _param = "")
        {
            doUpdateItems(_param);

            if (afterUpdateItems != null)
                afterUpdateItems(getItemKey(getCursorPosIndex()));
        }

        /// <summary>
        /// 执行更新列表项的核心逻辑。
        /// </summary>
        virtual protected void doUpdateItems(string _param = "")
        {
            string p = string.IsNullOrEmpty(_param) ? param : _param;
            // 从数据源获取数据和键
            List<ListItemData> datas = data.getDatas(index, p);
            List<string> keys = data.getKeys(index, p);

            // 计算总页数
            allPage = (int)Math.Ceiling((float)datas.Count / getMaxNum());
            if (page > allPage)
            {
                page = 0;
                resetCurosr();
            }
            int startIndex = page * getMaxNum();
            clearAll(); // 清空所有项的旧数据
            if (datas != null && keys != null)
                for (int i = 0; i < listItems.Count; i++)
                {
                    if (i + startIndex < datas.Count && i + startIndex < keys.Count)
                    {
                        // 为列表项设置新的数据和键
                        setKey(i, keys[i + startIndex]);
                        setData(i, datas[i + startIndex]);
                    }
                    else
                        setData(i, null); // 如果没有数据，则清空
                }
        }

        override public void setDataSource(int _index, keyBoardMenusController dataSource)
        {
            base.setDataSource(_index, dataSource);
        }

        public List<listItemUI> getlistItems()
        {
            return listItems;
        }

        public int getlistLength()
        {
            return listItems.Count;
        }

        //=============================== 被子节点操作 =================================
        /// <summary>
        /// 由子节点调用的入口函数，用于激活自身。
        /// </summary>
        public void doFunc()
        {
            cursor.SetActive(true);
            changeState(nowState.window, gameObject);
        }

        //=============================== 对子节点操作 =================================
        public string getItemKey(int i)
        {
            return listItems[i].Key;
        }

        public void setKey(int index, string key)
        {
            if (index < 0 || index >= listItems.Count)
                return;
            listItems[index].setKeyIfNotNull(key);
        }

        public void setData(int index, ListItemData data)
        {
            if (index < 0 || index >= listItems.Count)
                return;
            listItems[index].setDataIfNotNull(data);
        }

        public void clearAll()
        {
            for (int i = 0; i < listItems.Count; i++)
            {
                listItems[i].clearData();
                listItems[i].clearKey();
            }
        }

        public void doItemFunc(int _index, string _param)
        {
            listItems[_index].doFunc(_param, gameObject);
        }

        public void doItemFunc(int _index)
        {
            listItems[_index].doFunc();
        }

        public void changeBtn(int _index,bool able)
        {
            var b = listItems[_index].gameObject.GetComponent<Button>();
            b.interactable = able;
        }

        /// <summary>
        /// 通过鼠标事件直接设置当前选中的项目。
        /// </summary>
        /// <param name="item">被鼠标悬停的UI项的GameObject</param>
        public void setSelection(GameObject item)
        {
            // 在列表中查找传入的UI项的索引
            int index = listItems.FindIndex(listItem => listItem.gameObject == item);

            // 如果找到了该项
            if (index != -1)
            {
                // 将一维索引转换为二维的光标坐标
                int x = index % optionNum.x;
                int y = index / optionNum.x;
                // 设置光标到新的位置
                setCursorPos(x, y);
            }
        }
    }
}
