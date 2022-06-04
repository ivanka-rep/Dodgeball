using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Utilities
{
    public static class TweenAnimation
    {
        public static void PlayAnimation(Transform targetTransform, AnimationType type, float duration, Action onComplete = null)
        {

            switch (type)
            {
                case AnimationType.OpeningTransition:
                    targetTransform.DOScale(1f, duration).From(0f)
                        .onComplete = () => onComplete?.Invoke(); break;
                case AnimationType.ClosingTransition:
                    targetTransform.DOScale(0f, duration).From(1f)
                        .onComplete = () => onComplete?.Invoke(); break;
                case AnimationType.Shake:
                    targetTransform.DOShakePosition(duration, 3f, 30, 90f, true)
                        .onComplete = () => onComplete?.Invoke(); break;
            }
        }
        
    }
    
    [Serializable] public enum AnimationType
    {
        OpeningTransition,
        ClosingTransition,
        Shake,
    }
}