using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    
public class Controller2D : RaycastCollision
{
    [SerializeField] private float _maxWalkeableSurfaceAngel;
    [SerializeField] private float _maxDescendableSurfaceAngel;
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _descendingSpeed;

    private int _input;
    private ContactFilter2D _contactFilter2D;
    
    public bool OnGround => _collisionInfo.Below;
    public bool Above => _collisionInfo.Above;
    public bool Right => _collisionInfo.Right;
    public bool Left => _collisionInfo.Left;
    public bool DescendingExcessiveSlope => _collisionInfo.DescendingExcessiveSlope;

    private void Awake()
    {
        _contactFilter2D = new ContactFilter2D();
        _contactFilter2D.SetLayerMask(_collisionMask);
        _contactFilter2D.useLayerMask = true;
    }

    private void Update()
    {
        CalculateRays();
    }

    public void Move(Vector2 deltaPosition, out Vector2 result, bool isGrounded = false)
    {
        CalculateRays();
        
        _collisionInfo.Reset();
        _collisionInfo.OldDeltaPosition = deltaPosition;
        
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

        UpStair(ref deltaPosition);
        
        transform.Translate(deltaPosition);
        
        if (isGrounded)
        {
            _collisionInfo.Below = true;
        }
        
        result = deltaPosition;
    }

    public void Move(Vector2 deltaPosition, bool isGrounded = false)
    {
        Move(deltaPosition, out Vector2 result, isGrounded);
    }

    public void UpStair(ref Vector2 deltaPosition)
    {
        if (deltaPosition.x != 0)
        {
            float directionX = Mathf.Sign(deltaPosition.x);

            for (int i = 0; i <= _steps; i++)
            {
                Ray ray = (directionX == -1) ? _left.GetRay((float)i / _steps) : _right.GetRay((float)i / _steps);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _shell, _collisionMask);

                if (hit)
                {
                    if (hit.collider.TryGetComponent(out Stair stair))
                    {
                        if (stair.UpStarDirection != directionX)
                        {
                            deltaPosition.y = stair.UpStaringSpeed * Time.deltaTime;
                            _collisionInfo.Below = true;
                        }
                    }
                }
            }
        }
    }
    
    #region Collision
    
    private CollisionInfo _collisionInfo;

    private void VerticalCollision(ref Vector2 deltaPosition)
    {
        float direction = Mathf.Sign(deltaPosition.y);
        float rayDistance = Mathf.Abs(deltaPosition.y) + _shell;

        for (int i = 0; i <= _steps; i++)
        {
            Ray ray = direction == -1 ? _bottom.GetRay((float)i / _steps) : _up.GetRay((float)i / _steps);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin + Vector3.right * deltaPosition.x, ray.direction, rayDistance, _collisionMask);
            
            if (hit)
            {
                deltaPosition.y = (hit.distance - _shell) * direction;
                rayDistance = hit.distance;

                if (_collisionInfo.ClimbingSlope)
                {
                    deltaPosition.x = deltaPosition.y / Mathf.Tan(_collisionInfo.SlopeAngel * Mathf.Deg2Rad) * Mathf.Sign(deltaPosition.x);
                }

                _collisionInfo.Above = direction == 1;
                _collisionInfo.Below = direction == -1 && !_collisionInfo.DescendingExcessiveSlope;
                
                Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.blue);
            }
        }

        if (_collisionInfo.ClimbingSlope)
        {
            float directionX = Mathf.Sign(deltaPosition.x);
            rayDistance = Mathf.Abs(deltaPosition.x) + _shell;
            Vector2 rayOrigin = ((directionX == -1) ? _left.StartPoint : _right.StartPoint) + Vector2.up * deltaPosition.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayDistance, _collisionMask);

            if (hit)
            {
                float slopeAngel = Vector2.Angle(hit.normal, Vector2.up);
                
                if (slopeAngel != _collisionInfo.SlopeAngel)
                {
                    deltaPosition.x = (hit.distance - _shell) * directionX;
                    _collisionInfo.SlopeAngel = slopeAngel;
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
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, rayDistance, _collisionMask);
            
            if (hit)
            {
                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngel = Vector2.Angle(hit.normal, Vector2.up); 
                
                if (slopeAngel <= _maxWalkeableSurfaceAngel && i == 0)
                {
                    if (_collisionInfo.DescendingSlope)
                    {
                        _collisionInfo.DescendingSlope = false;
                        deltaPosition = _collisionInfo.OldDeltaPosition;
                    }
                    
                    float startDistanceToSlope = 0;
                    if (slopeAngel != _collisionInfo.OldSlopeAngel)
                    {
                        startDistanceToSlope = hit.distance - _shell;
                        deltaPosition.x -= startDistanceToSlope * direction;
                    }
                    
                    ClimbSlope(ref deltaPosition, slopeAngel);
                    
                    deltaPosition.x += startDistanceToSlope * direction;
                }
                
                if (!_collisionInfo.ClimbingSlope || slopeAngel > _maxWalkeableSurfaceAngel)
                {
                    deltaPosition.x = (hit.distance - _shell) * direction;
                    rayDistance = hit.distance;

                    if(_collisionInfo.ClimbingSlope)
                    {
                        float climbY = Mathf.Tan(_collisionInfo.SlopeAngel * Mathf.Deg2Rad) *
                                       Mathf.Abs(deltaPosition.x);

                        if (_collisionInfo.Velocity.y <= 0)
                        {
                            deltaPosition.y = climbY;
                        }
                    }

                    if (_collisionInfo.DescendingExcessiveSlope)
                    {
                        deltaPosition.x = oldDeltaPosition.x;
                        deltaPosition.y = oldDeltaPosition.y;
                    }

                    _collisionInfo.Left = direction == -1;
                    _collisionInfo.Right = direction == 1;   
                }
            }
        }

        if (_collisionInfo.ClimbingSlope)
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
        _collisionInfo.Below = true;
        _collisionInfo.ClimbingSlope = true;
        _collisionInfo.SlopeAngel = slopeAngel;
    }
    
    private void DescendSlope(ref Vector2 deltaPosition)
    {
        float directionX = Mathf.Sign(deltaPosition.x);
        Ray ray = (directionX == -1) ? _bottom.GetRay(1) : _bottom.GetRay(0) ;
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, _collisionMask);
        
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeAngle != 0)
            {
                Ray ray2 = _right.GetRay(0);
                float rayLength = _shell;
                RaycastHit2D rHit2 = Physics2D.Raycast(ray2.origin, ray2.direction, rayLength, _collisionMask);
                Ray ray3 = _left.GetRay(0);
                RaycastHit2D lHit2 = Physics2D.Raycast(ray3.origin, ray3.direction, rayLength, _collisionMask);
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

                                _collisionInfo.SlopeAngel = slopeAngle;
                                _collisionInfo.DescendingSlope = true;
                                _collisionInfo.Below = true;
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

                                _collisionInfo.DescendingExcessiveSlope = true;
                                _collisionInfo.Below = false;
                            }
                        }
                    }
                }
            }
        }
    }

    public void CheckOnGround()
    {
        for (int i = 0; i <= _steps; i++)
        {
            Ray ray = _bottom.GetRay((float)i / _steps);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _shell * 2, _collisionMask);

            if (hit)
            {
                float slopeAngel = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngel <= _maxWalkeableSurfaceAngel)
                {
                    _collisionInfo.Below = true;
                    return;
                }
            }
        }

        _collisionInfo.Below = false;
    }

    #endregion
}

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

    public void Reset()
    {
        Above = Below = Left = Right = false;

        ClimbingSlope = false;
        DescendingSlope = false;
        DescendingExcessiveSlope = false;

        OldSlopeAngel = SlopeAngel;
        SlopeAngel = 0;
        Velocity = Vector2.zero;
    }
}

}