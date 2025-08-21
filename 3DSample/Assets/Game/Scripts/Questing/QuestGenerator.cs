using UnityEngine;
using System.Collections.Generic;
using BaseData; // For dataTree and other core types
using System;

// This helper class is defined inside the same file to overcome tool limitations.
// Ideally, it would be in its own file.
public class DynamicTreeNode : TreeNodeInterface
{
    private string _id;
    private Dictionary<string, object> _data = new Dictionary<string, object>();
    private List<TreeNodeInterface> _children = new List<TreeNodeInterface>();
    private List<TreeNodeInterface> _parents = new List<TreeNodeInterface>();

    public DynamicTreeNode(string id) { this._id = id; }
    public string getId() { return _id; }

    public T getData<T>(string name) {
        if (_data.ContainsKey(name)) return (T)_data[name];
        return default(T);
    }
    public object getData(string name) {
        if (_data.ContainsKey(name)) return _data[name];
        return null;
    }
     public Type getType(string name) {
        if (_data.ContainsKey(name)) return _data[name].GetType();
        return null;
    }
    public void SetData(string name, object value) { _data[name] = value; }

    public int getNodeType() { return (int)getData(structProperty.type); }
    public void addChild(TreeNodeInterface c) { _children.Add(c); }
    public void addParent(TreeNodeInterface p) { _parents.Add(p); }
    public TreeNodeInterface getChild(string id) { return _children.Find(c => c.getId() == id); }
    public List<TreeNodeInterface> getChildren() { return _children; }
    public TreeNodeInterface getParent(string id) { return _parents.Find(p => p.getId() == id); }
    public List<TreeNodeInterface> getParents() { return _parents; }
    public bool removeChild(TreeNodeInterface c) { return _children.Remove(c); }
    public bool removeParent(TreeNodeInterface p) { return _parents.Remove(p); }
}


/// <summary>
/// AI动态任务生成器。
/// 负责在运行时动态地创建和组装任务事件。
/// </summary>
public class QuestGenerator : MonoBehaviour
{
    public static QuestGenerator instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public dataTree GenerateFetchQuest()
    {
        var npc = QuestDataSources.instance.GetRandomNpc();
        var item = QuestDataSources.instance.GetRandomItem();
        var rewardItem = QuestDataSources.instance.GetRandomItem();

        if (npc == null || item == null || rewardItem == null || item.id == rewardItem.id)
        {
            Debug.LogError("无法生成任务：缺少有效的数据。");
            return null;
        }

        string npcName = npc.name;
        string itemName = item.name;
        string questId = "GeneratedQuest_" + System.Guid.NewGuid().ToString();

        dataTree newQuestTree = ScriptableObject.CreateInstance<dataTree>();
        var root = new DynamicTreeNode("root");
        newQuestTree.setRoot(root);

        // Build the event tree using helper methods
        string requestText = QuestTextTemplates.GetRandomTemplate(QuestTextTemplates.Request).Replace("[ITEM]", itemName);
        var dialogueNode = CreateTextNode(root, "dialogue1", requestText, npcName);
        var choicesNode = CreateNode(dialogueNode, "choices1", eventType.ShowChoices);

        // Accept Branch
        var acceptBranch = CreateNode(choicesNode, "acceptBranch", eventType.none, "好的，我帮你找。");
        var acceptTextNode = CreateTextNode(acceptBranch, "acceptText", QuestTextTemplates.GetRandomTemplate(QuestTextTemplates.Accept), npcName);
        var startQuestNode = CreateNode(acceptTextNode, "startQuest", eventType.startQuest, questId);

        // Refuse Branch
        var refuseBranch = CreateNode(choicesNode, "refuseBranch", eventType.none, "抱歉，我没空。");
        CreateTextNode(refuseBranch, "refuseText", QuestTextTemplates.GetRandomTemplate(QuestTextTemplates.Refuse), npcName);

        Debug.Log("成功生成任务： " + npcName + " 需要 " + itemName);
        return newQuestTree;
    }

    // ================== Helper Methods for Node Creation ==================

    private DynamicTreeNode CreateNode(TreeNodeInterface parent, string id, eventType type, string str1 = null)
    {
        var node = new DynamicTreeNode(id);
        node.SetData(structProperty.type, type);
        if (str1 != null) node.SetData(structProperty.str1, str1);
        parent.addChild(node);
        node.addParent(parent);
        return node;
    }

    private DynamicTreeNode CreateTextNode(TreeNodeInterface parent, string id, string text, string characterName)
    {
        var node = CreateNode(parent, id, eventType.text);
        node.SetData(structProperty.str1, text);
        node.SetData(structProperty.str2, characterName);
        return node;
    }
}
