using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors; // Make sure to include the namespace of the base class

/// <summary>
/// 一个用于2.5D游戏的特殊角色控制器。
/// 角色使用3D模型，但在2D平面（X和Y轴）上移动，Z轴被锁定。
/// 继承自 ActorMove 以便与框架的其他部分集成。
/// </summary>
public class CharacterController2_5D : ActorMove
{
    private Rigidbody _rigidbody;
    private Animator anima;

    protected float turnSmoothing = 15f;
    protected float speedDampTime = 0.1f;

    override protected void onAwake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        // 建议在Unity编辑器中为角色添加 Rigidbody 组件，并在此处获取
        if (_rigidbody == null)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            _rigidbody.useGravity = true;
        }

        anima = gameObject.GetComponent<Animator>();
        if (anima == null)
            anima = gameObject.GetComponentInChildren<Animator>();
    }

    override public void doEveryFrame()
    {
        // 每帧执行的逻辑，目前为空
    }

    /// <summary>
    /// 根据输入向量移动角色。
    /// </summary>
    /// <param name="vec">输入的2D向量，vec.x代表水平移动，vec.y在2.5D中通常不用于水平移动。</param>
    /// <param name="run">是否跑步。</param>
    /// <param name="_rush">是否冲刺（此脚本中未使用）。</param>
    public override void Move(Vector2 vec, bool run = false, bool _rush = false)
    {
        // 如果水平输入大于一个很小的值，则认为有移动输入
        if (Mathf.Abs(vec.x) > 0.1f)
        {
            // 计算移动速度
            float currentSpeed = (canRun && run ? runSpeed : speed);
            Vector3 targetVelocity = new Vector3(vec.x * currentSpeed, _rigidbody.velocity.y, 0);
            _rigidbody.velocity = targetVelocity;

            // 旋转角色以面向移动方向
            if (vec.x > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), turnSmoothing * Time.deltaTime);
            }
            else if (vec.x < 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), turnSmoothing * Time.deltaTime);
            }

            // 更新动画
            anima.SetFloat(getHat().speedF, Mathf.Abs(vec.x));
        }
        else
        {
            // 如果没有输入，则停止水平移动
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            // 更新动画到静止状态
            anima.SetFloat(getHat().speedF, 0);
        }
    }

    /// <summary>
    /// 旋转角色。在2.5D中，通常由Move方法自动处理，此方法可以留空。
    /// </summary>
    public override void Rotating(float horizontal, float vertical, bool auto = false)
    {
        // 在这个2.5D控制器中，我们不希望有独立的旋转控制。
        // 旋转由移动方向决定。
    }

    /// <summary>
    /// 使角色跳跃。
    /// </summary>
    public override void Jump()
    {
        // 检查是否在地面上（需要一个地面检测机制，此处简化）
        // if (isGrounded)
        // {
            Vector3 v = _rigidbody.velocity;
            _rigidbody.velocity = new Vector3(v.x, jumpForce, 0); // Z轴速度始终为0
        // }
    }
}
