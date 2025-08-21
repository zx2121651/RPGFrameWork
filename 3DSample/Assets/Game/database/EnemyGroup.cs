using System.Collections.Generic;
using UnityEngine;
using BaseData; // For monsterDictionary

/// <summary>
/// 定义一个敌人队伍的 ScriptableObject。
/// 用于在编辑器中预设不同的敌人遭遇。
/// </summary>
[CreateAssetMenu(fileName = "New Enemy Group", menuName = "Database/Enemy Group")]
public class EnemyGroup : ScriptableObject
{
    public List<monsterDictionary> enemies;
}
