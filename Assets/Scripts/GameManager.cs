using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 游戏全局管理器
/// 负责：
/// 1. 游戏流程状态
/// 2. 全局规则数据（消除次数 / 预览权等）
/// 3. 对外事件分发
/// </summary>
public class GameManager : Singleton<GameManager>
{
    #region Game State

    private int currentLevel;
    public GameState CurrentState { get; private set; } = GameState.None;

    public event Action<GameState> OnGameStateChanged;

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
        OnGameStateChanged += GameStateHandler;
    }

    #endregion

    #region Core Game Data

    [Header("核心游戏数据")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private List<LevelConfig> levelConfigs; //所含的游戏关卡配置数据

    [SerializeField] private GameObject WinUI;
    [SerializeField] private GameObject LoseUI;
    
    #endregion

    #region Events

    /// <summary>
    /// 关卡切换时
    /// </summary>
    public event Action OnLevelChanged;
    
    #endregion

    #region Lifecycle

    protected override void Awake()
    {
        base.Awake();
        WinUI.SetActive(false);
        LoseUI.SetActive(false);
    }

    public void Start()
    {
        levelManager=LevelManager.Instance;
        StartGame();
    }
    
    #endregion

    #region Game Flow

    public void StartGame()
    {
        currentLevel = -1;

        ChangeState(GameState.Playing);

        LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        currentLevel++;

        if (currentLevel >= levelConfigs.Count)
        {
            // 所有关卡都完成
            ChangeState(GameState.Win);
            return;
        }

        LevelConfig config = levelConfigs[currentLevel];

        levelManager.Generate(config);
        OnLevelChanged?.Invoke();
    }

    //处理游戏状态
    private void GameStateHandler(GameState state)
    {
        switch (state)
        {
            case GameState.Lose:
            {
                LoseGame();
                break;
            }
            case GameState.Win:
            {
                WinGame();
                break;
            }
            case GameState.Playing:
            {
                break;
            }
            case GameState.Paused:
            {
                PauseGame();
                break;
            }
        }
    }
    public void WinGame()
    {
        if(WinUI!=null || !WinUI.activeInHierarchy)
            WinUI.SetActive(true);
    }

    public void LoseGame()
    {
        if(LoseUI!=null || !LoseUI.activeInHierarchy)
            LoseUI.SetActive(true);
    }

    public void Relife()
    {
        //关闭ui
        if(LoseUI!=null || LoseUI.activeInHierarchy)
            LoseUI.SetActive(false);
        //移除满槽
        if(SlotView.Instance!=null)
            SlotView.Instance.RemoveAllCards();
    }
    
    public void PauseGame()
    {
        
    }

    #endregion
    

}
