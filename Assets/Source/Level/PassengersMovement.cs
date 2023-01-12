using UnityEngine;

namespace Game
{
    public struct PassengersMovement
    {
        public Transform Transform;
        public Vector2 DeltaPosition;
        public bool MoveBefore;
        public bool IsGrounded;
        public bool AboveOrBelow;
        public bool LeftOrRight;

        public PassengersMovement(Transform transform, Vector2 deltaPosition, bool moveBefore, bool isGrounded, bool aboveOrBelow, bool leftOrRight)
        {
            Transform = transform;
            DeltaPosition = deltaPosition;
            MoveBefore = moveBefore;
            IsGrounded = isGrounded;
            AboveOrBelow = aboveOrBelow;
            LeftOrRight = leftOrRight;
        }
    }
}