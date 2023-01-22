using UnityEngine;

namespace Source
{
    public struct CollisionInfo
    {
        public bool Above, Below;
        public bool Right, Left;

        public bool ClimbingSlope;
        public float SlopeAngel, OldSlopeAngel;
        public bool DescendingSlope;
        public bool DescendingExcessiveSlope;
        public Vector2 OldDeltaPosition;
        public Vector2 Velocity;
        public bool ClimbingStair;

        public void Reset()
        {
            ClimbingSlope = false;
            DescendingSlope = false;
            DescendingExcessiveSlope = false;

            OldSlopeAngel = SlopeAngel;
            SlopeAngel = 0;
            Velocity = Vector2.zero;
        }
    }
}