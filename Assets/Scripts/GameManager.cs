using UniRx;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour, IGameManager
{
    private PlayerController _playerController;
    private GameStateMachine _stateMachine;
    private TargetGenerator _targetGenerator;

    [Inject]
    public void InjectDependencies(GameStateMachine stateMachine,
        PlayerController playerController,
        TargetGenerator targetGenerator)
    {
        this._stateMachine = stateMachine;
        this._playerController = playerController;
        this._targetGenerator = targetGenerator;
    }
    private void Awake()
    { 
        _stateMachine.ChangeState(GameStateType.Pause);
    }
    
    private void Start()
    {
       Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0)) // Detects a tap or mouse click
            .First() // Ensures this only happens once
            .Subscribe(_ => StartGame())
            .AddTo(this);
    }
    
    private void StartGame()
    {
        _stateMachine.ChangeState(GameStateType.Play);
        
        _targetGenerator.StartLevel(); // Start target generation
        EnablePlayerControl(true); // Enable player controls if needed
    }
    
    private void Update()
    {
        _stateMachine.Update();
    }

    public void ChangeState(GameStateType newState)
    {
        _stateMachine.ChangeState(newState);
    }

    public void EnablePlayerControl(bool isEnabled)
    {
        //_playerController.enabled = isEnabled;
    }
}