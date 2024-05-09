using UniRx;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour, IGameManager
{
    private GameStateMachine _stateMachine;
    private TargetGenerator _targetGenerator;
    private BallController _ballController;

    [Inject]
    public void InjectDependencies(GameStateMachine stateMachine,
        TargetGenerator targetGenerator, 
        BallController ballController)
    {
        this._stateMachine = stateMachine;
        this._targetGenerator = targetGenerator;
        this._ballController = ballController;
    }
    private void Awake()
    { 
        _stateMachine.ChangeState(GameStateType.Pause);
    }
    
    public void StartGame()
    {
        _targetGenerator.StartLevel(); 
        EnablePlayerControl(true); 
    }

    public void PrepareGame()
    {
        _targetGenerator.PrepareLevel();
        _ballController.PrepareLevel();
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

    public void StopGame(bool b)
    {
        EnablePlayerControl(false);

        _targetGenerator.StopLevel();
        _ballController.StopLevel();
    }
}