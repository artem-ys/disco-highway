using DG.Tweening;
using UnityEngine;
using Zenject;

public class Ball : MonoBehaviour, ICollidable
{
    private CollisionManager _collisionManager;
    
    [Inject]
    public void Initialize(CollisionManager collisionManager)
    { 
        this._collisionManager = collisionManager;
    }
    public void Launch(float speed, int rowId)
    {
        this.rowId = rowId;
        this._collisionManager.RegisterCollidable(this);
        
        float moveDistance = 100f; // Example distance
        float duration = moveDistance / speed; // Calculate duration to keep consistent speed
       
        transform.DOMoveZ(transform.position.z + moveDistance, duration)
            .SetEase(Ease.Linear) // Use a linear ease for consistent movement speed.
            .OnComplete(OnReachedDestination); // Optional: Do something when the movement is complete.
        
        float rotationAmount = moveDistance / (Mathf.PI * transform.localScale.x); // Circumference = PI * Diameter, but using scale as a rough diameter
        
        transform.DOBlendableRotateBy(Vector3.right * rotationAmount * 360f, duration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);

    }

    public int rowId { get; set; }
    public CollidableType CollidableType => CollidableType.Ball;

    public void HandleCollision(ICollidable other)
    {
        
    }

    public void ResetBall()
    {
        DOTween.Kill(transform); // Stop any DOTween animations on this transform.
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    private void OnReachedDestination()
    {
        // Handle logic when the ball reaches its destination.
        // For example, you might want to deactivate the ball or return it to a pool.
        Debug.Log("Ball reached its target.");
        // If using pooling, return the ball to the pool instead of deactivating.
    }

}