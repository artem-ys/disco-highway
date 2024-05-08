public enum CollidableType
{
    Ball,
    Block,
    Target,
}

public interface ICollidable
{
    int RowId { get; set; }
    
    CollidableType Type { get; } 
    void HandleCollision(ICollidable other);
}