using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BaseData
{
    public enum SkillType
    {
        Attack,
        Heal,
        Buff,
        Debuff,
        Other
    }

    [System.Serializable]
    public class skillDictionary : myListStructNode
    {
        public SkillType skillType = SkillType.Attack;
        public SkillType _skillType
        {
            get { return skillType; }
        }

        public int power = 10;
        public int _power
        {
            get { return power; }
        }

        public int mpCost = 5;
        public int _mpCost
        {
            get { return mpCost; }
        }

        public string commonEvent = "";
        public string _commonEvent
        {
            get { return commonEvent; }
        }
    }

    [CreateAssetMenu(fileName = "New Skill List", menuName = "Database/Skill List")]
    public class skillListDefine : myList<skillDictionary>
    {
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(skillListDefine))]
    public class skillInspector : myListInspector<skillDictionary>
    {
        protected override void drawInspector(int i, skillDictionary e)
        {
            e.id = EditorGUILayout.TextField("技能ID", e.id);
            e.name = EditorGUILayout.TextField("技能名", e.name);
            e.text = EditorGUILayout.TextField("描述文字", e.text);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("图片");
            e.img = (Sprite)EditorGUILayout.ObjectField(e.img, typeof(Sprite));
            EditorGUILayout.EndHorizontal();

            e.skillType = (SkillType)EditorGUILayout.EnumPopup("技能类型", e.skillType);
            e.power = EditorGUILayout.IntField("威力", e.power);
            e.mpCost = EditorGUILayout.IntField("MP消耗", e.mpCost);
            e.commonEvent = EditorGUILayout.TextField("关联公共事件", e.commonEvent);
        }
    }
#endif
}
