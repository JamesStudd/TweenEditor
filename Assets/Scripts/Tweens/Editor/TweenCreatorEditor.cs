using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.DOTweenEditor;
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
        private List<(string Name, Object Value)> _currentParameterValues;

        private string[] _methodNames;
        private List<MethodInfo> _methods;

        private int _selectedMethodIndex;
        private string[] MethodNames => _methodNames ??= InitializeMethodNames();
        private List<MethodInfo> Methods => _methods ??= InitializeMethodList();

        private bool _overrideEase;
        private Ease _ease = Ease.Linear;
        
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
            
            _overrideEase = EditorGUILayout.Toggle("Override Ease", _overrideEase);

            using (new EditorGUI.DisabledScope(!_overrideEase))
            {
                _ease = (Ease)EditorGUILayout.EnumPopup("Ease", _ease);
            }

            if (GUILayout.Button("Add"))
            {
                AddCurrentTween();
            }

            if (GUILayout.Button("Preview"))
            {
                PreviewTweens();
            }

            if (GUILayout.Button("Stop"))
            {
                DOTweenEditorPreview.Stop(true);
            }
        }

        private void AddCurrentTween()
        {
            var tweenCreator = (TweenCreator)target;
            var method = Methods[_selectedMethodIndex];

            var entry = new TweenEntry
            {
                MethodName = method.Name,
                Parameters = _currentParameterValues
                    .Select(o =>
                    {
                        var p = new TweenParameter();
                        p.SetValue(o.Value);
                        p.SetName(o.Name);
                        return p;
                    })
                    .ToList(),
                OverrideEase = _overrideEase,
                Ease = _ease
            };

            Undo.RecordObject(tweenCreator, "Add Tween Entry");
            tweenCreator.Tweens.Add(entry);
            EditorUtility.SetDirty(tweenCreator);
        }

        private void PreviewTweens()
        {
            var tweenCreator = (TweenCreator)target;

            var seq = DOTween.Sequence();

            foreach (var entry in tweenCreator.Tweens)
            {
                var method = FindShortcutMethod(entry.MethodName, entry.Parameters.Count);
                if (method == null)
                {
                    continue;
                }

                var args = entry.Parameters.Select(p => p.GetValue()).ToArray();

                if (method.Invoke(null, args) is Tween tween)
                {
                    if (entry.OverrideEase)
                    {
                        tween.SetEase(entry.Ease);
                    }

                    seq.Append(tween);
                }
            }

            seq.AppendCallback(() => DOTweenEditorPreview.Stop(true));

            DOTweenEditorPreview.PrepareTweenForPreview(seq);
            DOTweenEditorPreview.Start();
        }


        private MethodInfo FindShortcutMethod(string methodName, int paramCount)
        {
            return Methods.FirstOrDefault(m =>
                m.Name == methodName &&
                m.GetParameters().Length == paramCount);
        }

        private void InitializeNewParameterValues()
        {
            var method = Methods[_selectedMethodIndex];
            var parameters = method.GetParameters();
            _currentParameterValues = parameters.Select(e => (Name: e.Name, Value: e.ParameterType.Default())).ToList();
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

        private (string Name, Object Value) DrawParameterField(ParameterInfo parameter, (string Name, Object parameterValue) parameterValue)
        {
            return (parameterValue.Name, TryDrawInbuiltType(parameter.ParameterType, parameterValue.parameterValue, parameter.Name));
        }

        private static Object TryDrawInbuiltType(Type type, Object value, string label)
        {
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Int32 => EditorGUILayout.IntField(label, (int)value),
                TypeCode.Single => EditorGUILayout.FloatField(label, (float)value),
                TypeCode.Boolean => EditorGUILayout.Toggle(label, (bool)value),
                TypeCode.String => EditorGUILayout.TextField(label, (string)value),
                _ => DrawObjectField(type, value, label)
            };
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