using UniRx;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Zenject;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetSpawnData
{
    public TargetType type;
    public int rowId;
    public float timeToReach; // Time for the target to reach Z=0 from the start
    public float speed;
    public int beatNum;
}

public class TargetGenerator : MonoBehaviour
{
    private List<TargetSpawnData> spawnData = new List<TargetSpawnData>();
    private List<Target> allTargets = new List<Target>();
    public AudioSource audio;
    
    public bool IsActivated { get; private set; } = false;
    
    // Injected array of all target pools
    private TargetPool[] _targetPools;
    private BottomBlocksManager _bottomBlocksManager;
    
    public float spawnInterval = 2f; // Seconds
    public float moveDistance = -60f; // Negative Z direction
    public float moveDuration = 0.1f; // Duration for the target to reach its destination
   
    [Inject]
    public void InjectDependencies(TargetPool[] targetPools,
        BottomBlocksManager bottomBlocksManager)
    {
        this._targetPools = targetPools;
        this._bottomBlocksManager = bottomBlocksManager;
    }
    
    public int GetRandomExcluding(int n, int m, int k, int dist)
    {
        int result;
        do
        {
            result = Random.Range(n, m); 
        }
        while (Math.Abs(result - k) <= dist);

        return result;
    }
    
    public static int GetRandomNumber(int n, int k, float[] probabilities)
    {
        if (probabilities.Length != k - n + 1)
        {
            throw new ArgumentException("The probabilities array length must match the number of possible outcomes.");
        }

        float totalProbability = 0f;
        foreach (var prob in probabilities)
        {
            totalProbability += prob;
        }

        if (totalProbability == 0)
        {
            throw new ArgumentException("The sum of all probabilities cannot be zero.");
        }

        for (int i = 0; i < probabilities.Length; i++)
        {
            probabilities[i] /= totalProbability;
        }

        float[] cumulative = new float[probabilities.Length];
        cumulative[0] = probabilities[0];
        for (int i = 1; i < probabilities.Length; i++)
        {
            cumulative[i] = cumulative[i - 1] + probabilities[i];
        }
        
        float randomValue = UnityEngine.Random.value;
        
        for (int i = 0; i < cumulative.Length; i++)
        {
            if (randomValue <= cumulative[i])
            {
                return n + i;
            }
        }
        return n;
    }
    
    private void PopulateTestData()
    {
        spawnData.Clear();

        float beatTime = (60.0f / 170.0f);
        float timeToReach = 0;
        
        for (int i = 0; i < 250; i++)
        {
            var type = i < 8 ? 0 : (TargetType)(i%2);
            var rowId = i < 8 ? 2 : i % 5;
            
            var beat = beatTime;

            if (i < 8)
                beat *= 2;
            
            if (i > 24 && i < 50)
                beat *= 2;
            
            if (i > 64 && i < 80)
                beat *= 4;
            
            spawnData.Add(new TargetSpawnData()
            {
                type = type, 
                rowId = rowId, 
                timeToReach = timeToReach,
                beatNum = i
            });
            
            if (type == TargetType.WrongPlatform)
            {
                rowId = GetRandomExcluding(0, 5, rowId, 1);
                
                spawnData.Add(new TargetSpawnData()
                {
                    type = TargetType.StandardPlatform, 
                    rowId = rowId, 
                    timeToReach = timeToReach,
                    beatNum = i
                });
            }
            
            timeToReach += beat;
        }
    }

    private void Awake()
    {
        PopulateTestData();

        PrepareTargets();
    }
    
    private void PrepareTargets()
    {
        foreach (var targetSpawnData in spawnData)
        {
            var speed = 50.0f;
            var startMovePosition = targetSpawnData.timeToReach * speed;
            var bottomBlock = _bottomBlocksManager.bottomBlocks[targetSpawnData.rowId];
            var spawnPosition = new Vector3(bottomBlock.transform.position.x, 0, startMovePosition);
            var selectedPool = _targetPools[(int)targetSpawnData.type];
            targetSpawnData.speed = speed;
            
            var target = selectedPool.Spawn(spawnPosition, Quaternion.identity, targetSpawnData);
            //target.gameObject.SetActive(false); // Initially inactive, activated when level starts
            
            allTargets.Add(target); 
        }
    }
    
    public void StartLevel()
    {
        audio.Play();
        
        IsActivated = true;
        
        foreach (var target in allTargets)
        {
            target.Launch(); 
            target.gameObject.SetActive(true); 
        }
    }
    
    public float JumpFunc(float x, float l)
    {
        return - (x * x) / (l * l) + 1;
    }
    
    public float JumpFunc2(float x, float l)
    {
        if (x < -l || x > l)
        {
            return 0f;
        }

        return 0.5f*(float)Math.Cos(Math.PI * x / l) + 0.5f;
    }
    
    public bool FindBallBetweenTargets(float ballZPosition, out float centerDistance)
    {
        List<Target> onlyStandardTargets = allTargets.Where(target => target.targetType == TargetType.StandardPlatform).ToList();
        
        for (int i = 0; i < onlyStandardTargets.Count - 1; i++)
        {
            float z1 = onlyStandardTargets[i].transform.position.z;
            float z2 = onlyStandardTargets[i + 1].transform.position.z;
            
            // Check if the ball's Z position is between z1 and z2
            if (ballZPosition >= z1 && ballZPosition <= z2)
            {
                var distanceFromCenter = Mathf.Abs(ballZPosition - ((z2 + z1) / 2.0f));
                var flyDistance = Mathf.Abs(z2 - z1);
                var jumpHeightCoef = flyDistance / 40.0f;

                centerDistance = JumpFunc(distanceFromCenter, flyDistance/2)*jumpHeightCoef;
                
                //Debug.Log($"{distanceFromCenter} {z1} {z2} {l} {centerDistance}");
                
                return true;
            }
        }
        
        centerDistance = 0;
        return false;
    }
    
    public float[] GetTargetDistances(float zPosition)
    {
        return allTargets
            //.Where(target => target.transform.position.z > zPosition)
            .Select(target => Mathf.Abs(target.transform.position.z - zPosition))
            .Distinct()
            .ToArray();
    }
}