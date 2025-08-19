using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Func();

/// <summary>
/// 定义了所有可交互UI菜单需要实现的通用接口。
/// 包含了对键盘和鼠标操作的响应。
/// </summary>
public interface keyboardMenuInterface
{
    // 向上选择
    void up();
    // 向下选择
    void down();
    // 向左选择
    void left();
    // 向右选择
    void right();
    // 确认选项
    void doOption();
    // 取消/返回
    void cancel();
    // 改变窗口尺寸（用于响应全局设置变化）
    void changeSize();
    // 直接设置当前选中的项目（主要用于鼠标悬停）
    void setSelection(GameObject item);
}
