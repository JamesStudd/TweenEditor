using System;
using System.Collections.Generic;
using DG.Tweening;

namespace Tweens
{
    [Serializable]
    public class TweenEntry
    {
        public string MethodName;
        public List<TweenParameter> Parameters = new();

        public bool OverrideEase;
        public Ease Ease = Ease.OutQuad;
    }
}