using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制战斗中单个单位（玩家或敌人）的抬头显示（HUD）。
/// 负责显示名称、生命值、魔法值等信息。
/// </summary>
public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text hpText;
    public Slider hpSlider;
    public Text mpText;
    public Slider mpSlider;

    /// <summary>
    /// 设置HUD的初始状态。
    /// </summary>
    /// <param name="unitName">单位名称。</param>
    /// <param name="stats">单位的属性。</param>
    public void SetupHUD(string unitName, ActorStats stats)
    {
        nameText.text = unitName;

        if (hpSlider != null)
        {
            hpSlider.maxValue = stats.maxHp;
            hpSlider.value = stats.maxHp; // Assuming full health at start
        }
        if (hpText != null)
        {
            hpText.text = "HP: " + stats.maxHp + "/" + stats.maxHp;
        }

        if (mpSlider != null)
        {
            mpSlider.maxValue = stats.maxMp;
            mpSlider.value = stats.maxMp; // Assuming full mp at start
        }
        if (mpText != null)
        {
            mpText.text = "MP: " + stats.maxMp + "/" + stats.maxMp;
        }
    }

    /// <summary>
    /// 更新生命值显示。
    /// </summary>
    /// <param name="currentHp">当前生命值。</param>
    public void SetHP(int currentHp)
    {
        if (hpSlider != null)
        {
            hpSlider.value = currentHp;
        }
        if (hpText != null)
        {
            hpText.text = "HP: " + currentHp + "/" + hpSlider.maxValue;
        }
    }

    /// <summary>
    /// 更新魔法值显示。
    /// </summary>
    /// <param name="currentMp">当前魔法值。</param>
    public void SetMP(int currentMp)
    {
        if (mpSlider != null)
        {
            mpSlider.value = currentMp;
        }
        if (mpText != null)
        {
            mpText.text = "MP: " + currentMp + "/" + mpSlider.maxValue;
        }
    }
}
