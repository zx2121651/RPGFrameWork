using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseData; // For monsterDictionary
using UI; // For CommandMenu

// 战斗单位的包装类，用于存储战斗中的动态数据
public class Combatant
{
    public ActorStats baseStats;
    public int currentHp;
    public int currentMp;
    public string unitName;
    public BattleHUD hud;
    public bool isPlayer;

    public Combatant(string name, ActorStats stats, bool isPlayerUnit)
    {
        unitName = name;
        baseStats = stats;
        currentHp = stats.maxHp;
        currentMp = stats.maxMp;
        isPlayer = isPlayerUnit;
    }

    public bool TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;
        hud.SetHP(currentHp);
        return currentHp <= 0; // 返回是否死亡
    }
}

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Busy, // 正在执行动作
    Won,
    Lost
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public BattleState state;

    [Header("UI引用")]
    public CommandMenu commandMenu;
    public BattleHUD playerHud; // 简化为单个玩家
    public BattleHUD enemyHud;  // 简化为单个敌人

    private Combatant playerCombatant;
    private Combatant enemyCombatant;

    private void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // For simplicity, this manager will handle one player vs one enemy for now.
    // The structure can be expanded to handle lists of combatants.

    public void StartBattle(List<monsterDictionary> enemies)
    {
        if (enemies == null || enemies.Count == 0)
        {
            Debug.LogError("Cannot start battle with no enemies.");
            return;
        }
        state = BattleState.Start;
        // For this initial implementation, we'll only fight the first enemy in the group.
        StartCoroutine(SetupBattle(enemies[0]));
    }

    private IEnumerator SetupBattle(monsterDictionary enemy)
    {
        // Assuming player data is retrieved from gameManager
        var playerInfo = ManagerSpace.gameManager.instance.SystemSetting.PlayerInfos[0];
        playerCombatant = new Combatant(playerInfo.name, playerInfo.stats, true);
        playerHud.SetupHUD(playerCombatant.unitName, playerCombatant.baseStats);

        enemyCombatant = new Combatant(enemy.name, enemy.stats, false);
        enemyHud.SetupHUD(enemyCombatant.unitName, enemyCombatant.baseStats);

        yield return new WaitForSeconds(1f);

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        commandMenu.gameObject.SetActive(true);
        commandMenu.Setup();
        commandMenu.onCommandSelected = OnCommandSelected;
    }

    private void OnCommandSelected(CommandMenu.Command command)
    {
        if (state != BattleState.PlayerTurn) return;

        StartCoroutine(PlayerAction(command));
    }

    private IEnumerator PlayerAction(CommandMenu.Command command)
    {
        state = BattleState.Busy;

        if (command == CommandMenu.Command.Attack)
        {
            int damage = playerCombatant.baseStats.attack - enemyCombatant.baseStats.defense;
            if (damage < 0) damage = 1;

            Debug.Log(playerCombatant.unitName + " 攻击 " + enemyCombatant.unitName + "，造成 " + damage + " 点伤害！");
            bool isDead = enemyCombatant.TakeDamage(damage);

            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                state = BattleState.Won;
                EndBattle();
            }
            else
            {
                state = BattleState.EnemyTurn;
                StartCoroutine(EnemyTurn());
            }
        }
        else
        {
            Debug.Log("该功能暂未实现！");
            yield return new WaitForSeconds(1f);
            state = BattleState.PlayerTurn;
            PlayerTurn();
        }
    }

    private IEnumerator EnemyTurn()
    {
        Debug.Log(enemyCombatant.unitName + " 的回合！");
        yield return new WaitForSeconds(1f);

        int damage = enemyCombatant.baseStats.attack - playerCombatant.baseStats.defense;
        if (damage < 0) damage = 1;

        Debug.Log(enemyCombatant.unitName + " 攻击 " + playerCombatant.unitName + "，造成 " + damage + " 点伤害！");
        bool isDead = playerCombatant.TakeDamage(damage);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.Lost;
            EndBattle();
        }
        else
        {
            state = BattleState.PlayerTurn;
            PlayerTurn();
        }
    }

    private void EndBattle()
    {
        if (state == BattleState.Won)
        {
            Debug.Log("战斗胜利！");
        }
        else if (state == BattleState.Lost)
        {
            Debug.Log("战斗失败...");
        }
        // TODO: 关闭战斗UI，返回到之前的场景/状态
    }
}
