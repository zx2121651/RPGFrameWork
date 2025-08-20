using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManagerSpace
{
    /// <summary>
    /// 管理用户输入和相关响应的控制器。
    /// 这是一个单例，负责轮询键盘输入并管理当前的游戏控制状态。
    /// </summary>
    [DisallowMultipleComponent]
    public class ControlManager : MonoBehaviour
    {
        // ControlManager 的静态单例实例
        public static ControlManager instance;

        // 存储每帧的用户输入列表
        private List<keyInput> inputs = new List<keyInput>();
        /// <summary>
        /// 获取当前帧的输入列表。其他脚本可以通过此属性来检查玩家输入。
        /// </summary>
        public List<keyInput> Inputs
        {
            get
            {
                return inputs;
            }
        }

        // 当控制状态改变时触发的事件
        public event changeStateCallbaks callbackState;

        // 记录当前的游戏操作状态 (例如：移动、窗口交互、自动事件)
        private nowState nowstate = nowState.window;
        /// <summary>
        /// 获取当前的游戏操作状态。
        /// </summary>
        public nowState NowState
        {
            get
            {
                return nowstate;
            }
        }

        // 记录当前正在交互的UI窗口的引用
        private GameObject nowWindow = null;
        /// <summary>
        /// 获取当前正在交互的UI窗口。
        /// </summary>
        public GameObject NowWindow
        {
            get
            {
                return nowWindow;
            }
        }

        // 记录当前玩家控制的角色
        private GameObject player = null;
        /// <summary>
        /// 获取当前玩家控制的角色。
        /// </summary>
        public GameObject NowPlayer
        {
            get
            {
                return player;
            }
        }

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
        /// 每帧检查玩家的键盘输入，并填充到 `inputs` 列表中。
        /// </summary>
        private void checkInput()
        {
            inputs.Clear();

            // 检测可连续触发的按键 (GetKey)
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                inputs.Add(keyInput.up);
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                inputs.Add(keyInput.down);
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                inputs.Add(keyInput.left);
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                inputs.Add(keyInput.right);
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                inputs.Add(keyInput.shift);
            }

            // 检测仅单次触发的按键 (GetKeyDown)
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                inputs.Add(keyInput.upOnce);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                inputs.Add(keyInput.downOnce);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                inputs.Add(keyInput.leftOnce);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                inputs.Add(keyInput.rightOnce);
            }
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                inputs.Add(keyInput.confirm);
            }
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape))
            {
                inputs.Add(keyInput.cancel);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                inputs.Add(keyInput.menu);
            }
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                inputs.Add(keyInput.ctrl);
            }
        }



        /// <summary>
        /// 改变当前的交互状态。
        /// </summary>
        /// <param name="state">新的状态。</param>
        /// <param name="_object">与新状态关联的游戏对象（例如，要交互的窗口或要控制的角色）。</param>
        public void changeState(nowState state, GameObject _object = null)
        {
            if (state == nowState.none)
                return;

            nowstate = state;
            if (callbackState != null)
                callbackState(nowstate);

            if (state == nowState.window)
            {
                if (_object != null)
                {
                    nowWindow = _object;
                }
                if(nowWindow!=null)
                {
                    nowWindow.SetActive(true);
                }
            }
            else if (state == nowState.move)
            {
                if (_object != null)
                    player = _object;
                if (player != null)
                {
                    CameraFollow c = Camera.main.GetComponent<CameraFollow>();
                    if (c != null)
                        c.player = player.gameObject;
                }
            }
        }

        /// <summary>
        /// 开启玩家控制角色移动的状态。
        /// </summary>
        /// <param name="_player">玩家将要控制的角色。</param>
        public void startUserControl(GameObject _player = null)
        {
            changeState(nowState.move, _player);
        }

        /// <summary>
        /// 停止玩家控制，通常用于进入事件或过场动画。
        /// </summary>
        public void stopUserControl()
        {
            changeState(nowState.auto);
        }

        /// <summary>
        /// 在主循环中每帧执行的操作。
        /// </summary>
        public void doEveryFrame()
        {
            checkInput();
        }
    }
}


