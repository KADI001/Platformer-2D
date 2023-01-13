using System;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(Gravity))]
    public class UpStair : MonoBehaviour
    {
        private Controller2D _controller;
        private Gravity _gravity;
        private Jump _jump;

        private void Awake()
        {
            _controller = GetComponent<Controller2D>();
            _gravity = GetComponent<Gravity>();
            _jump = GetComponent<Jump>();
        }
        
        private void FixedUpdate()
        {
            _gravity.enabled = true;

            if (_controller.Velocity.x != 0)
            {
                float directionX = Mathf.Sign(_controller.Velocity.x);

                for (int i = 0; i <= _controller.Steps; i++)
                {
                    float progress = (float)i / _controller.Steps;
                    Ray ray = (directionX == -1) ? 
                          _controller.LeftRayRange.GetRay(progress)
                        : _controller.RightRayRange.GetRay(progress);
                    RaycastHit2D hit = Physics2DEx.Raycast(ray, _controller.Shell, _controller.Mask);
                    
                    Debug.DrawRay(ray.origin, ray.direction * _controller.Shell, Color.blue);
                    
                    if (hit)
                    {
                        if (hit.collider.TryGetComponent(out Stair stair))
                        {
                            if (stair.UpStarDirection != directionX)
                            {
                                _gravity.enabled = false;
                                Vector2 deltaPosition = Vector2.up * (stair.UpStaringSpeed * Time.deltaTime);
                                _controller.Move(deltaPosition);
                                _controller.Info.Below = true;
                                return;
                            }
                        }
                    }
                }
            }
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
        }
    }
}