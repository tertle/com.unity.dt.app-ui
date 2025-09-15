using System;
using UnityEngine;

namespace Unity.AppUI.Samples.MVVMRedux
{
    [Serializable]
    public class Todo
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string text;

        [SerializeField]
        public bool completed;

        public override string ToString()
        {
            return @$"{{
        id: {id},
        text: {text},
        completed: {completed}
    }}";
        }
    }
}
