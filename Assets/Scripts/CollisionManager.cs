using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private List<ICollidable> collidables = new();

    public void RegisterCollidable(ICollidable collidable)
    {
        collidables.Add(collidable);
    }

    private void Start()
    {
        Observable.EveryUpdate().Subscribe(_ => CheckForCollisions()).AddTo(this);
    }

    private void CheckForCollisions()
    {
        foreach (var collidable1 in collidables)
        {
            foreach (var collidable2 in collidables)
            {
                // Ensure that we only consider collisions between targets and blocks/balls
                if (collidable1 != collidable2 && 
                    collidable1.RowId == collidable2.RowId && 
                    ((collidable1.Type == CollidableType.Target && collidable2.Type != CollidableType.Target) ||
                     (collidable2.Type == CollidableType.Target && collidable1.Type != CollidableType.Target)) &&
                    Mathf.Abs(((MonoBehaviour)collidable1).transform.position.z - ((MonoBehaviour)collidable2).transform.position.z) < 1f)
                {
                    collidable1.HandleCollision(collidable2);
                    collidable2.HandleCollision(collidable1);
                }
            }
        }
    }
}