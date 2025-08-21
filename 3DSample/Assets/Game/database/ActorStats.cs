using UnityEngine;

/// <summary>
/// 存储角色或怪物的核心战斗属性。
/// 创建为 ScriptableObject，以便可以作为资源文件在Unity编辑器中创建和分配。
/// </summary>
[CreateAssetMenu(fileName = "New Actor Stats", menuName = "Database/Actor Stats")]
public class ActorStats : ScriptableObject
{
    [Header("基本属性")]
    public int maxHp = 100; // 最大生命值
    public int maxMp = 50;  // 最大魔法值

    [Header("战斗属性")]
    public int attack = 10;     // 攻击力
    public int defense = 5;     // 防御力
    public int speed = 10;      // 速度，用于决定行动顺序
}
