using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制战斗中目标选择的逻辑。
/// </summary>
public class TargetSelection : MonoBehaviour
{
    // 回调函数，当目标被选定时调用，参数为目标的索引
    public System.Action<int> onTargetSelected;

    private List<GameObject> targets; // 所有可选目标
    private int currentTargetIndex = 0;
    private GameObject cursor; // 用于指示当前目标的光标

    /// <summary>
    /// 激活目标选择模式。
    /// </summary>
    /// <param name="availableTargets">可供选择的目标列表。</param>
    /// <param name="selectionCursor">用于高亮显示的光标对象。</param>
    public void BeginSelection(List<GameObject> availableTargets, GameObject selectionCursor)
    {
        this.targets = availableTargets;
        this.cursor = selectionCursor;
        this.currentTargetIndex = 0;

        this.gameObject.SetActive(true);
        UpdateCursorPosition();
    }

    private void Update()
    {
        // 使用ControlManager获取输入
        List<keyInput> inputs = ManagerSpace.ControlManager.instance.Inputs;

        if (inputs.Contains(keyInput.rightOnce) || inputs.Contains(keyInput.downOnce))
        {
            currentTargetIndex = (currentTargetIndex + 1) % targets.Count;
            UpdateCursorPosition();
        }
        else if (inputs.Contains(keyInput.leftOnce) || inputs.Contains(keyInput.upOnce))
        {
            currentTargetIndex = (currentTargetIndex - 1 + targets.Count) % targets.Count;
            UpdateCursorPosition();
        }
        else if (inputs.Contains(keyInput.confirm))
        {
            SelectTarget();
        }
        else if (inputs.Contains(keyInput.cancel))
        {
            CancelSelection();
        }
    }

    private void UpdateCursorPosition()
    {
        if (cursor != null && targets.Count > 0)
        {
            cursor.SetActive(true);
            // 将光标放置在目标头顶（或脚下）
            cursor.transform.position = targets[currentTargetIndex].transform.position + Vector3.up * 2;
        }
    }

    private void SelectTarget()
    {
        if (onTargetSelected != null)
        {
            onTargetSelected(currentTargetIndex);
        }
        EndSelection();
    }

    private void CancelSelection()
    {
        // 传递一个-1表示取消选择
        if (onTargetSelected != null)
        {
            onTargetSelected(-1);
        }
        EndSelection();
    }

    private void EndSelection()
    {
        if (cursor != null)
        {
            cursor.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }
}
