using UnityEngine;
using System.Collections;
using System.ComponentModel;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ScrollRectExtensions
    {
        public static void Clear(this ScrollRect scrollRect)
        {
            foreach (Transform child in scrollRect.content) {
                Object.Destroy(child.gameObject);
            }
        }

        public static void ScrollToTop(this ScrollRect scrollRect)
        {
            scrollRect.Scroll(1);
        }
        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            scrollRect.Scroll(0);
        }

        static void Scroll(this ScrollRect scrollRect, float target)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = target;
            Canvas.ForceUpdateCanvases();
        }
    }
}