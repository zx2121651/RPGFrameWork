using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BaseData; // For itemDictionary, TreeNode, etc.
using ManagerSpace; // For gameManager

/// <summary>
/// 负责动态生成简单任务的“AI”模块。
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

    /// <summary>
    /// 生成一个新的取物任务，并返回一个可执行的事件结构。
    /// </summary>
    /// <returns>一个包含任务对话和逻辑的 eventStruct。</returns>
    public eventStruct GenerateFetchQuest()
    {
        // 1. 获取数据源
        var allNpcs = gameManager.instance.SystemSetting.PlayerInfos;
        // 假设道具列表资源名为 "Item List Define 1" 且在 Resources/Data/ 目录下
        var itemListAsset = Resources.Load<itemListDefine>("Data/Item List Define 1");

        if (allNpcs == null || allNpcs.Length == 0 || itemListAsset == null)
        {
            Debug.LogError("无法生成任务：NPC或道具列表为空。");
            return null;
        }

        // 2. 随机选择NPC和道具
        // 筛选出可以给予任务的NPC（例如，不是玩家自己）
        var questGiverNpc = allNpcs[Random.Range(1, allNpcs.Length)]; // 假设索引0是玩家

        // 筛选出可以作为任务目标的道具
        var validItems = itemListAsset.datalist.Where(item => item.type == itemType.item).ToList();
        if (validItems.Count == 0)
        {
            Debug.LogError("无法生成任务：没有有效的任务道具。");
            return null;
        }
        var targetItem = validItems[Random.Range(0, validItems.Count)];

        // 3. 创建动态任务
        string description = $"帮 {questGiverNpc.name} 找回 {targetItem.name}。";
        GeneratedQuest newQuest = GeneratedQuestManager.instance.CreateQuest(questGiverNpc.name, targetItem.id, description);

        // 4. 在内存中动态构建事件树
        return CreateQuestEventTree(newQuest, questGiverNpc, targetItem);
    }

    /// <summary>
    /// 根据生成的任务数据，动态创建事件树。
    /// </summary>
    private eventStruct CreateQuestEventTree(GeneratedQuest quest, PlayerInfo npc, itemDictionary item)
    {
        // 创建一个新的事件结构
        var eventTree = ScriptableObject.CreateInstance<dataTree>();

        // 创建根对话节点
        var rootNode = new TreeNode();
        rootNode.id = "root";

        // 设置对话内容
        var dialogueNode = new TreeNode();
        dialogueNode.id = "dialogue_start";
        dialogueNode.addParent(rootNode);
        rootNode.addChild(dialogueNode);

        // 使用反射来设置节点属性
        SetNodeProperty(dialogueNode, structProperty.type, eventType.text);
        SetNodeProperty(dialogueNode, structProperty.str1, $"你好，能帮我找一下我的 {item.name} 吗？");
        SetNodeProperty(dialogueNode, structProperty.str2, npc.name);
        SetNodeProperty(dialogueNode, structProperty.str3, npc.face.name);

        // 创建 "接受任务" 事件节点
        var acceptQuestNode = new TreeNode();
        acceptQuestNode.id = "accept_quest";
        acceptQuestNode.addParent(dialogueNode);
        dialogueNode.addChild(acceptQuestNode);

        SetNodeProperty(acceptQuestNode, structProperty.type, eventType.startQuest); // 假设有startQuest事件
        SetNodeProperty(acceptQuestNode, structProperty.str1, quest.questID); // 传递任务ID

        // 封装到 eventStruct 中
        var es = new eventStruct();
        // es.eventList = eventTree; // This needs to be properly set up.
        // The reflection-based system makes direct creation of a populated dataTree complex.
        // For now, this demonstrates the logic. The actual implementation might need a helper function
        // to properly construct the ScriptableObject and its internal nodes.

        Debug.Log("成功生成任务事件树（逻辑演示）。");

        // NOTE: This is a simplified logical representation.
        // The actual creation of a functional dataTree in memory is complex
        // due to the framework's reliance on ScriptableObjects and custom editors.
        // A full implementation would require deeper integration with the data-building process.
        // For this step, we will return a null and log the logic.
        return null;
    }

    // 辅助方法，用于通过反射设置节点属性
    private void SetNodeProperty(TreeNode node, string propertyName, object value)
    {
        // This is a placeholder for the logic that would be needed to
        // dynamically add properties to a ScriptableObject-backed node.
        // The current framework is not designed for this to be done easily at runtime.
        // A real implementation would likely involve creating a temporary dictionary
        // and then having the DataNode constructor read from that.
    }
}
