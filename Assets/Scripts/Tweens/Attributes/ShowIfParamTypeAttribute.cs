using System;
using UnityEngine;

namespace Tweens.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ShowIfParamTypeAttribute : PropertyAttribute
    {
        public readonly int TypeValue;
        public readonly string TypeFieldName;

        public ShowIfParamTypeAttribute(int typeValue, string typeFieldName = "Type")
        {
            TypeValue = typeValue;
            TypeFieldName = typeFieldName;
        }
    }
}