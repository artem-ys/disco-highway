using UniRx;
using UnityEngine;

public class WinState : IGameState
{
    private readonly IGameManager _gameManager;

    public WinState(IGameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void EnterState()
    {
        _gameManager.StopGame(true);
        
        Debug.Log("Game is now in Win State.");
    }

    public void UpdateState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _gameManager.ChangeState(GameStateType.Pause); // Or GameStateType.NextLevel if applicable
        }
    }

    public void ExitState()
    {
        // Clean up lose state-specific UI or effects
    }

    public void Dispose()
    {
        // Handle any required disposal actions
    }
}