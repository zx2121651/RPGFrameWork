using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 管理所有动态生成的任务。
/// 这是一个单例类。
/// </summary>
public class GeneratedQuestManager : MonoBehaviour
{
    public static GeneratedQuestManager instance;

    private List<GeneratedQuest> activeQuests = new List<GeneratedQuest>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 创建并注册一个新的动态任务。
    /// </summary>
    /// <returns>新创建的任务实例。</returns>
    public GeneratedQuest CreateQuest(string giver, string item, string description)
    {
        GeneratedQuest newQuest = new GeneratedQuest(giver, item, description);
        activeQuests.Add(newQuest);
        Debug.Log("新动态任务已创建: " + newQuest.questID);
        return newQuest;
    }

    /// <summary>
    /// 检查玩家是否已经有一个关于特定物品的未完成任务。
    /// </summary>
    public bool HasActiveQuestForItem(string itemID)
    {
        return activeQuests.Any(q => q.targetItemID == itemID && q.status == GeneratedQuestStatus.InProgress);
    }

    /// <summary>
    /// 检查玩家是否正在为某个NPC执行任务。
    /// </summary>
    public GeneratedQuest GetActiveQuestByGiver(string giverName)
    {
        return activeQuests.FirstOrDefault(q => q.questGiverName == giverName && q.status == GeneratedQuestStatus.InProgress);
    }

    /// <summary>
    /// 尝试完成一个任务。
    /// </summary>
    /// <param name="questID">要完成的任务ID。</param>
    public void CompleteQuest(string questID)
    {
        GeneratedQuest quest = activeQuests.FirstOrDefault(q => q.questID == questID);
        if (quest != null && quest.status == GeneratedQuestStatus.InProgress)
        {
            // 假设玩家已持有目标物品，此处应有检查逻辑
            // ManagerSpace.MIFactory.getMI().getItem(quest.targetItemID, -1); // 移除任务物品

            quest.status = GeneratedQuestStatus.Completed;
            Debug.Log("动态任务已完成: " + quest.questID);
        }
    }
}
