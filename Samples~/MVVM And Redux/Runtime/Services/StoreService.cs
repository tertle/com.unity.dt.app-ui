using Unity.AppUI.Redux;

namespace Unity.AppUI.Samples.MVVMRedux
{
    public class StoreService : IStoreService
    {
        public Store store { get; }

        public StoreService()
        {
            store = new Store();
        }
    }
}
