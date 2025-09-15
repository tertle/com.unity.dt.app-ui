using Unity.AppUI.Redux;

namespace Unity.AppUI.Samples.MVVMRedux
{
    public interface IStoreService
    {
        IStore<PartitionedState> store { get; }

        public string sliceName { get; }

        void SaveState();
    }
}
