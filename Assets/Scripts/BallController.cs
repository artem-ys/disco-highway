using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

public class BallController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 360f; // Degrees per second
    public float jumpHeight = 5f; // Maximum height the ball jumps
    public float targetThreshold = 1f; // Distance threshold to consider 'directly above' a target
    
    private TargetGenerator _targetGenerator;
    private GameStateMachine _gameStateMachine;
    private IDisposable _controlSubscription;
    private Vector3 targetPosition;
    private bool _isFalling;

    [Inject]
    public void InjectDependencies(TargetGenerator targetGenerator, 
                                    GameStateMachine gameStateMachine)
    {
        this._targetGenerator = targetGenerator;
        this._gameStateMachine = gameStateMachine;
        
        
    }
    
    private void Start()
    {
        transform.DORotate(new Vector3(rotationSpeed, 0f, 0f), 1f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
     
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0))
            .Where(_ => _targetGenerator.IsActivated)
            .Select(_ => Input.mousePosition)
            .Select(screenPos => Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.transform.position.y)))
            .Subscribe(worldPos =>
            {
                var position = transform.position;
                targetPosition = new Vector3(worldPos.x*3, position.y, position.z);

                targetPosition.x = Mathf.Clamp(targetPosition.x, -8.5f, 8.5f);
                /*var currentPosition = transform.position;
                var inputPosition = new Vector3(worldPos.x, currentPosition.y, currentPosition.z);

                // Apply sensitivity by interpolating between the current position and the input position
                targetPosition = Vector3.Lerp(currentPosition, inputPosition, 2f);*/
            })
            .AddTo(this);
        
        this.UpdateAsObservable()
            .Where(_ => _targetGenerator.IsActivated)
            .Subscribe(_ =>
            {
                MoveAndJump();
            })
            .AddTo(this);
    }

    private void MoveAndJump()
    {
        if (_isFalling)
        {
            return;
        }
        
        var position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        float height = 0f;
        
        if (_targetGenerator.FindBallBetweenTargets(transform.position.z, out var centerDistance))
        {
            height = jumpHeight*centerDistance;
        }
        
        position.y = height + 1.0f;
        
        transform.position = position;
    }

    public void StopLevel()
    {
        _isFalling = true;
        
        transform.DOMoveY(-25f, 1f)
            .SetEase(Ease.InQuad);
    }

    public void PrepareLevel()
    {
        _isFalling = false;
        
        transform.DOMove(new Vector3(0f,2.46f,0f), 1f)
            .SetEase(Ease.InQuad);
    }
}
