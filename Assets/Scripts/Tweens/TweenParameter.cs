using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tweens
{
    [Serializable]
    public class TweenParameter
    {
        public enum ParamType
        {
            Int,
            Float,
            Bool,
            String,
            Vector2,
            Vector3,
            Color,
            Object
        }

        public ParamType Type;

        public int IntValue;
        public float FloatValue;
        public bool BoolValue;
        public string StringValue;
        public Vector2 Vector2Value;
        public Vector3 Vector3Value;
        public Color ColorValue;
        public Object ObjectValue;

        public object GetValue()
        {
            return Type switch
            {
                ParamType.Int => IntValue,
                ParamType.Float => FloatValue,
                ParamType.Bool => BoolValue,
                ParamType.String => StringValue,
                ParamType.Vector2 => Vector2Value,
                ParamType.Vector3 => Vector3Value,
                ParamType.Color => ColorValue,
                ParamType.Object => ObjectValue,
                _ => null
            };
        }

        public void SetValue(object value)
        {
            switch (value)
            {
                case int v:
                    Type = ParamType.Int;
                    IntValue = v;
                    break;
                case float v:
                    Type = ParamType.Float;
                    FloatValue = v;
                    break;
                case bool v:
                    Type = ParamType.Bool;
                    BoolValue = v;
                    break;
                case string v:
                    Type = ParamType.String;
                    StringValue = v;
                    break;
                case Vector2 v:
                    Type = ParamType.Vector2;
                    Vector2Value = v;
                    break;
                case Vector3 v:
                    Type = ParamType.Vector3;
                    Vector3Value = v;
                    break;
                case Color v:
                    Type = ParamType.Color;
                    ColorValue = v;
                    break;
                case Object v:
                    Type = ParamType.Object;
                    ObjectValue = v;
                    break;
                default:
                    // If the incoming value is boxed (e.g. System.Single boxed to float), attempt conversions
                    Debug.LogError("Failed to convert type");
                    if (value is IConvertible ic)
                    {
                        try
                        {
                            var f = Convert.ToSingle(ic);
                            Type = ParamType.Float;
                            FloatValue = f;
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    break;
            }
        }
    }
}