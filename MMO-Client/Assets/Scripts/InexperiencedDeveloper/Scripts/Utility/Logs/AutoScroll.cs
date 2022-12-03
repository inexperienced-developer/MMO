using UnityEngine;
using UnityEngine.UI;

namespace InexperiencedDeveloper.Utils
{
    public class AutoScroll
    {

        public static void ScrollInstant(ScrollRect scroll, float endPos, bool vertical = true)
        {
            if (vertical)
            {
                scroll.verticalNormalizedPosition = endPos; 
                scroll.horizontalNormalizedPosition = endPos;
            }
            else
            {
                scroll.horizontalNormalizedPosition = endPos;
            }
        }
    }
}

