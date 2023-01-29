using UnityEngine;

namespace Source
{
    public class HeroAnimator : MonoBehaviour
    {
        public static readonly int IsStanding = Animator.StringToHash("IsStanding");
        public static readonly int IsJumping = Animator.StringToHash("IsJumping");
        public static readonly int OnGround = Animator.StringToHash("OnGround");
        public static readonly int State = Animator.StringToHash("State");

        public readonly int IdleState = Animator.StringToHash("Idle");
        public readonly int RunState = Animator.StringToHash("Run");
        public readonly int JumpState = Animator.StringToHash("Jump");

        private Animator _animator;
        private Controller2D _controller;
        private UpStair _upStair;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _controller = GetComponent<Controller2D>();
            _upStair = GetComponent<UpStair>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            {
                _animator.SetInteger(State, (int)PlayerState.Run);
            }
            else
            {
                if (_controller.OnGround)
                    _animator.SetInteger(State, (int)PlayerState.Idle);
            }

            if (!_controller.OnGround && !_upStair.isClimbing)
                _animator.SetInteger(State, (int)PlayerState.Jump);

            Flip();
        }

        private void Flip()
        {
            // Vector2 newRotation = transform.rotation.eulerAngles;
            // newRotation.y = _controller.Velocity.x == 0 ? newRotation.y : Mathf.Sign(_controller.Velocity.x) == 1 ? 0 : 180;
            // transform.rotation = Quaternion.Euler(newRotation);
            
            _spriteRenderer.flipX = _controller.Velocity.x == 0
                ? _spriteRenderer.flipX
                : Mathf.Sign(_controller.Velocity.x) == 1;
        }
    }
}