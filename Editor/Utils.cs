using UnityEditor;
using UnityEngine;

namespace Unity.AppUI.Editor
{
    static class Utils
    {
        internal static bool AddItemInArray(SerializedProperty array, Object item)
        {
            if (IndexOf(array, item) == -1)
            {
                var arrayIndex = array.arraySize;
                array.InsertArrayElementAtIndex(arrayIndex);
                var arrayElem = array.GetArrayElementAtIndex(arrayIndex);
                arrayElem.objectReferenceValue = item;
                return true;
            }

            return false;
        }

        internal static int IndexOf(SerializedProperty array, Object item)
        {
            for (var i = 0; i < array.arraySize; ++i)
            {
                var arrayElem = array.GetArrayElementAtIndex(i);
                if (item == arrayElem.objectReferenceValue)
                    return i;
            }

            return -1;
        }
    }
}
