using BaseData;
using System;
using System.Collections.Generic;
using UnityEngine;

// =================================== 全局枚举定义 ===================================

/// <summary>
/// 特殊值，通常用于表示“全部”或“任意”。
/// </summary>
public enum SpecialValue
{
    all = -1
}

/// <summary>
/// 任务类型。
/// </summary>
public enum questType
{
    all = SpecialValue.all,
    main,   // 主线
    side,   // 支线
    hidden  // 隐藏
}

/// <summary>
/// 道具类型。
/// </summary>
public enum itemType
{
    all = SpecialValue.all,
    item,   // 普通道具
    notes,  // 笔记
    food,   // 食物
    gifts   // 礼物
}

/// <summary>
/// 任务状态。
/// </summary>
public enum questStatus
{
    doing,      // 进行中
    finished,   // 已完成
    failed,     // 已失败
    all         // 全部
}

/// <summary>
/// 事件的触发条件。
/// </summary>
public enum start
{
    Z,              // 按下确认键
    playerTouch,    // 玩家接触
    eventTouch,     // 事件接触（例如，一个移动的NPC撞到另一个NPC）
    auto            // 自动执行
}

/// <summary>
/// 角色朝向。
/// </summary>
public enum turn
{
    up,
    down,
    left,
    right,
    all // 任意方向
}

/// <summary>
/// 数值计算方式。
/// </summary>
public enum calValue
{
    add,        // 加
    minus,      // 减
    multiply,   // 乘
    divide,     // 除
    set         // 直接赋值
}

/// <summary>
/// 查找游戏对象的方式。
/// </summary>
public enum findType
{
    name, // 按名称
    tag   // 按标签
}

/// <summary>
/// 独立开关的标识符。
/// </summary>
public enum IndependentSwitch
{
    switchA,
    switchB,
    switchC,
    switchD
}

/// <summary>
/// 事件系统中所有可用的事件类型。
/// </summary>
// 添加新事件时，请从尾部添加，否则会打乱已保存的事件数据中的顺序。
public enum eventType
{
    none,
    debugLog,
    text,
    wait,
    beBlack,
    beWhite,
    startUserControl,
    stopUserControl,
    changeGlobalSwith,
    changeGlobalInt,
    changeGlobalDouble,
    playBgm,
    stopBgm,
    showPic,
    movePic,
    hidePic,
    changeScene,
    changeIndependentSwitch,
    commonEvent,
    textSpecial,
    playAnima,
    moveCamera,
    sbMoveToSw,
    flashScreen,
    shakeScreen,
    changeWeather,
    showSavePanel,
    changeItem,
    changeThingPos,
    playSE,
    playBgs,
    stopBgs,
    changeThingActive,
    changeTurn,
    startQuest,
    changeQuest,
    If,
    setFollow,
    changeTeam,
    changeGlobalVector3,
    ShowChoices, // 显示多项选择框
    StartBattle // 开始一场战斗
}

/// <summary>
/// 角色组件类型。
/// </summary>
public enum actorComponentType
{
    basicMove,
    basicBattle,
    followBehaviour
}

/// <summary>
/// 事件条件判断的变量类型。
/// </summary>
public enum conditionType
{
    Switch,
    Int,
    Float,
    IndependentSwitch
}

/// <summary>
/// 多个条件之间的连接逻辑。
/// </summary>
public enum connect
{
    and, // 与
    or   // 或
}

/// <summary>
/// 数值比较方式。
/// </summary>
public enum compare
{
    larger,     // 大于
    less,       // 小于
    equal,      // 等于
    notEqual    // 不等于
}

// =================================== 属性名常量 ===================================
// 使用常量类来存储属性名字符串，可以避免手写错误并方便重构。

/// <summary>
/// 数据字典的名称。
/// </summary>
public class dictionaryName
{
    public static string before = "_";

    // 游戏数据字典
    public static string items = "items";
    public static string quests = "quests";

    // 全局变量字典
    public static string ints = "ints";
    public static string doubles = "doubles";
    public static string switchs = "switchs";
    public static string vec3s = "vec3s";

    // 资源字典
    public static string audio = "audio";
    public static string image = "image";
    public static string prefab = "prefab";
}

/// <summary>
/// 通用属性名。
/// </summary>
public class propertyName
{
    public static string name = dictionaryName.before + "name";
    public static string type = dictionaryName.before + "type";
    public static string value = dictionaryName.before + "value";
    public static string pitch = dictionaryName.before + "pitch";
}

/// <summary>
/// 道具的属性名。
/// </summary>
public class itemProperty
{
    public static string name = dictionaryName.before + "name";
    public static string text = dictionaryName.before + "text";
    public static string img = dictionaryName.before + "img";
    public static string type = dictionaryName.before + "type";
    public static string times = dictionaryName.before + "times";
    public static string commonEvent = dictionaryName.before + "commonEvent";
    public static string eventNum = dictionaryName.before + "eventNum";
}

/// <summary>
/// 任务的属性名。
/// </summary>
public class questProperty
{
    public static string name = dictionaryName.before + "name";
    public static string text = dictionaryName.before + "text";
    public static string img = dictionaryName.before + "img";
    public static string type = dictionaryName.before + "type";
    public static string status = dictionaryName.before + "status";
    public static string hard = dictionaryName.before + "hard";
    public static string steps = dictionaryName.before + "steps";
    public static string stepNum = dictionaryName.before + "stepNum";
}

/// <summary>
/// 事件节点中用于存储参数的通用属性名。
/// </summary>
public class structProperty
{
    public static string type = dictionaryName.before + "type";

    public static string float1 = dictionaryName.before + "float1";
    public static string float2 = dictionaryName.before + "float2";
    public static string float3 = dictionaryName.before + "float3";
    public static string float4 = dictionaryName.before + "float4";

    public static string str1 = dictionaryName.before + "str1";
    public static string str2 = dictionaryName.before + "str2";
    public static string str3 = dictionaryName.before + "str3";
    public static string str4 = dictionaryName.before + "str4";

    public static string int1 = dictionaryName.before + "int1";

    public static string bool1 = dictionaryName.before + "bool1";
    public static string bool2 = dictionaryName.before + "bool2";
    public static string bool3 = dictionaryName.before + "bool3";
    public static string bool4 = dictionaryName.before + "bool4";

    public static string img1 = dictionaryName.before + "img1";
    public static string img2 = dictionaryName.before + "img2";
    public static string img3 = dictionaryName.before + "img3";

    public static string vec1 = dictionaryName.before + "vec1";
    public static string vec2 = dictionaryName.before + "vec2";

    public static string color1 = dictionaryName.before + "color1";

    public static string howToCal = dictionaryName.before + "howToCal";
    public static string turn = dictionaryName.before + "turn";
    public static string findType = dictionaryName.before + "findType";
    public static string questStatus = dictionaryName.before + "questStatus";
    public static string independentSwitch = dictionaryName.before + "independentSwitch";
    public static string compare = dictionaryName.before + "compare";
    public static string conditionType = dictionaryName.before + "conditionType";
}

// =================================== 数据结构体 ===================================

/// <summary>
/// 封装一个事件页的数据和状态。
/// </summary>
[System.Serializable]
public class eventStruct
{
    // 事件树 ScriptableObject 的引用
    public dataTree eventList;

    // 是否可以反复执行
    public bool loop = false;

    public EventListInterface EventListInterface
    {
        get
        {
            return (EventListInterface)eventList;
        }
    }

    // 本事件当前执行到的节点
    private TreeNodeInterface thisNow = null;
    public TreeNodeInterface ThisNow
    {
        get
        {
            return thisNow;
        }

        set
        {
            thisNow = value;
        }
    }

    // 事件页是否已完成
    private bool finish = false;
    public bool Finish
    {
        get
        {
            return finish;
        }

        set
        {
            finish = value;
        }
    }

    // 事件页是否需要被移除
    private bool remove = false;
    public bool Remove
    {
        get
        {
            return remove;
        }

        set
        {
            remove = value;
        }
    }

    public void reset()
    {
        thisNow = null;
        finish = false;
        remove = false;
    }
}

/// <summary>
/// 封装一个事件触发条件。
/// </summary>
[System.Serializable]
public struct conditionDictionary
{
    public conditionType type;
    public string name;
    public connect thisConnect;
    public compare compare;
    public string value;
}

// =================================== 接口和基类 ===================================

/// <summary>
/// 朝向相关的工具类。
/// </summary>
public class turnTool
{
    public static turn get(Vector3 v)
    {
        return get(new Vector2Int((int)v.x, (int)v.y));
    }

    public static turn get(Vector2Int turn)
    {
        if (turn == new Vector2Int(0, -1))
            return global::turn.down;
        else if (turn == new Vector2Int(0, 1))
            return global::turn.up;
        else if (turn == new Vector2Int(-1, 0))
            return global::turn.left;
        else if (turn == new Vector2Int(1, 0))
            return global::turn.right;
        else
            return global::turn.all;
    }

    public static Vector2Int get(turn t)
    {
        if (t == global::turn.down)
            return new Vector2Int(0, -1);
        else if (t == global::turn.up)
            return new Vector2Int(0, 1);
        else if (t == global::turn.left)
            return new Vector2Int(-1, 0);
        else if (t == global::turn.right)
            return new Vector2Int(1, 0);
        else
            return Vector2Int.zero;
    }
}

/// <summary>
/// 数据节点的基础接口，定义了通过反射获取数据的方法。
/// </summary>
public interface dataNodeInterface
{
    Type getType(string name);
    object getData(string name);
    T getData<T>(string name);
}

/// <summary>
/// 列表节点的接口。
/// </summary>
public interface ListNodeInterface : dataNodeInterface
{
    string getId();
    int getNodeType();
}

/// <summary>
/// 树节点的接口，用于事件系统。
/// </summary>
public interface TreeNodeInterface : ListNodeInterface
{
    TreeNodeInterface getChild(string id);
    void addChild(TreeNodeInterface c);
    bool removeChild(TreeNodeInterface c);
    List<TreeNodeInterface> getChildren();

    TreeNodeInterface getParent(string id);
    void addParent(TreeNodeInterface p);
    bool removeParent(TreeNodeInterface c);
    List<TreeNodeInterface> getParents();
      
}

/// <summary>
/// ScriptableObject 基类，用于创建列表类型的资源。
/// </summary>
public class dataList : ScriptableObject
{
    protected List<ListNodeInterface> list = new List<ListNodeInterface>();

    protected Func toInterfaceFunc;

    public void init()
    {
        if (toInterfaceFunc != null)
            toInterfaceFunc();
    }

    public List<ListNodeInterface> getList()
    {
        return list;
    }

    public ListNodeInterface getListNode(string id)
    {
        foreach (var i in list)
        {
            if (i.getId() == id)
                return i;
        }
        return null;
    }
}

/// <summary>
/// ScriptableObject 基类，用于创建树类型的资源（如事件）。
/// </summary>
public class dataTree : ScriptableObject
{
    protected TreeNodeInterface root;

    protected Func toInterfaceFunc;

    public void init()
    {
        if (toInterfaceFunc != null)
            toInterfaceFunc();
    }
    public TreeNodeInterface getRoot()
    {
        return root;
    }
}

/// <summary>
/// 事件列表的接口，定义了检查触发条件的方法。
/// </summary>
public interface EventListInterface
{
    start getHowToStart();
    bool[] getIndependentSwitchs();
    void setIndependentSwitchs(bool[] ind);
    bool checkEventConditions();
    bool checkCondition(conditionDictionary condition, bool[] independentSwitchs);
}

