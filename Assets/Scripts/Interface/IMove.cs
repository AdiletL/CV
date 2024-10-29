public interface IMove
{
    public float MovementSpeed { get; set; }
    public float RotationSpeed { get; set; }
    
    public void Move();
    public void Rotate();
}