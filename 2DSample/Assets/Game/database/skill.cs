using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BaseData
{
    /// <summary>
    /// 定义技能的类型，用于逻辑区分。
    /// </summary>
    public enum SkillType
    {
        Attack, // 攻击
        Heal,   // 治疗
        Buff,   // 增益
        Debuff, // 减益
        Other   // 其他
    }

    /// <summary>
    /// 定义“技能”的数据结构。
    /// </summary>
    [System.Serializable]
    public class skillDictionary : myListStructNode
    {
        // 技能的类型
        public SkillType skillType = SkillType.Attack;
        public SkillType _skillType
        {
            get { return skillType; }
        }

        // 技能的威力 (例如：伤害值、治疗量)
        public int power = 10;
        public int _power
        {
            get { return power; }
        }

        // 技能的MP (魔法值) 消耗
        public int mpCost = 5;
        public int _mpCost
        {
            get { return mpCost; }
        }

        // 技能可以关联一个公共事件，以实现复杂的逻辑效果（如播放动画、添加状态等）
        public string commonEvent = "";
        public string _commonEvent
        {
            get { return commonEvent; }
        }
    }

    /// <summary>
    /// “技能列表”的ScriptableObject定义。
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill List", menuName = "Database/Skill List")]
    public class skillListDefine : myList<skillDictionary>
    {
    }

#if UNITY_EDITOR
    /// <summary>
    /// 为“技能列表”提供一个自定义的Unity编辑器界面。
    /// </summary>
    [CustomEditor(typeof(skillListDefine))]
    public class skillInspector : myListInspector<skillDictionary>
    {
        /// <summary>
        /// 绘制列表中每个技能条目的编辑器UI。
        /// </summary>
        protected override void drawInspector(int i, skillDictionary e)
        {
            e.id = EditorGUILayout.TextField("技能ID", e.id);
            e.name = EditorGUILayout.TextField("技能名", e.name);
            e.text = EditorGUILayout.TextField("描述文字", e.text);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("图片 (图标)");
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
