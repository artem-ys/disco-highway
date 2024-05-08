using UnityEngine;
using Zenject;

public class BallShooter : IBallShooter
{
    [Inject]
    private BallPool _ballPool;
    
    public void ShootBallFrom(Vector3 position)
    {
        float shootSpeed = 10f; // Example speed
        _ballPool.Spawn(position, shootSpeed);
    }
}
