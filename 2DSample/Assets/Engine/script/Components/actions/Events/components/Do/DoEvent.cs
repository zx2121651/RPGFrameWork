using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public class DoEvent : DoBasicEvents
    {
        protected class eventStruct
        {
            public eventType type;
            public calValue howToCal;
            public turn turn;
            public findType findType;
            public questStatus questStatus;
            public IndependentSwitch independentSwitch;
            public conditionType conditionType;
            public compare compare;

            //设置名字和文本
            public string str1;
            public string str2;
            public string str3;
            public string str4;
            public bool bool1 = false;
            public bool bool2 = false;
            public float float1;
            public float float2;
            public int int1;
            public Vector3 vec1;
            public Vector3 vec2;
            public Color color1;

            public eventStruct(TreeNodeInterface toDo)
            {
                type = toDo.getData<eventType>(structProperty.type);
                float1 = toDo.getData<float>(structProperty.float1);
                float2 = toDo.getData<float>(structProperty.float2);
                str1 = toDo.getData<string>(structProperty.str1);
                str2 = toDo.getData<string>(structProperty.str2);
                str3 = toDo.getData<string>(structProperty.str3);
                str4 = toDo.getData<string>(structProperty.str4);
                int1 = toDo.getData<int>(structProperty.int1);
                bool1 = toDo.getData<bool>(structProperty.bool1);
                bool2 = toDo.getData<bool>(structProperty.bool2);
                vec1 = toDo.getData<Vector3>(structProperty.vec1);
                vec2 = toDo.getData<Vector3>(structProperty.vec2);
                howToCal = toDo.getData<calValue>(structProperty.howToCal);
                turn = toDo.getData<turn>(structProperty.turn);
                findType = toDo.getData<findType>(structProperty.findType);
                color1 = toDo.getData<Color>(structProperty.color1);
                questStatus = toDo.getData<questStatus>(structProperty.questStatus);
                independentSwitch = toDo.getData<IndependentSwitch>(structProperty.independentSwitch);
                compare = toDo.getData<compare>(structProperty.compare);
                conditionType = toDo.getData<conditionType>(structProperty.conditionType);
            }
        }

        /// <summary>
        /// 执行单个事件节点的具体逻辑。
        /// 这是事件系统的核心解释器。
        /// </summary>
        /// <param name="toDo">要执行的事件树节点。</param>
        override public void doSth(TreeNodeInterface toDo)
        {
            // 将事件节点中的数据解包到一个 eventStruct 中，方便访问
            eventStruct e = new eventStruct(toDo);

            // 根据事件类型执行不同的操作
            switch (e.type)
            {
                // =============================== 流程控制 ===============================
                // 空操作，直接进入下一个事件
                case eventType.none: pushNow(true); break;
                // If 条件事件，其逻辑在检查子节点条件时处理，此处仅作为流程节点
                case eventType.If: pushNow(true); break;
                // 显示多项选择
                case eventType.ShowChoices:
                    // 获取当前事件节点的所有子节点，每个子节点代表一个选项
                    var children = toDo.getChildren();
                    if (children != null && children.Count > 0)
                    {
                        // 创建一个列表来存储选项的显示文本
                        List<string> choices = new List<string>();
                        foreach (var child in children)
                        {
                            // 从每个子节点的 str1 属性中获取选项文本
                            choices.Add(child.getData<string>(structProperty.str1));
                        }

                        // 调用UI管理器显示选项面板，并传入一个回调函数
                        a.showMultiChoosePanel(choices, (selectedIndex) => {
                            // 当玩家做出选择后，此回调函数被执行
                            if (selectedIndex >= 0 && selectedIndex < children.Count)
                            {
                                // 如果选择有效，则将对应的子节点作为下一个事件推入堆栈，实现分支
                                pushNow(true, children[selectedIndex]);
                            }
                            else
                            {
                                // 如果选择无效或被取消，则不进行分支，直接继续执行后续事件
                                pushNow(true);
                            }
                        });
                    }
                    else
                    {
                        // 如果ShowChoices节点下没有定义子节点（即没有选项），则直接跳过
                        pushNow(true);
                    }
                    break;

                // =============================== 场景与玩家控制 ===============================
                // 开始玩家控制
                case eventType.startUserControl: pushNow(true); startUserControl(); break;
                // 停止玩家控制
                case eventType.stopUserControl: pushNow(true); a.stopUserControl(); break;
                // 切换场景
                case eventType.changeScene:
                    pushNow(true);
                    changeSceneDo csd = new changeSceneDo(a.getTeam()[0], e.vec1, e.turn);
                    a.loadLevel(e.str1, csd);
                    break;
                // 移动镜头
                case eventType.moveCamera:
                    CameraMoveToSw(e.vec1, e.float1);
                    break;

                // =============================== 变量与数据 ===============================
                // 修改全局整数
                case eventType.changeGlobalInt:
                    calValue cal = e.howToCal;
                    int u = a.getInt(e.str1);
                    if (cal == calValue.add) a.setInt(e.str1, u + e.int1);
                    else if (cal == calValue.minus) a.setInt(e.str1, u - e.int1);
                    else if (cal == calValue.multiply) a.setInt(e.str1, u * e.int1);
                    else if (cal == calValue.divide) a.setInt(e.str1, u / e.int1);
                    else a.setInt(e.str1, e.int1);
                    pushNow(true); break;
                // 修改全局浮点数
                case eventType.changeGlobalDouble:
                    cal = e.howToCal;
                    double y = a.getDouble(e.str1);
                    if (cal == calValue.add) a.setDouble(e.str1, y + e.float1);
                    else if (cal == calValue.minus) a.setDouble(e.str1, y - e.float1);
                    else if (cal == calValue.multiply) a.setDouble(e.str1, y * e.float1);
                    else if (cal == calValue.divide) a.setDouble(e.str1, y / e.float1);
                    else a.setDouble(e.str1, e.float1);
                    pushNow(true); break;
                // 修改全局三维向量
                case eventType.changeGlobalVector3:
                    cal = e.howToCal;
                    Vector3 v = a.getVec3(e.str1);
                    if (cal == calValue.add) a.setVec3(e.str1, v + e.vec1);
                    else if (cal == calValue.minus) a.setVec3(e.str1, v - e.vec1);
                    else if (cal == calValue.multiply) a.setVec3(e.str1, v * e.vec1.x);
                    else if (cal == calValue.divide) a.setVec3(e.str1, v / e.vec1.x);
                    else a.setVec3(e.str1, e.vec1);
                    pushNow(true); break;
                // 修改全局开关
                case eventType.changeGlobalSwith:
                    a.setSwitch(e.str1, e.bool1);
                    pushNow(true); break;
                // 改变道具数量
                case eventType.changeItem:
                    a.getItem(e.str1, e.int1);
                    var n = a.getItemInfo(e.str1).getData<string>(itemProperty.name);
                    int num = Mathf.Abs(e.int1);
                    if(num>0)
                        showHint(a.findText(n, getSetting().nowlang) + (e.int1>0?" +": " -") + num);
                    break;
                // 开始一个任务
                case eventType.startQuest:
                    a.addQuest(e.str1);
                    pushNow(true);
                    break;
                // 改变任务状态
                case eventType.changeQuest:
                    a.changeQuest(e.str1, e.questStatus);
                    pushNow(true);
                    break;
                // 改变队伍成员
                case eventType.changeTeam:
                    n = a.getSystemSetting().PlayerInfos[e.int1].name;
                    if (e.bool1)
                    {
                        inTeam(e.int1);
                        showHint(a.findText(n, getSetting().nowlang) + a.findText("sys.inTeam", getSetting().nowlang));
                    }
                    else
                    {
                        outTeam(e.int1);
                        showHint(a.findText(n, getSetting().nowlang) + a.findText("sys.outTeam", getSetting().nowlang));
                    }
                    break;

                // =============================== 视觉效果 ===============================
                // 淡入
                case eventType.beBlack: beBlack(e.float1); break;
                // 淡出
                case eventType.beWhite: beWhite(e.float1); break;
                // 等待
                case eventType.wait: waitForTime(e.float1); break;
                 // 显示图片
                case eventType.showPic:
                    a.showPicPanel(e.str1, e.vec1, a.findImg(e.str2), e.float1 / 255.0f);
                    pushNow(true); break;
                // 移动图片
                case eventType.movePic:
                    a.movePicPanel(e.str1, e.vec1, e.float1 / 255.0f, e.float2);
                    if (e.bool1) waitForTime(e.float2);
                    else pushNow(true);
                    break;
                // 隐藏图片
                case eventType.hidePic:
                    pushNow(true);
                    a.hidePicPanel(e.str1); break;
                // 显示特殊文本
                case eventType.textSpecial:
                    string text = a.findText(e.str1, getSetting().nowlang);
                    showSpecialTextPanel(a, text, e.vec1, a.findAudio(e.str2), a.findPitch(e.str2));
                    break;
                // 播放动画
                case eventType.playAnima:
                    var prefab = spawnPrefab(e.str1, e.vec1);
                    var animator = prefab.GetComponent<Animator>();
                    AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];
                    var speed = animator.GetCurrentAnimatorStateInfo(0).speed;
                    var time = clip.length / (speed * speed);
                    if (e.bool1) waitForTime(time);
                    else pushNow(true);
                    break;
                // 屏幕闪烁
                case eventType.flashScreen:
                    a.showFlash(e.color1, e.float1);
                    if (e.bool1) waitForTime(e.float1);
                    else pushNow(true);
                    break;
                // 屏幕震动
                case eventType.shakeScreen:
                    ShakeCamera(e.float2, e.float1);
                    if (e.bool1) waitForTime(e.float1);
                    else pushNow(true);
                    break;

                // =============================== 音频 ===============================
                // 播放BGM
                case eventType.playBgm:
                    a.playMusic(a.findAudio(e.str1));
                    pushNow(true); break;
                // 停止BGM
                case eventType.stopBgm:
                    a.stopMusic();
                    pushNow(true); break;
                // 播放BGS (背景音效)
                case eventType.playBgs:
                    a.playBGS(a.findAudio(e.str1));
                    pushNow(true); break;
                // 停止BGS
                case eventType.stopBgs:
                    a.stopBGS();
                    pushNow(true); break;
                // 播放SE (音效)
                case eventType.playSE:
                    a.playSE(a.findAudio(e.str1), a.findPitch(e.str1));
                    pushNow(true); break;

                // =============================== 场景物体操作 ===============================
                // 移动场景物体
                case eventType.sbMoveToSw:
                    GameObject g = null;
                    if (e.findType == findType.tag) g = GameObject.FindGameObjectWithTag(e.str1);
                    else if (e.findType == findType.name) g = GameObject.Find(e.str1);
                    if (g.GetComponent<ActorInterface>() == null) g = g.transform.GetChild(0).gameObject;
                    if (g == null) Debug.LogError("未找到物体");
                    else { SbMoveToSw(g, e.vec1, e.turn); }
                    break;
                // 改变物体位置
                case eventType.changeThingPos:
                    pushNow(true); g = null;
                    if (e.findType == findType.tag) g = GameObject.FindGameObjectWithTag(e.str1);
                    else if (e.findType == findType.name) g = GameObject.Find(e.str1);
                    if (g == null) Debug.LogError("未找到物体");
                    else { changeThingPos(g, e.vec1); }
                    break;
                // 改变物体朝向
                case eventType.changeTurn:
                    g = null;
                    if (e.findType == findType.tag) g = GameObject.FindGameObjectWithTag(e.str1);
                    else if (e.findType == findType.name) g = GameObject.Find(e.str1);
                    if (g == null) Debug.LogError("未找到物体");
                    else
                    {
                        if (g.GetComponent<ActorInterface>() == null) g = g.transform.GetChild(0).gameObject;
                        setActorTurn(g, e.turn);
                    }
                    break;
                // 改变物体激活状态 (显/隐)
                case eventType.changeThingActive:
                    pushNow(true); g = null; // 注意find函数无法查找隐藏的物体
                    if (e.findType == findType.tag)
                    {
                        if (e.str1 == HashsAndTags.player) g = a.Player.gameObject;
                        else g = GameObject.FindGameObjectWithTag(e.str1).transform.GetChild(0).gameObject;
                    }
                    else if (e.findType == findType.name) g = GameObject.Find(e.str1).transform.GetChild(0).gameObject;
                    if (g == null) Debug.LogError("未找到物体");
                    else g.SetActive(e.bool1);
                    break;
                // 设置跟随者
                case eventType.setFollow:
                    g = null;
                    if (e.findType == findType.tag) g = GameObject.FindGameObjectWithTag(e.str1);
                    else if (e.findType == findType.name) g = GameObject.Find(e.str1);
                    if(g==null) Debug.LogError("未找到物体");
                    else
                    {
                        GameObject g2 = Instantiate(getSystemSetting().PlayerInfos[e.int1].playerPrefab);
                        g2.transform.position = g.transform.position;
                        setFollow(g2, e.bool1);
                    }                  
                    pushNow(true);
                    break;

                // =============================== 系统与其他 ===============================
                // 在控制台打印日志 (调试用)
                case eventType.debugLog:
                    Debug.Log(a.findText(e.str1,language.none));
                    pushNow(true); break;
                // 显示存档界面
                case eventType.showSavePanel:
                    pushNow();
                    a.showSavePanel();
                    break;
                // 改变天气 (暂未实现)
                case eventType.changeWeather:
                    pushNow(true);
                    break;

                case eventType.StartBattle:
                    // 从Resources文件夹加载指定的敌人队伍资源
                    EnemyGroup enemyGroup = Resources.Load<EnemyGroup>("EnemyGroups/" + e.str1);
                    if (enemyGroup != null)
                    {
                        // 检查BattleManager实例是否存在
                        if (BattleManager.instance != null)
                        {
                            BattleManager.instance.StartBattle(enemyGroup.enemies);
                            // 通常战斗开始后，事件流会暂停，由BattleManager接管
                            // 此处不调用 pushNow()，等待战斗结束后再决定后续事件
                        }
                        else
                        {
                            Debug.LogError("BattleManager instance not found!");
                            pushNow(true); // 发生错误，继续执行后续事件以避免卡死
                        }
                    }
                    else
                    {
                        Debug.LogError("EnemyGroup asset not found in Resources: " + e.str1);
                        pushNow(true);
                    }
                    break;

               // default 会造成混乱，严禁出现
            }
        }
    }
}
