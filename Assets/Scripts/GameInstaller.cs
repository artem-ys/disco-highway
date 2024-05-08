using Zenject;

public class GameInstaller : MonoInstaller
{
    public Ball ballPrefab; 
    public Target targetPrefabType1;
    public Target targetPrefabType2;
    
    public override void InstallBindings()
    {
        //Container.Bind<GameStateFactory>().AsSingle();
        Container.Bind<IGameStateFactory>().To<GameStateFactory>().AsSingle();
        Container.Bind<GameStateMachine>().AsSingle().NonLazy();
        
        Container.BindInterfacesAndSelfTo<PlayState>().AsTransient();
        Container.BindInterfacesAndSelfTo<PauseState>().AsTransient();
        // Add bindings for other states as needed.
        
        Container.Bind<IGameManager>().To<GameManager>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<BottomBlocksManager>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
        
        Container.Bind<IBallShooter>().To<BallShooter>().AsSingle();
        
        Container.BindMemoryPool<Ball, BallPool>()
            .WithInitialSize(20) // Initial pool size
            .FromComponentInNewPrefab(ballPrefab)
            .UnderTransformGroup("Balls"); // Organize pooled objects under this transform
        
        // Bind TargetGenerator
        Container.Bind<TargetGenerator>().FromComponentInHierarchy().AsSingle();

        // Bind target pools
        Container.BindMemoryPool<Target, TargetType1Pool>()
            .FromComponentInNewPrefab(targetPrefabType1)
            .UnderTransformGroup("Targets");
        Container.BindMemoryPool<Target, TargetType2Pool>()
            .FromComponentInNewPrefab(targetPrefabType2)
            .UnderTransformGroup("Targets");
        // Add bindings for more pools as needed.

        // Bind the array of all target pools to TargetPool
        Container.Bind<TargetPool>().To<TargetType1Pool>().FromResolve();
        Container.Bind<TargetPool>().To<TargetType2Pool>().FromResolve();
        //Container.Bind<TargetPool[]>().FromResolveAll();
        
        Container.Bind<CollisionManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        Container.Bind<BallController>().FromComponentInHierarchy().AsSingle();
        
    }
    
}