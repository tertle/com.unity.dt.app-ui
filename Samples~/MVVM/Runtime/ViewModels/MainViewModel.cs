using System;
using Unity.AppUI.MVVM;

namespace Unity.AppUI.Samples.MVVM
{
    public class MainViewModel : ObservableObject
    {
        int m_Counter;

        public int counter
        {
            get => m_Counter;
            set => SetProperty(ref m_Counter, value);
        }

        public MainViewModel()
        {
            counter = 0;
        }

        public void IncrementCounter()
        {
            counter++;
        }
    }
}
