using System;
using UnityEngine;
using Zenject;

public class Block : MonoBehaviour, ICollidable
{
    private IBallShooter _ballShooter;
    private CollisionManager _collisionManager;
    public int rowId { get; set; }
    public CollidableType CollidableType => CollidableType.Block;

    public void HandleCollision(ICollidable other)
    {
        
    }

    [Inject]
    public void Initialize(IBallShooter ballShooter, CollisionManager collisionManager)
    {
        this._ballShooter = ballShooter;
        this._collisionManager = collisionManager;
    }

    private void Awake()
    {
        _collisionManager.RegisterCollidable(this);
    }

    public void Activate()
    {
        Debug.Log("Block activated: " + gameObject.name);
        _ballShooter.ShootBallFrom(transform.position);

    }

}