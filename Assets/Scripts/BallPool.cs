using UnityEngine;
using Zenject;

public class BallPool : MonoMemoryPool<Vector3, float, Ball>
{
    protected override void Reinitialize(Vector3 position, float speed, Ball item)
    {
        item.ResetBall();
        item.transform.position = position;
        item.Launch(speed, item.rowId); // Assuming forward launch for simplicity.
    }
}