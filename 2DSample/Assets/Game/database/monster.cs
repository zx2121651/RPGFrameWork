using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BaseData
{
    /// <summary>
    /// 定义“怪物”的数据结构。
    /// 继承自 myListStructNode 以利用框架的通用列表功能。
    /// </summary>
    [System.Serializable]
    public class monsterDictionary : myListStructNode
    {
        //怪物的生命值
        public int hp;
        // _hp 属性用于被框架的反射系统访问
        public int _hp
        {
            get { return hp; }
        }

        //怪物的攻击力
        public int attack;
        // _attack 属性用于被框架的反射系统访问
        public int _attack
        {
            get { return attack; }
        }

        //怪物的防御力
        public int defense;
        // _defense 属性用于被框架的反射系统访问
        public int _defense
        {
            get { return defense; }
        }

        // 引用怪物的战斗属性
        public ActorStats stats;
        public ActorStats _stats
        {
            get { return stats; }
        }
    }

    /// <summary>
    /// “怪物列表”的ScriptableObject定义。
    /// 这允许我们在Unity编辑器的 "Assets/Create" 菜单中创建怪物列表资源。
    /// </summary>
    [CreateAssetMenu(fileName = "New Monster List", menuName = "Database/Monster List")]
    public class monsterListDefine : myList<monsterDictionary>
    {
        // 此类为空，因为它仅用于创建ScriptableObject资源。
        // 所有逻辑均由其父类 myList<T> 和自定义编辑器处理。
    }

#if UNITY_EDITOR
    /// <summary>
    /// 为“怪物列表”提供一个自定义的Unity编辑器界面。
    /// </summary>
    [CustomEditor(typeof(monsterListDefine))]
    public class monsterInspector : myListInspector<monsterDictionary>
    {
        /// <summary>
        /// 重写此方法以绘制列表中每个怪物条目的编辑器UI。
        /// </summary>
        /// <param name="i">条目在列表中的索引</param>
        /// <param name="e">要编辑的怪物数据实例</param>
        protected override void drawInspector(int i, monsterDictionary e)
        {
            // 使用 EditorGUILayout 来创建各种命名的字段
            e.id = EditorGUILayout.TextField("怪物id", e.id);
            e.name = EditorGUILayout.TextField("怪物名", e.name);
            e.text = EditorGUILayout.TextField("描述文字", e.text);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("图片");
            e.img = (Sprite)EditorGUILayout.ObjectField(e.img, typeof(Sprite));
            EditorGUILayout.EndHorizontal();

            e.hp = EditorGUILayout.IntField("生命值", e.hp);
            e.attack = EditorGUILayout.IntField("攻击力", e.attack);
            e.defense = EditorGUILayout.IntField("防御力", e.defense);

            // 在编辑器中添加一个字段来分配 ActorStats 资源
            e.stats = (ActorStats)EditorGUILayout.ObjectField("战斗属性", e.stats, typeof(ActorStats), false);
        }
    }
#endif
}
