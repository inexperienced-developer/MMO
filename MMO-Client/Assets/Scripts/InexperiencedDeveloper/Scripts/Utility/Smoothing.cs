using System;
using System.Collections.Generic;

namespace InexperiencedDeveloper.Utils
{
    public static class Smoothing
    {
        private static float[] smoothingKernels = new float[] { 0.4f, 0.3f, 0.2f, 0.1f };

        public static float SmoothValue(List<float> list, float val)
        {
            return SmoothValue(list, val, smoothingKernels);
        }

        public static float SmoothValue(List<float> list, float val, float[] smoothingKernels)
        {
            list.Insert(0, val);
            if (list.Count > smoothingKernels.Length)
            {
                list.RemoveAt(list.Count - 1);
            }
            float smoothList = 0f;
            float totalSmoothVal = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                float smoothingKernel = smoothingKernels[i];
                smoothList += smoothingKernel * list[i];
                totalSmoothVal += smoothingKernel;
            }

            return smoothList / totalSmoothVal;
        }
    }
}