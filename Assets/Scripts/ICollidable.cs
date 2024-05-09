public enum CollidableType
{
    Ball,
    Block,
    Target,
}

public interface ICollidable
{
    int rowId { get; set; }
    
    CollidableType CollidableType { get; } 
    void HandleCollision(ICollidable other);
}