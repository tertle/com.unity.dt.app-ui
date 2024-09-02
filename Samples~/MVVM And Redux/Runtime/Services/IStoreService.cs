using Unity.AppUI.Redux;

namespace Unity.AppUI.Samples.MVVMRedux
{
    public interface IStoreService
    {
        Store store { get; }
    }
}
