using UnityEngine;
using Zenject;

public abstract class TargetPool : MonoMemoryPool<Vector3, Quaternion, TargetSpawnData, Target>
{
    protected override void Reinitialize(Vector3 position, Quaternion rotation, TargetSpawnData spawnData, Target item)
    {
        item.ResetTarget();
        var transform = item.transform;
        transform.position = position;
        transform.rotation = rotation;
        item.Initialize(spawnData);
        item.gameObject.SetActive(true);
    }

    protected override void OnDespawned(Target item)
    {
        base.OnDespawned(item);
        item.gameObject.SetActive(false);
    }
}

public class TargetType1Pool : TargetPool { }
public class TargetType2Pool : TargetPool { }

public class TargetType3Pool : TargetPool { }