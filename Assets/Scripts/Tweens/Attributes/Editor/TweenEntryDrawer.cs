using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace Tweens.Attributes
{
    [CustomPropertyDrawer(typeof(TweenEntry))]
    public sealed class TweenEntryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var methodNameProp = property.FindPropertyRelative("MethodName");
            //var parametersProp = property.FindPropertyRelative("Parameters");
            var overrideEaseProp = property.FindPropertyRelative("OverrideEase");
            var easeProp = property.FindPropertyRelative("Ease");

            var easeText = overrideEaseProp.boolValue
                ? ((Ease)easeProp.enumValueIndex).ToString()
                : "Default";

            label.text = string.IsNullOrEmpty(methodNameProp.stringValue)
                ? $"Tween ({easeText})"
                : $"{methodNameProp.stringValue} ({easeText})";

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}