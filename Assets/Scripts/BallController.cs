using System.Collections.Generic;
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
    private Vector3 targetPosition;

    [Inject]
    public void InjectDependencies(TargetGenerator targetGenerator)
    {
        this._targetGenerator = targetGenerator;
    }
    
    private void Start()
    {
        transform.DORotate(new Vector3(rotationSpeed, 0f, 0f), 1f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
        
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0))
            .Select(_ => Input.mousePosition)
            .Select(screenPos => Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.transform.position.y)))
            .Subscribe(worldPos =>
            {
                var position = transform.position;
                targetPosition = new Vector3(worldPos.x, position.y, position.z);
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
        var position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        float height = 0f;
        
        if (_targetGenerator.FindBallBetweenTargets(transform.position.z, out var centerDistance))
        {
            height = jumpHeight*centerDistance;
        }
        
        position.y = height + 1.0f;
        
        transform.position = position;
    }
    
}
