using System;
using System.Collections.Generic;

namespace Tweens
{
    [Serializable]
    public class TweenEntry
    {
        public string MethodName;
        public List<TweenParameter> Parameters = new();
    }
}