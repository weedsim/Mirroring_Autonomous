using Fusion;

public interface IPackable<out T> 
{
    T Pack();
}
