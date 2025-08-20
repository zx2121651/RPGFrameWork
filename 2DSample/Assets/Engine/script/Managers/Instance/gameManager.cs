using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManagerSpace
{
    /// <summary>
    /// 存储游戏开始时的默认设置。
    /// </summary>
    [System.Serializable]
    public class startSetting
    {
        public Vector3 StartPos; // 玩家初始位置
        public turn StartTurn; // 玩家初始朝向
        public string StartScene; // 初始场景名称
        public List<int> startActor = new List<int>(); // 初始队伍中的角色ID列表
    }

    /// <summary>
    /// 用于管理正在运行的公共事件的数据结构。
    /// </summary>
    [System.Serializable]
    public class publicEventStruct
    {
        public Component com; // 触发此公共事件的组件
        public commonEventCallback callback; // 事件结束后的回调
        public publicEventInterface events; // 公共事件本身的接口引用

        public publicEventStruct(publicEventInterface events, commonEventCallback callback = null,Component cm = null)
        {
            this.events = events;
            this.callback = callback;
            this.com = cm;
        }
    }

    /// <summary>
    /// 游戏主管理器，负责管理游戏全局状态、设置、公共事件和队伍信息。
    /// 这是一个单例类。
    /// </summary>
    [DisallowMultipleComponent]
    public class gameManager : MonoBehaviour
    {
        // gameManager 的静态单例实例
        public static gameManager instance;

        // 当前正在运行的公共事件列表
        private List<publicEventStruct> commonEvents = new List<publicEventStruct>();

        // 公共事件的预制件，用于实例化
        [SerializeField]
        private GameObject publicEventPrefab;

        // 独立开关的字典，用于场景或事件的特定逻辑
        private Dictionary<string, bool[]> independentSwitches = new Dictionary<string, bool[]>();
        /// <summary>
        /// 获取独立开关字典的引用。
        /// </summary>
        public Dictionary<string, bool[]> IndependentSwitches
        {
            get
            {
                return independentSwitches;
            }
        }

        // 当前玩家队伍
        private List<int> team = new List<int>();
        /// <summary>
        /// 获取当前玩家队伍的引用。
        /// </summary>
        public List<int> Team
        {
            get
            {
                return team;
            }
        }

        // 系统的固定设置（不可在游戏中更改）
        public systemSetting SystemSetting = new systemSetting();
        // 游戏开始时的默认设置
        public startSetting StartSetting = new startSetting();

        // 玩家的可配置设置（例如音量、语言等）
        private settings setting = new settings();
        /// <summary>
        /// 获取玩家设置的引用。
        /// </summary>
        public settings Setting
        {
            get
            {
                return setting;
            }
        }

        // 游戏窗口的原始尺寸倍数
        private int oriSize;
        /// <summary>
        /// 获取游戏窗口的原始尺寸倍数。
        /// </summary>
        public int OriSize
        {
            get
            {
                return oriSize;
            }
        }

        // 记录游戏总时长
        private double playTime = 0.0;
        /// <summary>
        /// 获取或设置游戏总时长。
        /// </summary>
        public double PlayTime
        {
            get
            {
                return playTime;
            }
            set
            {
                playTime = value;
            }
        }

        // 当设置发生变化时触发的事件
        public event changeSettingCallbaks callbacks;


        private void Awake()
        {
            // 实现单例模式
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance.gameObject);
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// 初始化管理器。
        /// </summary>
        public void init()
        {
            Application.targetFrameRate = SystemSetting.FPS;
            loadSetting();
            oriSize = SystemSetting.startSize;
            Screen.SetResolution(SystemSetting.width * setting.windowSize, SystemSetting.height * setting.windowSize, false);
        }

        /// <summary>
        /// 在主循环中每帧执行的操作。
        /// </summary>
        public void doEveryFrame()
        {
            playTime += Time.deltaTime;
            checkCommonEvent();
        }

        /// <summary>
        /// 开始一个公共事件。
        /// </summary>
        /// <param name="name">公共事件的名称。</param>
        /// <param name="ns">当前的游戏状态。</param>
        /// <param name="c">事件结束后的回调。</param>
        /// <param name="cm">触发事件的组件。</param>
        public void startPublicEvent(string name, nowState ns, commonEventCallback c = null, Component cm = null)
        {
            if(cm!=null)
            {
                for(int i=0;i<commonEvents.Count;i++)
                {
                    if (commonEvents[i].com == cm)
                        return;
                }
            }

            GameObject g = Instantiate(publicEventPrefab);
            var e = g.GetComponent<publicEventInterface>();
            e.startEvent(name,ns);
            commonEvents.Add(new publicEventStruct(e,c,cm));
        }

        /// <summary>
        /// 每帧检查正在运行的公共事件的状态。
        /// </summary>
        private void checkCommonEvent()
        {
            for (int i = 0; i < commonEvents.Count; i++)
            {
                if (commonEvents[i].events.canNowListRemove())
                {
                    commonEvents[i].events.resetNowList();
                    commonEvents[i].events.exit();
                    if (commonEvents[i].callback != null)
                        commonEvents[i].callback();
                    commonEvents.RemoveAt(i);
                    i--;
                }
                else
                {
                    commonEvents[i].events.setCanDo(commonEvents[i].events.checkNowListEventConditions());
                }
            }
        }

        /// <summary>
        /// 从 PlayerPrefs 加载玩家设置。
        /// </summary>
        public void loadSetting()
        {
            //窗口尺寸
            if (PlayerPrefs.HasKey(settings.keys[0]))
                setting.alwaysRun = (PlayerPrefs.GetInt(settings.keys[0]) == 0 ? true : false);
            if (PlayerPrefs.HasKey(settings.keys[1]))
                setting.autoMessage = (PlayerPrefs.GetInt(settings.keys[1]) == 0 ? true : false);
            if (PlayerPrefs.HasKey(settings.keys[2]))
                setting.windowSize = PlayerPrefs.GetInt(settings.keys[2]);
            else
                setting.windowSize = SystemSetting.startSize;
            if (PlayerPrefs.HasKey(settings.keys[3]))
                setting.musicValue = PlayerPrefs.GetFloat(settings.keys[3]);
            if (PlayerPrefs.HasKey(settings.keys[4]))
                setting.SEValue = PlayerPrefs.GetFloat(settings.keys[4]);
            if (PlayerPrefs.HasKey(settings.keys[5]))
                setting.nowlang = (language)PlayerPrefs.GetInt(settings.keys[5]);

            if(callbacks!=null)
                callbacks(setting);
        }

        /// <summary>
        /// 将当前玩家设置保存到 PlayerPrefs。
        /// </summary>
        public void saveSetting()
        {
            PlayerPrefs.SetInt(settings.keys[0], setting.alwaysRun ? 0 : 1);
            PlayerPrefs.SetInt(settings.keys[1], setting.autoMessage ? 0 : 1);
            PlayerPrefs.SetInt(settings.keys[2], setting.windowSize);
            PlayerPrefs.SetFloat(settings.keys[3], setting.musicValue);
            PlayerPrefs.SetFloat(settings.keys[4], setting.SEValue);
            PlayerPrefs.SetInt(settings.keys[5], (int)setting.nowlang);

            if (callbacks != null)
                callbacks(setting);
        }

        /// <summary>
        /// 切换游戏语言。
        /// </summary>
        /// <param name="add">true为下一个语言，false为上一个语言。</param>
        public void changeLanguage(bool add)
        {
            int j = (int)setting.nowlang;
            int length = System.Enum.GetValues(typeof(language)).Length;
            if (add)
                setting.nowlang = (language)((j + 1) % length);
            else
                setting.nowlang = (language)((j - 1 + length) % length);
        }

        /// <summary>
        /// 切换指定的布尔型设置（例如：总是跑步）。
        /// </summary>
        /// <param name="key">设置的键名。</param>
        public void changeBool(string key)
        {
            if (key == settings.keys[0])
            {
                setting.alwaysRun = !setting.alwaysRun;
            }
            else if (key == settings.keys[1])
            {
                setting.autoMessage = !setting.autoMessage;
            }
        }

        /// <summary>
        /// 改变游戏窗口尺寸。
        /// </summary>
        /// <param name="add">true为放大，false为缩小。</param>
        public void addWindowSize(bool add)
        {
            if (add)
                setting.windowSize = setting.windowSize % SystemSetting.maxSize + 1;
            else
                setting.windowSize = (setting.windowSize - 1) > 0 ? setting.windowSize - 1 : SystemSetting.maxSize;
            if (setting.windowSize == SystemSetting.maxSize)
                Screen.SetResolution(Screen.width, Screen.height, true);
            else
                Screen.SetResolution(SystemSetting.width * setting.windowSize, SystemSetting.height * setting.windowSize, false);
        }

        /// <summary>
        /// 从存档加载独立开关的状态。
        /// </summary>
        public void loadIndependentSwitchs(Dictionary<string, bool[]> a)
        {
            independentSwitches = a;
        }

        /// <summary>
        /// 从存档加载队伍信息。
        /// </summary>
        public void loadTeam(List<int> team)
        {
            this.team = team;
        }

        /// <summary>
        /// 获取指定键的独立开关状态。
        /// </summary>
        public bool[] getIndependentSwitchs(string key)
        {
            if (independentSwitches.ContainsKey(key))
            {
                return independentSwitches[key];
            }
            else
            {
                return new bool[] { false, false, false, false };
            }
        }

        /// <summary>
        /// 设置指定键的独立开关状态。
        /// </summary>
        public void setIndependentSwitchs(string key, int index, bool ind)
        {
            if (!independentSwitches.ContainsKey(key))
            {
                independentSwitches.Add(key, new bool[] { false, false, false, false });
            }
            independentSwitches[key][index] = ind;
        }

        /// <summary>
        /// 角色入队（如果尚未在队中）。
        /// </summary>
        public void inTeamIfNotContain(int id)
        {
            if (!team.Contains(id))
                team.Add(id);
        }

        /// <summary>
        /// 角色离队（如果在队中）。
        /// </summary>
        public void outTeamIfContain(int id)
        {
            if (team.Contains(id))
                team.Remove(id);
        }
    }
}

