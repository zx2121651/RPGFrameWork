using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UI;

namespace ManagerSpace
{
    /// <summary>
    /// UI管理器，负责管理游戏中的所有UI面板的显示、隐藏和交互逻辑。
    /// 这是一个单例类。
    /// </summary>
    [DisallowMultipleComponent]
    public class UIManager : MonoBehaviour
    {
        // UIManager 的静态单例实例
        public static UIManager instance;

        // ===================== UI 预制件引用 =====================
        [Header("UI Prefabs")]
        // “是/否”二选一面板
        [SerializeField]
        private GameObject choosePanel;
        // 用于屏幕淡入淡出的黑色面板
        [SerializeField]
        private GameObject BlackPanel;
        // 用于屏幕淡入淡出的白色面板
        [SerializeField]
        private GameObject WhitePanel;
        // 标准对话框/文字面板
        [SerializeField]
        private GameObject TextPanel;
        // 特殊文字面板（例如，没有背景的浮动文字）
        [SerializeField]
        private GameObject SpecialTextPanel;
        // 屏幕底部提示框 (例如 "获得 xxx")
        [SerializeField]
        private GameObject hint;
        // 主菜单UI
        [SerializeField]
        private GameObject menu;
        // 存档/读档UI
        [SerializeField]
        private GameObject save;
        // 用于显示图片的面板
        [SerializeField]
        private GameObject picPanel;
        // 场景中可交互对象旁的提示图标
        [SerializeField]
        private GameObject canDoHint;
        // 多选面板的预制件
        [SerializeField]
        private GameObject multiChoosePanel;

        [Header("Audio")]
        // 显示文字时的默认打字音效
        [SerializeField]
        private AudioClip show;

        //===================== 内部状态变量 =====================
        // 控制文字是否显示完毕（用于打字机效果）
        private bool showFinish = true;
        /// <summary>
        /// 文字是否已经完全显示（打字机效果结束）。
        /// </summary>
        public bool ShowFinish
        {
            get
            {
                return showFinish;
            }
        }

        // 当前对话框是否可以被隐藏
        private bool canHide = false;
        /// <summary>
        /// 当前对话框是否可以被隐藏。
        /// </summary>
        public bool CanHide
        {
            get
            {
                return canHide;
            }
        }

        // 存储当前屏幕上显示的所有图片，按名称索引
        private Dictionary<string, GameObject> images = new Dictionary<string, GameObject>();

        // 对当前活动UI面板的引用
        private GameObject blackPanel = null;
        private GameObject whitePanel = null;
        private TextPanel textPanel = null;
        private GameObject menuPanel = null;
        // 请求显示文本框的组件（用于回调验证）
        private Component textRequest = null;

        // ===================== 委托和事件 =====================
        public delegate void StopSE();
        public delegate void PlaySE(AudioClip clip, float pitch = 1.0f, bool loop = false);
        public delegate void ChangeState(nowState state, GameObject _object = null);

        // 这些委托由其他管理器（如AudioManager, ControlManager）在初始化时赋值
        public StopSE stopSE;
        public PlaySE playSE;
        public ChangeState changeState;

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

        public void init()
        {
            // 初始化方法，目前为空
        }

        /// <summary>
        /// 执行屏幕淡入效果。
        /// </summary>
        /// <param name="time">淡入持续时间。</param>
        /// <param name="black">true表示使用黑色面板，false表示使用白色面板。</param>
        public void beBlack(float time, bool black)
        {
            if (black)
            {
                if (blackPanel == null)
                {
                    GameObject go = GameObject.Instantiate(BlackPanel) as GameObject;
                    blackPanel = go;
                    go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
                    go.GetComponent<RectTransform>().localPosition = Vector2.zero;
                    go.GetComponent<BlackPanel>().beBlack(time);
                }
                else
                {
                    if (whitePanel != null)
                        blackPanel.transform.localPosition = new Vector3(blackPanel.transform.localPosition.x, blackPanel.transform.localPosition.y, whitePanel.transform.localPosition.z - 1);
                    blackPanel.GetComponent<BlackPanel>().beBlack(time);
                }
            }
            else
            {
                if (whitePanel == null)
                {
                    GameObject go = GameObject.Instantiate(WhitePanel) as GameObject;
                    whitePanel = go;
                    go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
                    go.GetComponent<RectTransform>().localPosition = Vector2.zero;
                    go.GetComponent<BlackPanel>().beBlack(time);
                }
                else
                {
                    if (blackPanel != null)
                        whitePanel.transform.localPosition = new Vector3(whitePanel.transform.localPosition.x, whitePanel.transform.localPosition.y, blackPanel.transform.localPosition.z - 1);
                    whitePanel.GetComponent<BlackPanel>().beBlack(time);
                }
            }
        }

        /// <summary>
        /// 执行屏幕淡出效果。
        /// </summary>
        /// <param name="time">淡出持续时间。</param>
        /// <param name="black">true表示使用黑色面板，false表示使用白色面板。</param>
        public void beWhite(float time, bool black)
        {
            if (black)
            {
                if (blackPanel == null)
                {
                    GameObject go = GameObject.Instantiate(BlackPanel) as GameObject;
                    blackPanel = go;
                    go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
                    go.GetComponent<RectTransform>().localPosition = Vector2.zero;
                    Color color = go.GetComponent<RawImage>().color;
                    color.a = 1;
                    go.GetComponent<RawImage>().color = color;
                    go.GetComponent<BlackPanel>().beWhite(time);
                }
                else
                {
                    blackPanel.GetComponent<BlackPanel>().beWhite(time);
                }
            }
            else
            {
                if (whitePanel == null)
                {
                    GameObject go = GameObject.Instantiate(WhitePanel) as GameObject;
                    whitePanel = go;
                    go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
                    go.GetComponent<RectTransform>().localPosition = Vector2.zero;
                    Color color = go.GetComponent<RawImage>().color;
                    color.a = 1;
                    go.GetComponent<RawImage>().color = color;
                    go.GetComponent<BlackPanel>().beWhite(time);
                }
                else
                {
                    whitePanel.GetComponent<BlackPanel>().beWhite(time);
                }
            }
        }

        /// <summary>
        /// 显示标准对话框。
        /// </summary>
        public void showTextPanel(Component _object, string name, string text, Sprite sprite, AudioClip se = null, float p = 1.0f, bool _new = false)
        {
            AudioClip s;
            if (se == null)
                s = show;
            else
                s = se;
            textRequest = _object;
            if (textPanel == null)
            {
                GameObject go = GameObject.Instantiate(TextPanel) as GameObject;
                textPanel = go.GetComponent<TextPanel>();
                go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
                go.transform.localPosition = Vector2.zero;
            }
            else if (_new)
            {
                Destroy(textPanel.gameObject);
                GameObject go = GameObject.Instantiate(TextPanel) as GameObject;
                textPanel = go.GetComponent<TextPanel>();
                go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
                go.transform.localPosition = Vector2.zero;
            }
            else
            {
                textPanel.gameObject.SetActive(true);
                textPanel.changeSize();
            }
            changeState(nowState.text, textPanel.gameObject);
            textPanel.setNameText(name);
            textPanel.setAvator(sprite);
            showFinish = false;
            if (s != null)
                playSE(s, p, true);
            StartCoroutine(showText(text, 0.05f, s, p));
        }

        /// <summary>
        /// 显示特殊对话框（例如，没有背景）。
        /// </summary>
        public void showSpecialTextPanel(Component _object, string text, Vector2 pos, AudioClip se = null, float p = 1.0f)
        {
            AudioClip s;
            if (se == null)
                s = show;
            else
                s = se;

            textRequest = _object;
            if (textPanel)
                Destroy(textPanel.gameObject);
            GameObject go = GameObject.Instantiate(SpecialTextPanel) as GameObject;
            textPanel = go.GetComponent<TextPanel>();
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.transform.localPosition = pos;
            changeState(nowState.text, textPanel.gameObject);

            showFinish = false;
            if (s != null)
                playSE(s, p, true);
            StartCoroutine(showText(text, 0.05f, s, p));
        }

        /// <summary>
        /// 隐藏当前显示的对话框。
        /// </summary>
        public void hideTextPanel()
        {
            if (textPanel != null)
            {
                if (textPanel.gameObject.tag == "specialText")
                {
                    Destroy(textPanel.gameObject);
                    textPanel = null;
                }
                else
                    textPanel.gameObject.SetActive(false);
                changeState(nowState.auto);
            }
            showFinish = false;
            canHide = false;
        }

        /// <summary>
        /// 使用打字机效果显示文本的协程。
        /// </summary>
        IEnumerator showText(string text, float showCharTime, AudioClip se, float p)
        {
            // 此处为复杂的文本解析逻辑，用于处理颜色、大小、停顿等富文本标签
            if (string.IsNullOrEmpty(text))
            {
                textPanel.setMainText("");
            }
            else
            {
                text = text.Replace("\\c[10]", "\\c[9]");

                char[] A = text.ToCharArray();
                string txt = "";
                int mark = 0;
                char type = '0';
                foreach (var a in A)
                {
                    if (!showFinish)
                    {
                        if (mark > 0)
                        {
                            switch (a)
                            {
                                case '\\': break;
                                case '|':
                                    if (se != null)
                                        stopSE();
                                    yield return new WaitForSeconds(0.5f);
                                    if (se != null)
                                        playSE(se, p, true);
                                    mark = 0; break;
                                case '.':
                                    if (se != null)
                                        stopSE();
                                    yield return new WaitForSeconds(0.25f);
                                    if (se != null)
                                        playSE(se, p, true);
                                    mark = 0; break;
                                case 'c': mark = 2; type = 'c'; break;
                                case '{':
                                    mark = 0;
                                    if (type == 's')
                                        txt += "</size>";
                                    else
                                        type = 's';
                                    txt += "<size=" + (int)(textPanel.mainText.fontSize * 1.3f) + ">"; break;
                                case '}':
                                    mark = 0;
                                    if (type == 's')
                                        txt += "</size>";
                                    else
                                        type = 's';
                                    txt += "<size=" + (int)(textPanel.mainText.fontSize * 0.8f) + ">"; break;
                                case '[': mark--; break;
                                case ']': mark--; break;
                                case '1': if (type == 'c') txt += "<color=#67c2ea>"; break;
                                case '2': if (type == 'c') txt += "<color=#f78f6d>"; break;
                                case '3': if (type == 'c') txt += "<color=#7f9782>"; break;
                                case '4': if (type == 'c') txt += "<color=#99ccff>"; break;
                                case '6': if (type == 'c') txt += "<color=#ffffa0>"; break;
                                case '9': if (type == 'c') txt += "<color=#f45331>"; break;
                                case '0': if (type == 'c') { type = '0'; txt += "</color>"; } break;
                                default: mark = 0; break;
                            }
                        }
                        else
                        {
                            if (a == '\\')
                            {
                                mark = 1;
                            }
                            else
                            {
                                txt += a;
                                if (type == 'c')
                                {
                                    textPanel.setMainText(txt + "</color>");
                                }
                                else if (type == 's')
                                {
                                    textPanel.setMainText(txt + "</size>");
                                }
                                else
                                {
                                    textPanel.setMainText(txt);
                                }
                                yield return new WaitForSeconds(showCharTime);
                            }
                        }
                    }
                    else
                    {
                        text = text.Replace("\\|", "");
                        text = text.Replace("\\^", "");
                        text = text.Replace("\\.", "");
                        text = text.Replace("\\!", "");
                        text = text.Replace("\\c[0]", "</color>");
                        text = text.Replace("\\c[1]", "<color=#67c2ea>");
                        text = text.Replace("\\c[2]", "<color=#f78f6d>");
                        text = text.Replace("\\c[3]", "<color=#7f9782>");
                        text = text.Replace("\\c[4]", "<color=#99ccff>");
                        text = text.Replace("\\c[6]", "<color=#ffffa0>");
                        text = text.Replace("\\c[9]", "<color=#f45331>");

                        int num = Regex.Split(text, "\\{|\\}").Length - 1;
                        if (text.Contains("\\{"))
                        {
                            text = text.Replace("\\{", "<size=" + (int)(textPanel.mainText.fontSize * 1.3f) + ">");
                        }
                        if (text.Contains("\\}"))
                        {
                            text = text.Replace("\\}", "<size=" + (int)(textPanel.mainText.fontSize * 0.8f) + ">");
                        }
                        for (int i = 0; i < num; i++)
                            text += "</size>";

                        textPanel.setMainText(text);
                        break;
                    }
                }
                if (se != null)
                    stopSE();
            }

            showFinish = true;
            canHide = true;
        }

        /// <summary>
        /// 在屏幕底部显示一个短暂的提示信息。
        /// </summary>
        public void showHint(string text, int windowSize, int oriSize)
        {
            GameObject go = GameObject.Instantiate(hint) as GameObject;
            go.GetComponentInChildren<Text>().text = text;
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.transform.localPosition = new Vector2(0, -50 / oriSize * windowSize);
            Destroy(go, 3);
        }

        /// <summary>
        /// 跳过打字机效果，直接显示全部文字。
        /// </summary>
        public void skip()
        {
            showFinish = true;
            stopSE();
        }

        /// <summary>
        /// 检查请求显示文本框的组件是否为指定的组件。
        /// </summary>
        public bool checkTextRequet(Component _object)
        {
            return (_object == textRequest);
        }

        /// <summary>
        /// 显示主菜单。
        /// </summary>
        public void showMenuPanel()
        {
            GameObject go = GameObject.Instantiate(menu) as GameObject;
            menuPanel = go;
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.transform.localPosition = Vector2.zero;
            changeState(nowState.window, menuPanel.gameObject);
        }

        /// <summary>
        /// 显示存档/读档界面。
        /// </summary>
        public void showSavePanel()
        {
            GameObject go = GameObject.Instantiate(save) as GameObject;
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.transform.localPosition = Vector2.zero;
            changeState(nowState.window, go);
        }

        /// <summary>
        /// 显示一个任意的UI面板。
        /// </summary>
        public GameObject showAnyPanel(GameObject _panel, Vector2 _pos, bool mainUI, nowState nowstate)
        {
            if (nowstate != nowState.window || mainUI)
            {
                GameObject go = GameObject.Instantiate(_panel) as GameObject;
                changeState(nowState.window, go);
                go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
                basePanel basepanel = _panel.GetComponent<basePanel>();
                if (basepanel != null)
                {
                    basepanel.changeStartPos(_pos.x, _pos.y);
                }
                go.transform.localPosition = _pos;
                return go;
            }
            else
                return null;
        }

        /// <summary>
        /// 在指定世界坐标处显示一个可交互提示图标。
        /// </summary>
        public GameObject showCanDoHint(Vector3 _pos)
        {
            GameObject go = GameObject.Instantiate(canDoHint) as GameObject;
            var canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
            go.transform.SetParent(canvas);

            CanDoHint basepanel = go.GetComponent<CanDoHint>();
            if (basepanel != null)
            {
                basepanel.Pos = _pos;
            }

            return go;
        }

        /// <summary>
        /// 显示一个全屏的纯色块，通常用于闪屏效果。
        /// </summary>
        public GameObject showFlash(Color c, float time)
        {
            GameObject go = GameObject.Instantiate(picPanel) as GameObject;
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            go.transform.localPosition = Vector2.zero;
            PicPanel panel = go.GetComponent<PicPanel>();
            panel.setColor(c);
            panel.movePic(go.transform.localPosition, 0, time, true);
            return go;
        }

        /// <summary>
        /// 在屏幕上显示一张图片。
        /// </summary>
        public GameObject showPicPanel(string index, Vector2 _pos, Sprite sp, float a)
        {
            GameObject go = GameObject.Instantiate(picPanel) as GameObject;
            go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
            PicPanel panel = go.GetComponent<PicPanel>();
            panel.setPic(sp);
            panel.setSize(sp.texture.width, sp.texture.height);
            panel.setAlpha(a);
            panel.changeStartPos(_pos.x, _pos.y);
            go.transform.localPosition = _pos;
            images.Add(index, go);
            return go;
        }

        /// <summary>
        /// 移动并/或改变已显示图片的透明度。
        /// </summary>
        public void movePicPanel(string index, Vector2 pos, float a, float time)
        {
            images[index].GetComponent<PicPanel>().movePic(pos, a, time);
        }

        /// <summary>
        /// 隐藏并销毁指定的图片。
        /// </summary>
        public void hidePicPanel(string index)
        {
            GameObject g = images[index];
            images.Remove(index);
            Destroy(g);
        }

        /// <summary>
        /// 显示一个“是/否”二选一的确认框。
        /// </summary>
        public void showChoosePanel(string text, Func yes, Func no, nowState nowstate, GameObject mainPanel = null, bool black = false)
        {
            GameObject g = showAnyPanel(choosePanel, Vector2.zero, true, nowstate) as GameObject;
            var c = g.GetComponent<choosePanel>();
            c.ChooseText = text;
            c.yesFunc = yes;
            c.noFunc = no;
            c.setMenuPanel(mainPanel);
            if (black)
                c.BlackBackground();
        }

        /// <summary>
        /// 获取当前主菜单的引用。
        /// </summary>
        public GameObject getMainMenu()
        {
            if (menuPanel != null && menuPanel.activeInHierarchy)
            {
                return menuPanel;
            }
            else
                return null;
        }

        /// <summary>
        /// 设置主菜单的引用。
        /// </summary>
        public void setMainMenu(GameObject menu)
        {
            menuPanel = menu;
        }

        /// <summary>
        /// 实例化并显示多项选择UI面板。
        /// </summary>
        /// <param name="choices">要显示的选项文本列表。</param>
        /// <param name="callback">当玩家做出选择后要执行的回调，参数为所选项的索引。</param>
        public void showMultiChoosePanel(List<string> choices, System.Action<int> callback)
        {
            if (multiChoosePanel != null)
            {
                // 实例化预制件
                GameObject go = Instantiate(multiChoosePanel) as GameObject;
                // 设置其父节点为Canvas
                go.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
                go.transform.localPosition = Vector2.zero;

                // 获取面板脚本组件
                multiChoosePanel panel = go.GetComponent<multiChoosePanel>();
                if (panel != null)
                {
                    // 调用面板的设置方法
                    panel.showChoices(choices, callback);
                }
                else
                {
                    Debug.LogError("multiChoosePanel prefab does not have the multiChoosePanel script component.");
                    Destroy(go);
                }
            }
            else
            {
                Debug.LogError("multiChoosePanel prefab is not assigned in the UIManager.");
            }
        }
    }
}
