/// <summary>
/// 动态生成的任务的状态。
/// </summary>
public enum GeneratedQuestStatus
{
    InProgress,
    Completed
}

/// <summary>
/// 用于存储一个动态生成的任务的所有信息的类。
/// </summary>
public class GeneratedQuest
{
    public string questID;
    public string questGiverName;
    public string targetItemID;
    public string questDescription;
    public GeneratedQuestStatus status;

    public GeneratedQuest(string giver, string item, string description)
    {
        // 使用NPC和物品名生成一个唯一的ID
        this.questID = "GEN_" + giver + "_" + item;
        this.questGiverName = giver;
        this.targetItemID = item;
        this.questDescription = description;
        this.status = GeneratedQuestStatus.InProgress;
    }
}
