using UnityEngine;

namespace Source
{
    public class StandAloneKeyboardInput : MonoBehaviour, IKeyboardInput
    {
        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";
        
        public Vector2 Axis => new(Input.GetAxis(Horizontal), Input.GetAxis(Vertical));
        public bool JumpPressed => Input.GetKeyDown(KeyCode.Space);
        public bool FirePressed => Input.GetKeyDown(KeyCode.F);
        public bool ClimbStairPressed => Input.GetKeyDown(KeyCode.W);
        public bool RightMovePressed => Input.GetKeyDown(KeyCode.D);
        public bool LeftMovePressed => Input.GetKeyDown(KeyCode.A);
    }
}