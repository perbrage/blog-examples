namespace Brage.Infrastructure
{
    public interface IHandle<in T>
    {
        void Handle(T instance);
    }
}
