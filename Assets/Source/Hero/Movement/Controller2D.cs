using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source
{
    public class Controller2D : RaycastCollision
    {
        [SerializeField] private float _maxWalkeableSurfaceAngel;
        [SerializeField] private float _maxDescendableSurfaceAngel;
        [SerializeField] private float _descendingSpeed;

        private Vector2 _velocity;
        private Vector2 _deltaPosition;
        private int _input;

        public bool OnGround => Info.Below;
        public bool Above => Info.Above;
        public bool Right => Info.Right;
        public bool Left => Info.Left;
        public bool DescendingExcessiveSlope => Info.DescendingExcessiveSlope;
        public Vector2 Velocity => _velocity;

        private void Awake()
        {
            _steps = 5;
        }

        private void FixedUpdate()
        {
            if (Info.Below || Info.DescendingExcessiveSlope)
            {
                _velocity.y = _velocity.y < 0 ? 0 : _velocity.y;
            }

            if (Info.Above)
            {
                _velocity.y = _velocity.y > 0 ? 0 : _velocity.y;
            }
            
            Move(_velocity * Time.deltaTime);
        }

        public void SetVelocity(Vector2 velocity)
        {
            _velocity = velocity;
        }

        public void AddVelocity(Vector2 velocity)
        {
            _velocity += velocity;
        }

        public void Move(Vector2 deltaPosition)
        {
            CalculateRays();
            UpdateCollisions(deltaPosition);

            Info.Reset();
            Info.OldDeltaPosition = deltaPosition;

            if (deltaPosition.y < 0)
            {
                DescendSlope(ref deltaPosition);
            }

            if (deltaPosition.x != 0)
            {
                HorizontalCollision(ref deltaPosition);
            }

            if (deltaPosition.y != 0)
            {
                VerticalCollision(ref deltaPosition);
            }

            transform.Translate(deltaPosition);
            CalculateRays();
        }

        private void UpdateCollisions(Vector2 deltaPosition)
        {
            Info.Below = false;
            Info.Above = false;
            Info.Left = false;
            Info.Right = false;

            float x = Mathf.Abs(deltaPosition.x) + _shell;
            float y = Mathf.Abs(deltaPosition.y) + _shell;

            Physics2DEx.RaycastWithAction(_bottom, y, _collisionMask, _steps, (_) => Info.Below = true);
            Physics2DEx.RaycastWithAction(_up, y, _collisionMask, _steps, (_) => Info.Above = true);
            Physics2DEx.RaycastWithAction(_left, x, _collisionMask, _steps, (_) => Info.Left = true);
            Physics2DEx.RaycastWithAction(_right, x, _collisionMask, _steps, (_) => Info.Right = true);
        }

        private void VerticalCollision(ref Vector2 deltaPosition)
        {
            float direction = Mathf.Sign(deltaPosition.y);
            float rayDistance = Mathf.Abs(deltaPosition.y) + _shell;

            for (int i = 0; i <= _steps; i++)
            {
                Ray ray = direction == -1 ? _bottom.GetRay((float)i / _steps) : _up.GetRay((float)i / _steps);
                ray.origin += Vector3.right * deltaPosition.x;
                RaycastHit2D hit = Physics2DEx.Raycast(ray, rayDistance, _collisionMask);

                if (hit)
                {
                    deltaPosition.y = (hit.distance - _shell) * direction;
                    rayDistance = hit.distance;

                    if (Info.ClimbingSlope)
                    {
                        deltaPosition.x = deltaPosition.y / Mathf.Tan(Info.SlopeAngel * Mathf.Deg2Rad) *
                                          Mathf.Sign(deltaPosition.x);
                    }
                }
            }

            if (Info.ClimbingSlope)
            {
                float directionX = Mathf.Sign(deltaPosition.x);
                rayDistance = Mathf.Abs(deltaPosition.x) + _shell;
                Vector2 rayOrigin = ((directionX == -1) ? _left.StartPoint : _right.StartPoint) +
                                    Vector2.up * deltaPosition.y;
                RaycastHit2D hit =
                    Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayDistance, _collisionMask);

                if (hit)
                {
                    float slopeAngel = Vector2.Angle(hit.normal, Vector2.up);

                    if (slopeAngel != Info.SlopeAngel)
                    {
                        deltaPosition.x = (hit.distance - _shell) * directionX;
                        Info.SlopeAngel = slopeAngel;
                    }
                }
            }
        }

        private void HorizontalCollision(ref Vector2 deltaPosition)
        {
            Vector2 oldDeltaPosition = deltaPosition;
            float direction = Mathf.Sign(deltaPosition.x);
            float rayDistance = Mathf.Abs(deltaPosition.x) + _shell;

            for (int i = 0; i <= _steps; i++)
            {
                Ray ray = direction == -1 ? _left.GetRay((float)i / _steps) : _right.GetRay((float)i / _steps);
                RaycastHit2D hit = Physics2DEx.Raycast(ray, rayDistance, _collisionMask);

                if (hit)
                {
                    if (hit.distance == 0)
                    {
                        continue;
                    }

                    float slopeAngel = Vector2.Angle(hit.normal, Vector2.up);

                    if (slopeAngel <= _maxWalkeableSurfaceAngel && i == 0)
                    {
                        if (Info.DescendingSlope)
                        {
                            Info.DescendingSlope = false;
                            deltaPosition = Info.OldDeltaPosition;
                        }

                        float startDistanceToSlope = 0;
                        if (slopeAngel != Info.OldSlopeAngel)
                        {
                            startDistanceToSlope = hit.distance - _shell;
                            deltaPosition.x -= startDistanceToSlope * direction;
                        }

                        ClimbSlope(ref deltaPosition, slopeAngel);

                        deltaPosition.x += startDistanceToSlope * direction;
                    }

                    if (!Info.ClimbingSlope || slopeAngel > _maxWalkeableSurfaceAngel)
                    {
                        deltaPosition.x = (hit.distance - _shell) * direction;
                        rayDistance = hit.distance;

                        if (Info.ClimbingSlope)
                        {
                            float climbY = Mathf.Tan(Info.SlopeAngel * Mathf.Deg2Rad) *
                                           Mathf.Abs(deltaPosition.x);

                            if (Info.Velocity.y <= 0)
                            {
                                deltaPosition.y = climbY;
                            }
                        }

                        if (Info.DescendingExcessiveSlope)
                        {
                            deltaPosition.x = oldDeltaPosition.x;
                            deltaPosition.y = oldDeltaPosition.y;
                        }
                    }
                }
            }

            if (Info.ClimbingSlope)
            {
                Vector2 origin = ((direction == -1) ? _left.StartPoint : _right.StartPoint) + deltaPosition;
                Vector2 dir = Vector2.right * direction;
                float rayLength = Mathf.Abs(deltaPosition.x) + _shell;

                RaycastHit2D hit2 = Physics2D.Raycast(origin, dir, rayLength);

                if (hit2)
                {
                    float slopeAngel = Vector2.Angle(hit2.normal, Vector2.up);

                    if (slopeAngel > _maxWalkeableSurfaceAngel)
                    {
                        deltaPosition.x = 0;
                        deltaPosition.y = oldDeltaPosition.y;
                    }
                }
            }
        }

        private void ClimbSlope(ref Vector2 deltaPosition, float slopeAngel)
        {
            float direction = Mathf.Sign(deltaPosition.x);
            float moveDistanceX = Mathf.Abs(deltaPosition.x);
            float climbDeltaPositionY = Mathf.Sin(slopeAngel * Mathf.Deg2Rad) * moveDistanceX;
            if (climbDeltaPositionY >= deltaPosition.y)
            {
                deltaPosition.y = climbDeltaPositionY;
            }

            deltaPosition.x = Mathf.Cos(slopeAngel * Mathf.Deg2Rad) * moveDistanceX * direction;
            Info.ClimbingSlope = true;
            Info.SlopeAngel = slopeAngel;
        }

        private void DescendSlope(ref Vector2 deltaPosition)
        {
            float directionX = Mathf.Sign(deltaPosition.x);
            Ray ray = (directionX == -1) ? _bottom.GetRay(1) : _bottom.GetRay(0);
            RaycastHit2D hit = Physics2DEx.Raycast(ray, Mathf.Infinity, _collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeAngle != 0)
                {
                    Ray ray2 = _right.GetRay(0);
                    float rayLength = _shell;
                    RaycastHit2D rHit2 = Physics2DEx.Raycast(ray2, rayLength, _collisionMask);
                    Ray ray3 = _left.GetRay(0);
                    RaycastHit2D lHit2 = Physics2DEx.Raycast(ray3, rayLength, _collisionMask);
                    RaycastHit2D hit2 = rHit2 ? rHit2 : lHit2;

                    if (hit2)
                    {
                        if (slopeAngle <= _maxDescendableSurfaceAngel &&
                            hit2.transform.Equals(hit.transform))
                        {
                            if (Mathf.Sign(hit.normal.x) == directionX)
                            {
                                if (hit.distance - _shell <=
                                    Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaPosition.x))
                                {
                                    float moveDistance = Mathf.Abs(deltaPosition.x);
                                    float descendVelocity = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                                    deltaPosition.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance *
                                                      Mathf.Sign(deltaPosition.x);
                                    deltaPosition.y -= descendVelocity;

                                    Info.SlopeAngel = slopeAngle;
                                    Info.DescendingSlope = true;
                                    Info.Below = true;
                                }
                            }
                        }
                        else
                        {
                            float slopeAngle2 = Vector2.Angle(hit2.normal, Vector2.up);

                            if (slopeAngle2 > _maxDescendableSurfaceAngel)
                            {
                                if (deltaPosition.x == 0 || (Mathf.Sign(hit2.normal.x) != Mathf.Sign(deltaPosition.x)))
                                {
                                    float moveDistance = _descendingSpeed * Time.deltaTime;
                                    float descendVelocity = Mathf.Sin(slopeAngle2 * Mathf.Deg2Rad) * moveDistance;
                                    deltaPosition.x = Mathf.Cos(slopeAngle2 * Mathf.Deg2Rad) * moveDistance *
                                                      Mathf.Sign(hit.normal.x);
                                    deltaPosition.y -= descendVelocity;

                                    Info.DescendingExcessiveSlope = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}