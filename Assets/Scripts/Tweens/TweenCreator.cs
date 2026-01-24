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
            
            foreach (var tween in Tweens)
            {
                var foundMethod = allAvailableMethods.FirstOrDefault(e => e.Name == tween.MethodName && e.GetParameters().Length == tween.Parameters.Count);

                if (foundMethod != null)
                {
                    var parameters = tween.Parameters.Select(e => e.GetValue()).ToArray();
                    foundMethod.Invoke(this, parameters);
                }
            }
        }
        
        private static List<MethodInfo> InitializeMethodList()
        {
            var shortCutExtensions = typeof(ShortcutExtensions);
            var methods = shortCutExtensions.GetMethods(BindingFlags.Public | BindingFlags.Static).ToList();
            return methods.OrderBy(e => e.Name).ToList();
        }
    }
}