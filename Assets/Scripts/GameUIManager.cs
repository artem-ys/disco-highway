using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

public class GameUIManager : MonoBehaviour
{
    public CanvasGroup startScreen;
    public CanvasGroup lostScreen;
    public CanvasGroup winScreen;
    private GameStateMachine _stateMachine;

    [Inject]
    public void InjectDependencies(
        GameStateMachine stateMachine)
    {
        this._stateMachine = stateMachine;
    }
    
    private void Start()
    {
        // Subscribe to state changes in the game state machine
        _stateMachine.CurrentGameStateType
            .Subscribe(HandleStateChange)
            .AddTo(this);
        
        // Initialize UI to show only the start screen
        SetScreenActive(startScreen, true);
        SetScreenActive(lostScreen, false);
        SetScreenActive(winScreen, false);
    }

    private void HandleStateChange(GameStateType newState)
    {
        switch (newState)
        {
            case GameStateType.Pause:
                SetScreenActive(startScreen, true);
                SetScreenActive(lostScreen, false);
                SetScreenActive(winScreen, false);
                break;
            case GameStateType.Play:
                SetScreenActive(startScreen, false);
                SetScreenActive(lostScreen, false);
                SetScreenActive(winScreen, false);
                break;
            case GameStateType.Lose:
                SetScreenActive(lostScreen, true);
                break;
            case GameStateType.Win:
                SetScreenActive(winScreen, true);
                break;
            
        }
    }

    private void SetScreenActive(CanvasGroup screen, bool isActive)
    {
        if(isActive)
            screen.gameObject.SetActive(isActive);
        
        screen.DOFade(isActive ? 1.0f : 0.0f, 1.0f)
            .OnStart(() => screen.blocksRaycasts = isActive)
            .OnComplete(() =>
            {
                screen.interactable = isActive;
                
                if(!isActive)
                    screen.gameObject.SetActive(false);
            });
    }
}
