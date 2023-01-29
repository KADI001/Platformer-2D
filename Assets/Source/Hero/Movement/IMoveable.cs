using UnityEngine;

namespace Source
{
    public interface IMoveable
    {
        bool OnGround { get; }
        Vector2 Velocity { get; }
        CollisionInfo CollisionInfo { get; }
        float Shell { get; }
        int Steps { get; }
        RayRange LeftRayRange { get; }
        RayRange RightRayRange { get; }
        RayRange BottomRayRange { get; }
        RayRange UpRayRange { get; }
        LayerMask Mask { get; }
        void SetVelocity(Vector2 newVelocity);
        void AddVelocity(Vector2 velocity);
        void Move(Vector2 deltaPosition);
    }
}
