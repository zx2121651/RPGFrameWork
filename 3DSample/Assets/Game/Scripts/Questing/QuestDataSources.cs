using System.Collections.Generic;
using UnityEngine;
using BaseData; // For itemListDefine

/// <summary>
/// 负责从项目资源中加载和提供动态任务生成所需的数据。
/// </summary>
public class QuestDataSources : MonoBehaviour
{
    public static QuestDataSources instance;

    private List<itemListDefine> allItemLists;
    private List<GameObject> allNpcPrefabs;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllData()
    {
        // 加载所有道具列表
        allItemLists = new List<itemListDefine>(Resources.LoadAll<itemListDefine>(""));
        Debug.Log("Loaded " + allItemLists.Count + " item lists.");

        // 加载所有NPC预制件
        allNpcPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>("charactor/npcs"));
        Debug.Log("Loaded " + allNpcPrefabs.Count + " NPC prefabs.");
    }

    public itemDictionary GetRandomItem()
    {
        if (allItemLists == null || allItemLists.Count == 0) return null;

        // 随机选择一个道具列表
        var itemList = allItemLists[Random.Range(0, allItemLists.Count)];
        if (itemList.datalist == null || itemList.datalist.Count == 0) return null;

        // 从列表中随机选择一个道具
        return itemList.datalist[Random.Range(0, itemList.datalist.Count)];
    }

    public GameObject GetRandomNpc()
    {
        if (allNpcPrefabs == null || allNpcPrefabs.Count == 0) return null;

        return allNpcPrefabs[Random.Range(0, allNpcPrefabs.Count)];
    }
}
