using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Tweens
{
    public class TweenCreator : MonoBehaviour
    {
        public List<TweenEntry> Tweens = new();

        private void Start()
        {
            transform.DOMove(Vector3.up * 10, 4f);
        }
    }
}