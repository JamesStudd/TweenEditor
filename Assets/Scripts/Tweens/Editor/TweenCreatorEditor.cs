using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Tweens
{
    [CustomEditor(typeof(TweenCreator))]
    public class TweenCreatorEditor : Editor
    {
        private List<Object> _currentParameterValues;
        private string[] _methodNames;
        private List<MethodInfo> _methods;

        private int _selectedMethodIndex;
        private string[] MethodNames => _methodNames ??= InitializeMethodNames();
        private List<MethodInfo> Methods => _methods ??= InitializeMethodList();

        private void OnEnable()
        {
            InitializeTweenMethods();
            InitializeNewParameterValues();
        }

        private string[] InitializeMethodNames()
        {
            return Methods.Select(GetMethodDisplayName).ToArray();
        }

        private static List<MethodInfo> InitializeMethodList()
        {
            var shortCutExtensions = typeof(ShortcutExtensions);
            var methods = shortCutExtensions.GetMethods(BindingFlags.Public | BindingFlags.Static).ToList();
            return methods.OrderBy(e => e.Name).ToList();
        }

        private static string GetMethodDisplayName(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            var extraInfo = parameters.Length > 0
                ? $"{parameters[0].ParameterType}"
                : string.Empty;
            return $"{methodInfo.Name} ({extraInfo})";
        }

        private void InitializeTweenMethods()
        {
            var shortCutExtensions = typeof(ShortcutExtensions);
            var methods = shortCutExtensions.GetMethods(BindingFlags.Public | BindingFlags.Static).ToList();
            _methods = methods;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var newIndex = EditorGUILayout.Popup(_selectedMethodIndex, MethodNames);

            if (newIndex != _selectedMethodIndex)
            {
                _selectedMethodIndex = newIndex;
                InitializeNewParameterValues();
            }

            DrawCurrentMethodInfo();
        }

        private void InitializeNewParameterValues()
        {
            var method = Methods[_selectedMethodIndex];
            var parameters = method.GetParameters();
            _currentParameterValues = parameters.Select(e => e.ParameterType.Default()).ToList();
        }

        private void DrawCurrentMethodInfo()
        {
            var method = Methods[_selectedMethodIndex];
            var parameters = method.GetParameters();

            for (var index = 0; index < parameters.Length; index++)
            {
                var parameterInfo = parameters[index];
                var parameterValue = _currentParameterValues[index];

                parameterValue = DrawParameterField(parameterInfo, parameterValue);
                _currentParameterValues[index] = parameterValue;
            }
        }

        private Object DrawParameterField(ParameterInfo parameter, Object parameterValue)
        {
            return TryDrawInbuiltType(parameter.ParameterType, parameterValue, parameter.Name);
        }

        private static Object TryDrawInbuiltType(Type t, object value, string label)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Int32:
                    return EditorGUILayout.IntField(label, (int)value);
                case TypeCode.Single:
                    return EditorGUILayout.FloatField(label, (float)value);
                case TypeCode.Boolean:
                    return EditorGUILayout.Toggle(label, (bool)value);
                case TypeCode.String:
                    return EditorGUILayout.TextField(label, (string)value);
            }

            return DrawObjectField(t, value, label);
        }

        private static Object DrawObjectField(Type t, Object value, string label)
        {
            if (t == typeof(Vector3))
            {
                return EditorGUILayout.Vector3Field(label, (Vector3)value);
            }

            if (t == typeof(Vector2))
            {
                return EditorGUILayout.Vector2Field(label, (Vector2)value);
            }

            if (t == typeof(Color))
            {
                return EditorGUILayout.ColorField(label, (Color)value);
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(t))
            {
                var obj = value as UnityEngine.Object;
                return EditorGUILayout.ObjectField(label, obj, t, true);
            }

            EditorGUILayout.LabelField(label, $"Unsupported: {t.Name}");
            return value;
        }
    }
}