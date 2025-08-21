using System;
using System.Collections.Generic;
using BaseData; // For TreeNodeInterface and other base types

/// <summary>
/// 一个 TreeNodeInterface 的实现，专为在运行时动态创建事件而设计。
/// 与基于ScriptableObject的TreeNode不同，这个类的数据是直接在代码中设置的。
/// </summary>
public class DynamicTreeNode : TreeNodeInterface
{
    private string _id;
    private Dictionary<string, object> _data = new Dictionary<string, object>();
    private List<TreeNodeInterface> _children = new List<TreeNodeInterface>();
    private List<TreeNodeInterface> _parents = new List<TreeNodeInterface>();

    public DynamicTreeNode(string id)
    {
        this._id = id;
        // 默认设置类型为none，以避免空引用
        SetData(structProperty.type, eventType.none);
    }

    public string getId() { return _id; }

    // --- Data Handling ---
    public T getData<T>(string name)
    {
        if (_data.ContainsKey(name)) return (T)_data[name];
        return default(T);
    }

    public object getData(string name)
    {
        if (_data.ContainsKey(name)) return _data[name];
        return null;
    }

    public Type getType(string name)
    {
        if (_data.ContainsKey(name)) return _data[name].GetType();
        return null;
    }

    public void SetData(string name, object value)
    {
        _data[name] = value;
    }

    public int getNodeType()
    {
        return (int)getData(structProperty.type);
    }

    // --- Tree Structure ---
    public void addChild(TreeNodeInterface c) { _children.Add(c); }
    public void addParent(TreeNodeInterface p) { _parents.Add(p); }
    public TreeNodeInterface getChild(string id) { return _children.Find(c => c.getId() == id); }
    public List<TreeNodeInterface> getChildren() { return _children; }
    public TreeNodeInterface getParent(string id) { return _parents.Find(p => p.getId() == id); }
    public List<TreeNodeInterface> getParents() { return _parents; }
    public bool removeChild(TreeNodeInterface c) { return _children.Remove(c); }
    public bool removeParent(TreeNodeInterface p) { return _parents.Remove(p); }
}
