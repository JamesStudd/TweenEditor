using UnityEditor;
using UnityEngine;

namespace Tweens.Attributes
{
    [CustomPropertyDrawer(typeof(ShowIfParamTypeAttribute))]
    public sealed class ShowIfParamTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var a = (ShowIfParamTypeAttribute)attribute;

            if (!ShouldShow(property, a))
            {
                return;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var a = (ShowIfParamTypeAttribute)attribute;

            return ShouldShow(property, a) ? EditorGUI.GetPropertyHeight(property, label, true) : 0f;
        }

        private static bool ShouldShow(SerializedProperty property, ShowIfParamTypeAttribute a)
        {
            var typeProp = FindSibling(property, a.TypeFieldName);

            if (typeProp == null)
            {
                return true;
            }

            if (typeProp.propertyType != SerializedPropertyType.Enum)
            {
                return true;
            }

            return typeProp.enumValueIndex == a.TypeValue;
        }

        private static SerializedProperty FindSibling(SerializedProperty property, string siblingName)
        {
            var path = property.propertyPath;
            var dot = path.LastIndexOf('.');

            if (dot < 0)
            {
                return property.serializedObject.FindProperty(siblingName);
            }

            var parentPath = path[..dot];
            return property.serializedObject.FindProperty(parentPath + "." + siblingName);
        }
    }
}