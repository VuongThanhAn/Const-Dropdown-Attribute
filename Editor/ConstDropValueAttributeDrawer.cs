using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using Const.Attribute.Runtime;
using System.Collections.Generic;

namespace Const.Attribute.Editors
{
    [CustomPropertyDrawer(typeof(ConstDropValueAttribute))]
    public class ConstDropValueAttributeDrawer : PropertyDrawer
    {
        private const BindingFlags CONST_BINDING_FLAGS = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is not ConstDropValueAttribute constDropValueAttribute) return;

            property.serializedObject.Update();

            // Mặc định dùng class ban đầu
            var type = constDropValueAttribute.DefaultClassType;

            if (!string.IsNullOrEmpty(constDropValueAttribute.ConditionMemberName) && constDropValueAttribute.AlternateClassType != null)
            {
                var target = property.serializedObject.targetObject;
                var targetType = target.GetType();

                object conditionValue = null;

                // Check method
                var method = targetType.GetMethod(constDropValueAttribute.ConditionMemberName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (method != null && method.ReturnType == typeof(bool))
                {
                    conditionValue = method.Invoke(target, null);
                }
                else
                {
                    // Check property
                    var prop = targetType.GetProperty(constDropValueAttribute.ConditionMemberName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    if (prop != null && prop.PropertyType == typeof(bool))
                    {
                        conditionValue = prop.GetValue(target);
                    }
                    else
                    {
                        // Check field
                        var field = targetType.GetField(constDropValueAttribute.ConditionMemberName,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        if (field != null && field.FieldType == typeof(bool))
                        {
                            conditionValue = field.GetValue(target);
                        }
                    }
                }

                if (conditionValue is bool b && b)
                {
                    type = constDropValueAttribute.AlternateClassType;
                }
            }

            using var pooledObjects = DictionaryPool<string, object>.Get(out var dictionary);

            var fieldInfos = type.GetFields(CONST_BINDING_FLAGS);
            var propertyType = property.boxedValue?.GetType() ?? fieldInfos.FirstOrDefault()?.FieldType;

            dictionary.Add("<default>", default);
            foreach (var field in fieldInfos)
            {
                if (field.FieldType == propertyType)
                {
                    dictionary[field.Name] = field.GetValue(null);
                }
            }

            var valueList = dictionary.Values.ToList();
            var keyList = dictionary.Keys.ToList();

            var selectedIndex = valueList.IndexOf(property.boxedValue);
            if (selectedIndex < 0) selectedIndex = 0;

            position = EditorGUI.PrefixLabel(position, label);

            var buttonLabel = keyList[selectedIndex];
            if (EditorGUI.DropdownButton(position, new GUIContent(buttonLabel), FocusType.Keyboard))
            {
                SearchablePopup.Show(position, keyList, selectedIndex, i =>
                {
                    property.boxedValue = valueList[i];
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
        }


    }

    /// <summary>
    /// Searchable popup utility.
    /// </summary>
    public class SearchablePopup : PopupWindowContent
    {
        private string _search = "";
        private Vector2 _scroll;
        private readonly List<string> _items;
        private readonly System.Action<int> _onSelect;
        private int _currentIndex;

        public static void Show(Rect rect, List<string> items, int currentIndex, System.Action<int> onSelect)
        {
            PopupWindow.Show(rect, new SearchablePopup(items, currentIndex, onSelect));
        }

        private SearchablePopup(List<string> items, int currentIndex, System.Action<int> onSelect)
        {
            _items = items;
            _currentIndex = currentIndex;
            _onSelect = onSelect;
        }

        public override Vector2 GetWindowSize() => new Vector2(250, 300);

        public override void OnGUI(Rect rect)
        {
            // Search field with icon
            var searchRect = new Rect(5, 5, rect.width - 10, 20);

            var searchStyle = GUI.skin.FindStyle("ToolbarSeachTextField") ?? EditorStyles.textField;
            var icon = EditorGUIUtility.IconContent("Search Icon"); // hình kính lúp
            var iconRect = new Rect(searchRect.x + 2, searchRect.y + 2, 16, 16);
            GUI.Label(iconRect, icon);

            var textRect = new Rect(searchRect.x + 20, searchRect.y, searchRect.width - 22, searchRect.height);
            _search = EditorGUI.TextField(textRect, _search, searchStyle);

            // Filter items
            var filtered = string.IsNullOrEmpty(_search)
                ? _items
                : _items.Where(i => i.ToLower().Contains(_search.ToLower())).ToList();

            // Scrollable list
            var listRect = new Rect(0, 30, rect.width, rect.height - 30);
            _scroll = GUI.BeginScrollView(listRect, _scroll, new Rect(0, 0, rect.width - 20, filtered.Count * 20));

            for (int i = 0; i < filtered.Count; i++)
            {
                var itemRect = new Rect(5, i * 20, rect.width - 10, 20);
                if (GUI.Button(itemRect, filtered[i], (i == _currentIndex) ? EditorStyles.boldLabel : EditorStyles.label))
                {
                    var index = _items.IndexOf(filtered[i]);
                    _onSelect?.Invoke(index);
                    editorWindow.Close();
                }
            }

            GUI.EndScrollView();
        }

    }

}
