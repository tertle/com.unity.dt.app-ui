namespace Unity.AppUI.Samples.MVVMRedux
{
    public interface ILocalStorageService
    {
        T GetValue<T>(string key, T defaultValue = default(T));

        void SetValue<T>(string key, T value);
    }
}
