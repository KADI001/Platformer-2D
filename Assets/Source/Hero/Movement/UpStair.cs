using System;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(Gravity))]
    [RequireComponent(typeof(IMoveable))]
    public class UpStair : MonoBehaviour
    {
        private bool _isClimbing;
        private IMoveable _controller;
        private Gravity _gravity;
        private Jump _jump;

        public bool IsClimbing => _isClimbing;

        public bool isClimbing;

        private void Awake()
        {
            _controller = GetComponent<IMoveable>();
            _gravity = GetComponent<Gravity>();
            _jump = GetComponent<Jump>();
        }
        
        private void FixedUpdate()
        {
            _gravity.enabled = true;

            if (!Climb()) 
                _isClimbing = false;

            isClimbing = _isClimbing;
        }

        private bool Climb()
        {
            if (_controller.Velocity.x != 0)
            {
                float directionX = Mathf.Sign(_controller.Velocity.x);

                for (int i = 0; i <= _controller.Steps; i++)
                {
                    float progress = (float)i / _controller.Steps;
                    Ray ray = (directionX == -1)
                        ? _controller.LeftRayRange.GetRay(progress)
                        : _controller.RightRayRange.GetRay(progress);
                    RaycastHit2D hit = Physics2DEx.Raycast(ray, _controller.Shell, _controller.Mask);

                    if (hit)
                    {
                        if (hit.collider.TryGetComponent(out Stair stair))
                        {
                            if (stair.UpStarDirection != directionX)
                            {
                                _gravity.enabled = false;
                                Vector2 deltaPosition = Vector2.up * (stair.UpStaringSpeed * Time.deltaTime);
                                _controller.Move(deltaPosition);
                                _isClimbing = true;
                                if (_controller.Velocity.y < 0)
                                    _controller.SetVelocity(new Vector2(_controller.Velocity.x,
                                        Mathf.Clamp(_controller.Velocity.y, 0, int.MaxValue)));
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public void TurnOn()
        {
            _gravity.enabled = true;
            enabled = true;
        }

        public void TurnOff()
        {
            _gravity.enabled = true;
            enabled = false;
            _isClimbing = false;
        }
    }
}