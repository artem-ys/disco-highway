using UniRx;
using UnityEngine;

public class LoseState : IGameState
{
    private readonly IGameManager _gameManager;

    public LoseState(IGameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void EnterState()
    {
        _gameManager.StopGame(false);
        
        Debug.Log("Game is now in Lose State.");
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