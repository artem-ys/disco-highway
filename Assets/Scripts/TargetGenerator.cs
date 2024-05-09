using UniRx;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Zenject;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TargetSpawnData
{
    public TargetType type;
    public int rowId;
    public float timeToReach; 
    public float speed;
    public int beatNum;
    public AudioController audioController;
    public IGameManager gameManager;
    public TargetPool pool;
    public AudioSource audioSource;

    public TargetSpawnData()
    {
    }

    public TargetSpawnData(TargetType type, int rowId, float timeToReach, int beatNum)
    {
        this.type = type;
        this.rowId = rowId;
        this.timeToReach = timeToReach;
        this.beatNum = beatNum;
    }
}

public class TargetGenerator : MonoBehaviour
{
    private List<TargetSpawnData> spawnData = new List<TargetSpawnData>();
    private List<Target> allTargets = new List<Target>();
    public AudioSource audioSource;
    public AudioController audioController;
    
    public bool IsActivated { get; private set; } = false;
    
    // Injected array of all target pools
    private TargetPool[] _targetPools;
    private BottomBlocksManager _bottomBlocksManager;
    
    public float spawnInterval = 2f; // Seconds
    public float moveDistance = -60f; // Negative Z direction
    public float moveDuration = 0.1f; // Duration for the target to reach its destination
    private IGameManager _gameManager;
    private float _startTime;

    [Inject]
    public void InjectDependencies(TargetPool[] targetPools,
        BottomBlocksManager bottomBlocksManager,
        IGameManager gameManager)
    {
        this._targetPools = targetPools;
        this._bottomBlocksManager = bottomBlocksManager;
        this._gameManager = gameManager;
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
    
    public class BeatMultiplierRange
    {
        public int StartIndex;
        public int EndIndex;
        public int Multiplier;

        public BeatMultiplierRange(int start, int end, int multiplier)
        {
            StartIndex = start;
            EndIndex = end;
            Multiplier = multiplier;
        }
    }
    
    private void PopulateLevelData()
    {
        var multiplierRanges = new List<BeatMultiplierRange>
        {
            new(0, 9, 2), 
           
            new(17, 33, 2),
            
            new(64, 79, 2),
            
            new(97, 131, 4), 
            
            
            new(131, 160, 2),
            
            //new(1, 160, 2),
            
            new(190, 226, 2),
            
            new(226, 250, 4),
            
            new(297, 331, 4)
        };

        spawnData.Clear();

        float beatTime = (60.0f / 170.0f);
        float timeToReach = 0;
        int beatNum = 0;

        int totalBeats = Mathf.FloorToInt(audioSource.clip.length / beatTime);
        
        for (int i = 0; i < 550; i++)
        {
            var type = i < 8 ? 0 : (TargetType)(i%2);
            var rowId = i < 8 ? 2 : i % 5;
            
            var beat = beatTime;
            var beatMul = 1;
            
            foreach (var range in multiplierRanges.Where(range => beatNum >= range.StartIndex && beatNum < range.EndIndex))
            {
                beatMul = range.Multiplier;
                break;
            }

            beat *= beatMul;
            
            spawnData.Add(new TargetSpawnData(type, rowId, timeToReach, beatNum));
            
            spawnData.Add(new TargetSpawnData(TargetType.EmptyPlatform, 2, timeToReach, beatNum));
            
            if (type == TargetType.WrongPlatform)
            {
                rowId = GetRandomExcluding(0, 5, rowId, 1);
                
                spawnData.Add(new TargetSpawnData(
                    TargetType.StandardPlatform, 
                    rowId,
                    timeToReach, 
                    beatNum));
            }
            
            beatNum += beatMul;
            timeToReach += beat;

            if (timeToReach > audioSource.clip.length)
                break;
        }
    }

    public void PrepareLevel()
    {
        foreach (var t in allTargets)
        {
            t.Pool.Despawn(t);
        }
       
        allTargets.Clear();
            
            
        PopulateLevelData();

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
            targetSpawnData.audioController = this.audioController;
            targetSpawnData.gameManager = _gameManager;
            targetSpawnData.pool = selectedPool;
            targetSpawnData.audioSource = audioSource;
            
            var target = selectedPool.Spawn(spawnPosition, Quaternion.identity, targetSpawnData);
            
            allTargets.Add(target); 
        }
    }
    
    public void StartLevel()
    {
        _startTime = Time.time;
        
        audioSource.Play();

        audioSource.pitch = 1;
        
       /* DOTween.To(() => audioSource.pitch, x =>
            {
                audioSource.pitch = x; 
                
                allTargets.ForEach( t => t.SlowDownMovement(x));
            }, 1, 2f)
            .OnComplete(() => { });*/
        
        IsActivated = true;
        
        foreach (var target in allTargets)
        {
            target.Launch(); 
            //target.gameObject.SetActive(true); 
        }
    }
    
    public float JumpFunc(float x, float l)
    {
        return - (x * x) / (l * l) + 1;
    }
  
    public bool FindBallBetweenTargets(float ballZPosition, out float centerDistance)
    {
        List<Target> onlyStandardTargets = allTargets.Where(target => target.Type == TargetType.StandardPlatform).ToList();
        
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
 
    public void StopLevel()
    {
        IsActivated = false;
            
        allTargets.ForEach(t => t.Deactivate());
        
        // Slow down and stop audio
        DOTween.To(() => audioSource.pitch, x =>
            {
                audioSource.pitch = x; 
                
                allTargets.ForEach( t => t.SlowDownMovement(x));
            }, 0, 2f)
            .OnComplete(() => audioSource.Stop());
    }
}