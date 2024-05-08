using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TargetType
{
    StandardPlatform,
    WrongPlatform,
}

public class Target : MonoBehaviour, ICollidable
{
    public TargetType targetType;
    private float moveDistance;
    private float duration;
    private float speed;
    public int RowId { get; set; }
    public Transform platformTransform;
    public TMP_Text beatNum;
    
    public CollidableType Type => CollidableType.Target;

    public void HandleCollision(ICollidable other)
    {
        
    }

    public void Initialize(TargetSpawnData spawnData)
    {
        this.targetType = spawnData.type;
        this.RowId = spawnData.rowId;
        this.moveDistance = spawnData.timeToReach * spawnData.speed;
        this.duration = spawnData.timeToReach;
        this.speed = spawnData.speed;

        //if(beatNum != null)
        //    beatNum.text = spawnData.beatNum.ToString();
    }

    public void Launch()
    {
        var addMove = 100.0f;
        var addTime = addMove / speed;
        
        transform.DOMoveZ(-addMove,  duration + addTime)
            .SetEase(Ease.Linear)
            .OnComplete(OnReachedDestination);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(gameObject.CompareTag("CorrectBlock"))
            TriggerBounceEffect();
        
        if(gameObject.CompareTag($"WrongBlock"))
            TriggerWrongBounceEffect();
    }

    private void TriggerBounceEffect()
    {
        float bounceHeight = -2f; 
        int numJumps = 1;  
        float bounceDuration = 0.25f;  
        
        platformTransform.DOLocalJump(platformTransform.up * 0.1f, bounceHeight, numJumps, bounceDuration)
            .SetEase(Ease.OutQuad);  
    }
    private void TriggerWrongBounceEffect()
    {
        platformTransform.DOPunchPosition(platformTransform.up * 2f, 2)
            .SetEase(Ease.OutQuad);  
    }
    
    private void OnReachedDestination()
    {
        //Debug.Log("Target reached its destination.");
        gameObject.SetActive(false); // Deactivate the target for now.
    }

    public void ResetTarget()
    {
        DOTween.Kill(transform); // Stop any DOTween animations on this transform.
        transform.position = Vector3.zero;
        gameObject.SetActive(true); // Reactivate the target for reuse.
    }
}