namespace Unity.AppUI.UI
{
    class TextActions
    {
        public const int Cut = 11;

        public const string CutLabel =
#if UNITY_LOCALIZATION_PRESENT
            "@UI:cut";
#else
            "Cut";
#endif

        public const int Copy = 12;

        public const string CopyLabel =
#if UNITY_LOCALIZATION_PRESENT
            "@UI:copy";
#else
            "Copy";
#endif

        public const int Paste = 13;

        public const string PasteLabel =
#if UNITY_LOCALIZATION_PRESENT
            "@UI:paste";
#else
            "Paste";
#endif
    }
}
