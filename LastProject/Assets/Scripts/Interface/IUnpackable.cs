using Fusion;

public interface IUnpackable<in T>
{
    void Unpack(T packedInstance);
}
