using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BaseData
{
    [System.Serializable]
    public class monsterDictionary : myListStructNode
    {
        public int hp;
        public int _hp
        {
            get { return hp; }
        }

        public int attack;
        public int _attack
        {
            get { return attack; }
        }

        public int defense;
        public int _defense
        {
            get { return defense; }
        }
    }

    [CreateAssetMenu(fileName = "New Monster List", menuName = "Database/Monster List")]
    public class monsterListDefine : myList<monsterDictionary>
    {
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(monsterListDefine))]
    public class monsterInspector : myListInspector<monsterDictionary>
    {
        protected override void drawInspector(int i, monsterDictionary e)
        {
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
        }
    }
#endif
}
