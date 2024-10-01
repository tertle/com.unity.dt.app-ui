using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.AppUI.UI;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Object = UnityEngine.Object;

namespace Unity.AppUI.Editor
{
    class IconBrowser : EditorWindow
    {
        const string k_RegularVariant = "regular";

        const string k_FillVariant = "fill";

        const string k_BoldVariant = "bold";

        [UnityEditor.MenuItem("Window/App UI/Icon Browser")]
        public static void ShowWindow()
        {
            var window = GetWindow<IconBrowser>();
            window.titleContent = new GUIContent("App UI - Icon Browser");
            window.Show();
        }

        [SerializeField]
        StyleSheet styleSheetAsset;

        [SerializeField]
        string tempContent;

        void CreateGUI()
        {
            var root = rootVisualElement;
            root.AddToClassList("icon-browser");
            var panel = new Panel();
            panel.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.unity.dt.app-ui/PackageResources/Styles/Themes/App UI.tss"));
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.unity.dt.app-ui/Editor/IconBrowser.uss"));
            panel.scale = "small";
            panel.theme = EditorGUIUtility.isProSkin ? "editor-dark" : "editor-light";

            // create UI
            var header = new Label("Choose or create a new style sheet to browse its icons.\n" +
                "Then you can add new icons by dragging and dropping them from the Project panel, or remove existing ones.");
            header.AddToClassList("header");
            root.Add(header);
            var hint = new HelpBox("Select or create a new style sheet to browse icons", HelpBoxMessageType.Info);
            root.Add(hint);
            var objectField = new ObjectField("Style Sheet")
            {
                objectType = typeof(StyleSheet),
                allowSceneObjects = false,
            };
            root.Add(objectField);
            var generateButton = new Button(OnGenerateButtonClicked)
            {
                text = "Create a new App UI Icons style sheet"
            };
            root.Add(generateButton);
            var gridView = new GridView();
            gridView.dragger.acceptStartDrag = _ => false;
            gridView.dragger.acceptDrag = () => false;
            gridView.style.flexGrow = 1;
            gridView.makeItem = () => new IconEntryElement();
            gridView.bindItem = (element, i) => ((IconEntryElement)element).icon = (IconEntry)gridView.itemsSource[i];
            gridView.itemHeight = 100;
            gridView.columnCount = 4;
            gridView.selectionType = SelectionType.Multiple;
            root.Add(panel);
            panel.Add(gridView);

            var footer = new VisualElement { name = "footer" };
            footer.AddToClassList("footer");
            root.Add(footer);
            var msg = new Label { name = "message" };
            msg.AddToClassList("footer__message");
            footer.Add(msg);

            var buttonsGroup = new VisualElement();
            buttonsGroup.AddToClassList("footer__buttons");
            footer.Add(buttonsGroup);

            var addIconsButton = new Button(OnAddIconsClicked);
            addIconsButton.name = "add";
            addIconsButton.text = "Add icons...";
            addIconsButton.AddToClassList("footer__button");
            var saveFileButton = new Button(SaveFile);
            saveFileButton.name = "save";
            saveFileButton.text = "Save file";
            saveFileButton.AddToClassList("footer__button");
            buttonsGroup.Add(addIconsButton);
            buttonsGroup.Add(saveFileButton);

            // setup data binding
            panel.RegisterCallback<DragEnterEvent>(OnDragEnter);
            panel.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            panel.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            panel.RegisterCallback<DragPerformEvent>(OnDragPerform);
            objectField.RegisterValueChangedCallback(OnSelectedStyleSheetChanged);
            gridView.RegisterCallback<GeometryChangedEvent>(OnGridViewGeometryChanged);
            gridView.RegisterCallback<KeyUpEvent>(OnGridViewKeyUp);
            gridView.RegisterCallback<PointerUpEvent>(OnContextClicked);

            // refresh UI
            RefreshUI();
        }

        void OnContextClicked(PointerUpEvent evt)
        {
            if (evt.button == (int) MouseButton.RightMouse)
            {
                evt.StopPropagation();

                var menu = new GenericMenu();
                var gridView = rootVisualElement.Q<GridView>();
                var selectedIndices = gridView.selectedIndices.ToList();
                var canDelete = selectedIndices.Count > 0;

                if (canDelete)
                    menu.AddItem(new GUIContent("Delete selected icons"), false, DeleteSelectedIcons);
                else
                    menu.AddDisabledItem(new GUIContent("Delete selected icons"), false);
                menu.ShowAsContext();
            }
        }

        void DeleteSelectedIcons()
        {
            var gridView = rootVisualElement.Q<GridView>();
            var selectedIndices = gridView.selectedIndices.ToList();

            if (selectedIndices.Count > 0)
            {
                if (EditorUtility.DisplayDialog("Delete selected icons",
                        "Are you sure you want to delete the selected icons from the stylesheet?", "Yes", "No"))
                {
                    var source = gridView.itemsSource.Cast<IconEntry>().ToList();
                    var toDelete = selectedIndices.Select(i => source[i]).ToList();
                    var additionalIcons = ParseStyleSheet(tempContent);
                    var newIcons = additionalIcons
                        .Where(icn => toDelete.FindIndex(icn2 => icn2.path == icn.path) < 0).ToList();
                    tempContent = GetStyleSheetContent(newIcons);
                    RefreshUI();
                }
            }
        }

        void OnDragEnter(DragEnterEvent evt)
        {
            evt.StopPropagation();
        }

        void OnDragLeave(DragLeaveEvent evt)
        {
            evt.StopPropagation();
        }

        void OnDragUpdated(DragUpdatedEvent evt)
        {
            if (DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences.Any(obj => obj is Texture2D))
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            else
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

            evt.StopPropagation();
        }

        void OnDragPerform(DragPerformEvent evt)
        {
            if (DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences.Any(obj => obj is Texture2D))
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            else
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

            if (DragAndDrop.objectReferences.Length > 0)
            {
                var additionalIcons = ParseStyleSheet(tempContent);
                foreach (var obj in DragAndDrop.objectReferences)
                {
                    if (obj is Texture2D texture)
                    {
                        var path = AssetDatabase.GetAssetPath(texture);
                        var folderName = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path));
                        var iconName = ToKebabCase(texture.name);
                        var iconVariant = folderName.ToLowerInvariant();
                        var variants = Enum.GetNames(typeof(IconVariant)).Select(iv => iv.ToLowerInvariant()).ToList();
                        if (!variants.Contains(iconVariant))
                            iconVariant = k_RegularVariant;
                        if (additionalIcons.FindIndex(icn => icn.path == path) < 0)
                            additionalIcons.Add(new IconEntry
                            {
                                name = iconName,
                                variant = iconVariant,
                                path = path
                            });
                    }
                }
                DragAndDrop.AcceptDrag();
                tempContent = GetStyleSheetContent(additionalIcons);
                RefreshUI();
            }

            evt.StopPropagation();
        }

        void OnGridViewKeyUp(KeyUpEvent evt)
        {
            if (evt.keyCode is KeyCode.Delete or KeyCode.Backspace)
            {
                evt.StopPropagation();
                DeleteSelectedIcons();
            }
        }

        void OnAddIconsClicked()
        {
            // var menu = new GenericMenu();
            // menu.AddItem(new GUIContent("Add icons from App UI online repository"), false, OnAddIconsFromAppUIOnlineRepositoryClicked);
            // menu.AddSeparator("");
            // menu.AddItem(new GUIContent("Add a single icon from a texture..."), false, OnAddSingleIconClicked);
            //
            // menu.ShowAsContext();
            OnAddSingleIconClicked();
        }

        void OnAddIconsFromAppUIOnlineRepositoryClicked()
        {
            throw new NotImplementedException();
        }

        void OnAddSingleIconClicked()
        {
            var hiddenType = typeof(UnityEditor.Editor).Assembly.GetType( "UnityEditor.ObjectSelector" );
            var piGet = hiddenType.GetProperty( "get", BindingFlags.Public | BindingFlags.Static );
            var miShow = hiddenType.GetMethod("Show",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[]
                {
                    typeof(Texture2D),
                    typeof(Type),
                    typeof(Object),
                    typeof(bool),
                    typeof(List<int>),
                    typeof(Action<Object>),
                    typeof(Action<Object>),
#if UNITY_2022_1_OR_NEWER
                    typeof(bool),
#endif
                }, Array.Empty<ParameterModifier>());

            if (piGet == null || miShow == null)
                return;

            var objectSelector = piGet.GetValue(null);
            if (objectSelector == null)
                return;

            miShow.Invoke(objectSelector, new object[]
            {
                null,
                typeof(Texture2D),
                null,
                false,
                null,
                new Action<Object>(OnTexturePicked),
                new Action<Object>(o => { }),
#if UNITY_2022_1_OR_NEWER
                true
#endif
            });
        }

        void OnTexturePicked(Object obj)
        {
            if (obj is Texture2D texture && styleSheetAsset)
            {
                var path = AssetDatabase.GetAssetPath(texture);
                var folderName = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path));
                var iconName = ToKebabCase(texture.name);
                var iconVariant = folderName.ToLowerInvariant();
                var variants = Enum.GetNames(typeof(IconVariant)).Select(iv => iv.ToLowerInvariant()).ToList();
                if (!variants.Contains(iconVariant))
                    iconVariant = k_RegularVariant;

                var additionalIcons = ParseStyleSheet(tempContent);
                if (additionalIcons.FindIndex(icn => icn.path == path) < 0)
                    additionalIcons.Add(new IconEntry
                    {
                        name = iconName,
                        variant = iconVariant,
                        path = path
                    });

                tempContent = GetStyleSheetContent(additionalIcons);
                RefreshUI();
            }
        }

        void OnGridViewGeometryChanged(GeometryChangedEvent evt)
        {
            var gridView = rootVisualElement.Q<GridView>();
            var totalWidth = gridView.scrollView.contentContainer.layout.width;
            var columnCount = Mathf.Max(1, Mathf.FloorToInt(totalWidth / 100));
            rootVisualElement.Q<GridView>().columnCount = columnCount;
        }

        [UnityEditor.MenuItem("Assets/Create/App UI/Icons Style Sheet Asset")]
        static void CreateIconsStyleSheetAsset()
        {
            var endAction = CreateInstance<IconBrowserActions>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                endAction,
                "AppUIIcons.uss",
                null,
                null);
        }

        void OnGenerateButtonClicked()
        {
            var outPath = EditorUtility.SaveFilePanel(
                "Save App UI Icons style sheet", Application.dataPath, "AppUIIcons", "uss");
            if (string.IsNullOrEmpty(outPath))
                return;

            var adbPath = ToAdbPath(outPath);
            if (string.IsNullOrEmpty(adbPath))
            {
                EditorUtility.DisplayDialog("Invalid path",
                    "The selected path must be inside the Assets or Packages folder", "OK");
                return;
            }

            System.IO.File.WriteAllText(outPath, GetStyleSheetContent(Array.Empty<IconEntry>()));

            AssetDatabase.Refresh();

            styleSheetAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>(adbPath);
            tempContent = System.IO.File.ReadAllText(outPath);
            RefreshUI();
        }

        void OnSelectedStyleSheetChanged(ChangeEvent<Object> evt)
        {
            styleSheetAsset = evt.newValue as StyleSheet;
            tempContent = styleSheetAsset ? System.IO.File.ReadAllText(GetStyleSheetPath(styleSheetAsset)) : string.Empty;
            RefreshUI();
        }

        bool IsDirty => styleSheetAsset && tempContent != System.IO.File.ReadAllText(GetStyleSheetPath(styleSheetAsset));

        void RefreshUI()
        {
            var objectField = rootVisualElement.Q<ObjectField>();
            objectField.SetValueWithoutNotify(styleSheetAsset);

            rootVisualElement.Q<HelpBox>().style.display = styleSheetAsset == null ? DisplayStyle.Flex : DisplayStyle.None;

            var additionalIcons = ParseStyleSheet(tempContent);
            var gridView = rootVisualElement.Q<GridView>();
            gridView.itemsSource = additionalIcons;
            gridView.SetEnabled(additionalIcons.Count > 0);

            var footer = rootVisualElement.Q("footer");
            footer.SetEnabled(styleSheetAsset);

            var saveButton = footer.Q<Button>("save");
            saveButton.SetEnabled(IsDirty);

            var message = footer.Q<Label>("message");
            message.text = styleSheetAsset ? $"Total of {additionalIcons.Count + k_RequiredIcons.Length} icons " +
                $"({additionalIcons.Count} additional and {k_RequiredIcons.Length} required)" : "";
        }

        void SaveFile()
        {
            if (IsDirty)
                System.IO.File.WriteAllText(GetStyleSheetPath(styleSheetAsset), tempContent);

            var saveButton = rootVisualElement.Q<Button>("save");
            saveButton.SetEnabled(false);
        }

        static string GetStyleSheetContent(IEnumerable<IconEntry> additionalIcons)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("/*");
            sb.AppendLine(" * This style sheet was generated by the App UI Icon Browser.");
            sb.AppendLine("*/");

            sb.AppendLine(".appui {");

            sb.AppendLine("/* Required icons */");
            foreach (var icon in k_RequiredIcons)
            {
                sb.AppendLine($"    --appui-icon-{icon.name}-{icon.variant}: url(\"project:/{icon.path}\");");
            }

            sb.AppendLine("/* Additional icons */");
            foreach (var icon in additionalIcons)
            {
                sb.AppendLine($"    --appui-icon-{icon.name}-{icon.variant}: url(\"project:/{icon.path}\");");
            }

            sb.AppendLine("}");
            sb.AppendLine("");

            foreach (var icon in k_RequiredIcons)
            {
                sb.AppendLine($".appui-icon--{icon.name}--{icon.variant} {{");
                sb.AppendLine($"    --unity-image: var(--appui-icon-{icon.name}-{icon.variant});");
                sb.AppendLine("}");
                sb.AppendLine("");
            }

            foreach (var icon in additionalIcons)
            {
                sb.AppendLine($".appui-icon--{icon.name}--{icon.variant} {{");
                sb.AppendLine($"    --unity-image: var(--appui-icon-{icon.name}-{icon.variant});");
                sb.AppendLine("}");
                sb.AppendLine("");
            }

            return sb.ToString();
        }

        static List<IconEntry> ParseStyleSheet(string content)
        {
            var list = new List<IconEntry>();

            if (string.IsNullOrEmpty(content))
                return list;

            var lines = content.Split('\n');

            foreach (var line in lines)
            {
                if (line.Contains("url(\"project:/"))
                {
                    var regexPattern = @"--appui-icon-([\w-]+)-(\w+): url\(""project:\/(.+)""\);";
                    var match = Regex.Match(line, regexPattern, RegexOptions.None, TimeSpan.FromSeconds(2));
                    if (match.Success)
                    {
                        var path = match.Groups[3].Value;
                        if (k_RequiredIcons.Any(icon => icon.path == path))
                            continue;

                        list.Add(new IconEntry
                        {
                            name = match.Groups[1].Value,
                            variant = match.Groups[2].Value,
                            path = match.Groups[3].Value
                        });
                    }
                }
            }

            return list;
        }

        static readonly IconEntry[] k_RequiredIcons = new[]
        {
            new IconEntry { name = "calendar", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/Calendar.png" },

            new IconEntry { name = "caret-down", variant = k_FillVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Fill/CaretDown.png" },
            new IconEntry { name = "caret-down", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/CaretDown.png" },
            new IconEntry { name = "caret-left", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/CaretLeft.png" },
            new IconEntry { name = "caret-right", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/CaretRight.png" },
            new IconEntry { name = "caret-double-left", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/CaretDoubleLeft.png" },
            new IconEntry { name = "caret-double-right", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/CaretDoubleRight.png" },
            new IconEntry { name = "caret-up", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/CaretUp.png" },

            new IconEntry { name = "check", variant = k_BoldVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Bold/Check.png" },
            new IconEntry { name = "check", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/Check.png" },
            new IconEntry { name = "color-picker", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/ColorPicker.png" },

            new IconEntry { name = "delete", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/Delete.png" },
            new IconEntry { name = "dots-three", variant = k_BoldVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Bold/DotsThree.png" },
            new IconEntry { name = "dots-three-vertical", variant = k_BoldVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Bold/DotsThreeVertical.png" },

            new IconEntry { name = "info", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/Info.png" },

            new IconEntry { name = "list", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/List.png" },
            new IconEntry { name = "magnifying-glass", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/MagnifyingGlass.png" },
            new IconEntry { name = "menu", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/Menu.png" },
            new IconEntry { name = "minus", variant = k_BoldVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Bold/Minus.png" },
            new IconEntry { name = "minus", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/Minus.png" },

            new IconEntry { name = "plus", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/Plus.png" },

            new IconEntry { name = "resize-handle", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/ResizeHandle.png" },

            new IconEntry { name = "scene", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/Scene.png" },
            new IconEntry { name = "sub-menu-indicator", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/SubMenuIndicator.png" },

            new IconEntry { name = "users", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/Users.png" },

            new IconEntry { name = "warning", variant = k_FillVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Fill/Warning.png" },
            new IconEntry { name = "warning", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/Warning.png" },
            new IconEntry { name = "x", variant = k_RegularVariant, path = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/X.png" },
        };

        struct IconEntry
        {
            public string name;
            public string variant;
            public string path;
        }

        class IconEntryElement : VisualElement
        {
            IconEntry m_Icon;

            readonly Image m_Image;

            readonly VisualElement m_Element;

            readonly Label m_Label;

            public IconEntry icon
            {
                get => m_Icon;
                set
                {
                    m_Icon = value;
                    RefreshUI();
                }
            }

            public IconEntryElement()
            {
                AddToClassList("icon-entry");
                m_Element = new VisualElement
                {
                    pickingMode = PickingMode.Ignore
                };
                m_Element.AddToClassList("icon-entry__element");
                hierarchy.Add(m_Element);

                m_Image = new Image
                {
                    pickingMode = PickingMode.Ignore,
                };
                m_Image.AddToClassList("icon-entry__image");
                m_Element.hierarchy.Add(m_Image);

                m_Label = new Label
                {
                    pickingMode = PickingMode.Ignore,
                };
                m_Label.AddToClassList("icon-entry__label");
                m_Element.hierarchy.Add(m_Label);
            }

            void RefreshUI()
            {
                m_Image.image = AssetDatabase.LoadAssetAtPath<Texture2D>(m_Icon.path);
                m_Label.text = $"{m_Icon.name}";
            }
        }

        static string ToKebabCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                    value,
                    "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])",
                    "-$1",
                    RegexOptions.Compiled, TimeSpan.FromSeconds(2))
                .Trim()
                .ToLower()
                .Replace(".", "-");
        }

        static string ToAdbPath(string absolutePath)
        {
            var projectPath = Application.dataPath[..^"Assets".Length];
            var adbPath = absolutePath;
            if (adbPath.Length > projectPath.Length)
                adbPath = adbPath[projectPath.Length..];

            if (!adbPath.StartsWith("Assets") && !adbPath.StartsWith("Packages"))
            {
                Debug.LogError("Invalid path: " + adbPath);
                return null;
            }

            return adbPath;
        }

        static string GetStyleSheetPath(StyleSheet styleSheetAsset)
        {
            if (!styleSheetAsset)
                return null;

            var path = AssetDatabase.GetAssetPath(styleSheetAsset);
            if (path.StartsWith("Packages"))
            {
                // resolve path using PackageManager
                var pkgInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(path);
                if (pkgInfo == null)
                {
                    Debug.LogError("Unable to read Stylesheet: Failed to resolve package info for " + path);
                    return null;
                }
                var pkgPath = pkgInfo.resolvedPath;
                var assetRelativePath = path[pkgInfo.assetPath.Length..];
                path = pkgPath + assetRelativePath;
            }

            return path;
        }

        class IconBrowserActions : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                System.IO.File.WriteAllText(pathName, GetStyleSheetContent(Array.Empty<IconEntry>()));
                AssetDatabase.Refresh();

                var obj = AssetDatabase.LoadAssetAtPath<StyleSheet>(pathName);
                Selection.activeObject = obj;
            }
        }
    }
}
