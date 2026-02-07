using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using UnityEngine;

namespace Tweens
{
    public class TweenCreator : MonoBehaviour
    {
        public List<TweenEntry> Tweens = new();

        private void Start()
        {
            var allAvailableMethods = InitializeMethodList();

            foreach (var entry in Tweens)
            {
                var method = allAvailableMethods.FirstOrDefault(m =>
                    m.Name == entry.MethodName &&
                    m.GetParameters().Length == entry.Parameters.Count);

                if (method == null)
                {
                    Debug.LogError($"Failed to find method for tween entry: {entry.MethodName} with {entry.Parameters.Count} parameters.");
                    continue;
                }

                var args = entry.Parameters.Select(p => p.GetValue()).ToArray();

                if (method.Invoke(null, args) is Tween tween)
                {
                    if (entry.OverrideEase)
                    {
                        tween.SetEase(entry.Ease);
                    }
                }
            }
        }

        private static List<MethodInfo> InitializeMethodList()
        {
            var shortCutExtensions = typeof(ShortcutExtensions);
            var methods = shortCutExtensions.GetMethods(BindingFlags.Public | BindingFlags.Static).ToList();
            return methods.OrderBy(m => m.Name).ToList();
        }
    }
}