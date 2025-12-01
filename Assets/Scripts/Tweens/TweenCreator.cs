using DG.Tweening;
using UnityEngine;

namespace Tweens
{
    public class TweenCreator : MonoBehaviour
    {
        private void Start()
        {
            transform.DOMove(Vector3.up * 10, 4f);
        }
    }
}