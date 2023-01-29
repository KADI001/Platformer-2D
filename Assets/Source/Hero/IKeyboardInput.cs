using UnityEngine;

namespace Source
{
    public interface IKeyboardInput
    {
        public Vector2 Axis { get; }
        public bool JumpPressed { get; }
        public bool FirePressed { get; }
        public bool ClimbStairPressed { get; }
        public bool RightMovePressed { get; }
        public bool LeftMovePressed { get; }
    }
}
