# 自定义数据结构文档

本框架支持用户创建自定义的数据结构，以便在游戏中使用。本文档将以创建一个“怪物”数据结构为例，详细说明如何操作。

## 1. 创建数据结构脚本

首先，你需要创建一个 C# 脚本来定义你的数据结构。建议将这些脚本存放在 `Assets/Game/database` 目录下，以便于管理。

例如，我们在 `Assets/Game/database/` 目录下创建了一个名为 `monster.cs` 的文件，其内容如下：

```csharp
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BaseData
{
    // 1. 定义你的数据结构
    [System.Serializable]
    public class monsterDictionary : myListStructNode
    {
        // 在这里定义你的数据字段
        // 注意：为了能被框架正确识别，请为每个字段创建一个带有前缀"_"的属性
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

    // 2. 创建一个ScriptableObject来存储你的数据列表
    // [CreateAssetMenu] 属性可以让你在Unity编辑器的 "Assets/Create" 菜单中创建这个资源
    [CreateAssetMenu(fileName = "New Monster List", menuName = "Database/Monster List")]
    public class monsterListDefine : myList<monsterDictionary>
    {
    }

#if UNITY_EDITOR
    // 3. 创建一个自定义编辑器来编辑你的数据
    [CustomEditor(typeof(monsterListDefine))]
    public class monsterInspector : myListInspector<monsterDictionary>
    {
        // 重写 drawInspector 方法来定义你的编辑器界面
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
```

### 关键点解释：

*   **`monsterDictionary` 类**: 这个类继承自 `myListStructNode`，用于定义你的数据结构。你需要为你想要的每个数据字段（如 `hp`, `attack`, `defense`）添加一个公开的get属性，并且属性名以 `_` 开头。这是框架通过反射获取数据的要求。
*   **`monsterListDefine` 类**: 这个类继承自 `myList<monsterDictionary>`，它是一个 `ScriptableObject`。`[CreateAssetMenu]` 属性可以让你很方便地在 Unity 编辑器中创建这种类型的资源文件。
*   **`monsterInspector` 类**: 这个类继承自 `myListInspector<monsterDictionary>`，并被 `[CustomEditor(typeof(monsterListDefine))]` 属性标记。它用于在 Unity 的 Inspector 窗口中为你的数据列表提供一个自定义的编辑界面。你需要重写 `drawInspector` 方法，使用 `EditorGUILayout` 来定义每个数据字段的编辑控件。

## 2. 在 Unity 编辑器中创建数据资源

完成脚本编写后，回到 Unity 编辑器。

1.  在 `Project` 窗口中，右键点击 -> `Create` -> `Database` -> `Monster List`。
2.  这将在你选择的目录下创建一个新的 `Monster List` 资源文件。
3.  选中这个资源文件，你就可以在 `Inspector` 窗口中看到我们刚刚定义的自定义编辑器界面。

![图](pic/custom_data_inspector.png)  *(注意：此图片仅为示意，实际界面取决于你的代码)*

## 3. 编辑和使用数据

现在，你可以使用这个界面来添加、删除和编辑你的怪物数据了。

*   点击“添加元素”按钮来创建一个新的怪物条目。
*   在每个条目中填写怪物的 ID、名称、描述、图片以及你定义的其他属性（生命值、攻击力、防御力）。
*   这些数据被保存在你创建的 `Monster List` 资源文件中，可以在游戏运行时通过 `dataManager` 来读取和使用。

通过以上步骤，你就可以轻松地为你的游戏项目扩展任意复杂的自定义数据结构了。
