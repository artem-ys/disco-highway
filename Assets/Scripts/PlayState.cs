using UnityEditor;
using UnityEngine;

public class PlayState : IGameState
{
    private readonly IGameManager _gameManager;

    public PlayState(IGameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void EnterState()
    {
        _gameManager.EnablePlayerControl(true);
        Debug.Log("Game is now in Play State.");
    }

    public void UpdateState()
    {
        // If the pause button is pressed, change to PauseState
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _gameManager.ChangeState(GameStateType.Pause);
        }
    }

    public void ExitState()
    {
        _gameManager.EnablePlayerControl(false);
    }

    public void Dispose()
    {
    }
}