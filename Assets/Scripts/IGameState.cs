using System;

public interface IGameState : IDisposable
{
    void EnterState();
    void UpdateState();
    void ExitState();
}