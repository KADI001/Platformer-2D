using System;
using UnityEngine;

namespace Source
{
    public static class Physics2DEx
    {
        public static RaycastHit2D Raycast(Ray ray, float distance, LayerMask mask)
        {
            return Physics2D.Raycast(ray.origin, ray.direction, distance, mask);
        }

        public static void RaycastWithAction(RayRange rayRange, float distance, LayerMask mask, int steps, Action<RaycastHit2D> onHit)
        {
            for (int i = 0; i <= steps; i++)
            {
                float progress = (float)i / steps;
                Ray ray = rayRange.GetRay(progress);
                RaycastHit2D hit = Raycast(ray, distance, mask);

                if (hit)
                {
                    onHit?.Invoke(hit);
                }
            }
        }
    }

    public static class ObjectEx
    {
        public static T With<T>(this T imp, Action<T> act) where T : notnull
        {
            act?.Invoke(imp);
            return imp;
        }
    }
}