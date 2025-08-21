using System.Collections.Generic;

/// <summary>
/// 存储用于动态生成任务对话的字符串模板。
/// 使用 [NPC] 和 [ITEM] 作为占位符，它们将在运行时被替换。
/// </summary>
public static class QuestTextTemplates
{
    // 请求任务的对话
    public static List<string> Request = new List<string>
    {
        "你好，冒险者。我似乎把我的 [ITEM] 弄丢了，你能帮我找回来吗？",
        "我正在寻找 [ITEM]，你能帮我个忙吗？",
        "要是我能有 [ITEM] 就好了..."
    };

    // 接受任务后的对话
    public static List<string> Accept = new List<string>
    {
        "太感谢你了！我在等你回来。",
        "你真是个好人！拜托你了。",
        "祝你好运，冒险者！"
    };

    // 拒绝任务后的对话
    public static List<string> Refuse = new List<string>
    {
        "好吧，真遗憾...",
        "哦...那好吧。我再想想别的办法。",
        "没关系，我能理解。"
    };

    // 任务进行中的对话
    public static List<string> During = new List<string>
    {
        "你找到我的 [ITEM] 了吗？",
        "还没有找到 [ITEM] 吗？拜托你快一点。",
        "有什么进展吗？"
    };

    // 完成任务时的对话
    public static List<string> Complete = new List<string>
    {
        "啊，这就是我的 [ITEM]！太感谢你了！",
        "你找到了！我该怎么报答你呢？",
        "非常感谢你的帮助！"
    };

    // 获取随机模板的方法
    public static string GetRandomTemplate(List<string> templateList)
    {
        if (templateList == null || templateList.Count == 0) return "";
        return templateList[UnityEngine.Random.Range(0, templateList.Count)];
    }
}
