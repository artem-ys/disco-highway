using DG.Tweening;
using TMPro;
using UnityEngine;

public enum TargetType
{
    StandardPlatform,
    WrongPlatform,
    EmptyPlatform,
}

public class Target : MonoBehaviour, ICollidable
{
    private TargetSpawnData _spawnData;
    
    public Transform platformTransform;
    public TMP_Text beatNum;

    public UnshObjectBehaviour beatsBehaviour;
    private Tween _moveTween;

    public int rowId
    {
        get => _spawnData.rowId;
        set => _spawnData.rowId = value;
    }

    public CollidableType CollidableType => CollidableType.Target;

    public TargetType Type => _spawnData.type;

    public TargetPool Pool => _spawnData.pool;

    private bool _isFinal = false;
    private bool _isTriggersActive = true;

    public static int lastCorrectBlock = 0;

    public void HandleCollision(ICollidable other)
    {
        
    }
    
    public void Initialize(TargetSpawnData spawnData)
    {
        _isTriggersActive = true;
        _isFinal = false;
        
        this._spawnData = spawnData;
    
        this.beatsBehaviour.audioController = spawnData.audioController;

        beatsBehaviour.enabled = false;//spawnData.type == TargetType.StandardPlatform;
        
        //if(beatNum != null)
        //    beatNum.text = spawnData.beatNum.ToString();
    }

    public void Launch()
    {
        var addMove = 100.0f;
        var currentTime = _spawnData.audioSource.time - 0.1f;
        float totalAnimationTime = _spawnData.timeToReach + (100.0f / _spawnData.speed);
        float adjustedAnimationTime = Mathf.Max(totalAnimationTime - currentTime,0.0f);
        
        _moveTween = transform.DOMoveZ(-addMove,  adjustedAnimationTime)
            .SetEase(Ease.Linear)
            .OnComplete(OnReachedDestination);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!_isTriggersActive)
        {
            return;
        }

        if (gameObject.CompareTag("CorrectBlock"))
        {
            Debug.Log($"CORRECT BLOCK {_spawnData.beatNum}");

            lastCorrectBlock = _spawnData.beatNum;
            
            if (_spawnData.beatNum > 530)
            {
                HandleGameWin();
            }

            TriggerBounceEffect();
        }

        if (gameObject.CompareTag($"WrongBlock") && _spawnData.beatNum > lastCorrectBlock)
        {
            Debug.Log($"WRONG BLOCK {_spawnData.beatNum}");
            HandleGameEnd();
            TriggerWrongBounceEffect();
        }
    }

    private void HandleGameWin()
    {
        _spawnData.gameManager.ChangeState(GameStateType.Win);
    }

    private void HandleGameEnd()
    {
        _spawnData.gameManager.ChangeState(GameStateType.Lose);
    }
    
    public void SlowDownMovement(float slowFactor)
    {
        slowFactor = Mathf.Clamp01(slowFactor);
        
        if (_moveTween != null && _moveTween.IsActive() && gameObject.activeSelf && !_isFinal)
        {
            _moveTween.timeScale = slowFactor; 
        }
    }

    private void TriggerBounceEffect()
    {
        platformTransform.DOLocalJump(platformTransform.up * 0.1f, -2f, 1, 0.25f)
            .SetEase(Ease.OutQuad);  
    }
    private void TriggerWrongBounceEffect()
    {
        //platformTransform.DOPunchPosition(platformTransform.up * 2f, 2)
        //    .SetEase(Ease.OutQuad);

        _isFinal = true;
        
        platformTransform.DOMoveY(-5f, 1f)
            .SetEase(Ease.InQuad);
    }
    
    private void OnReachedDestination()
    {
        gameObject.SetActive(false);
    }

    public void ResetTarget()
    {
        DOTween.Kill(platformTransform);
        DOTween.Kill(transform); 
        transform.position = Vector3.zero;
        gameObject.SetActive(true); 
    }

    public void Deactivate()
    {
        beatsBehaviour.enabled = false;
        _isTriggersActive = false;
    }
}