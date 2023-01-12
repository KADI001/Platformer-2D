using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController4 : MonoBehaviour
{
    [SerializeField] private Bounds _bounds;
    [SerializeField] private ContactFilter2D _contactFilter;
    [SerializeField] private float _HSpeed;
    private RayRange _up, _right, _down, _left;

    private bool _onGround;
    private bool _collided;
    [SerializeField] [Min(0)] private float _minCollisionDistance = 0.1f;
    private int _countCollisionIterations = 10;
    private float _offset = 0.01f;
    private float _rayDistance = 0.1f;
    private int _steps = 3;
    private bool _collisionRight, _collisionLeft, _collisionUp;
    private Vector2 _velocity;
    private float _horizontalSpeed;
    private float _verticalSpeed;

    private void Update()
    {
        CalculateRays();

        _onGround = DetectCollision(_down);
        _collisionRight = DetectCollision(_right);
        _collisionLeft = DetectCollision(_left);
        _collisionUp = DetectCollision(_up);

        _horizontalSpeed = Input.GetAxis("Horizontal") * _HSpeed;

        if (_onGround == false)
        {
            _verticalSpeed += Physics2D.gravity.y * Time.deltaTime;
        }
        else
        {
            _verticalSpeed = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _verticalSpeed = 5f;
        }
        
        print(Time.time);

        Move();
    }

    private void Move()
    {
        Vector2 position = transform.position + _bounds.center;
        Vector2 velocity = new Vector2(_horizontalSpeed, _verticalSpeed);
        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 targetPosition = position + deltaPosition;
        
        var collided = Physics2D.OverlapBox(targetPosition, _bounds.size, 0, _contactFilter.layerMask.value);
        if (collided == false)
        {
            transform.position += (Vector3)deltaPosition;
            return;
        }

        Vector2 positionToMove = transform.position;
        
        for (int i = 1; i < _countCollisionIterations; i++)
        {
            float t = (float)i / _countCollisionIterations;
            Vector2 nextPoint = Vector2.Lerp(position, targetPosition, t);
            
            Debug.DrawRay(nextPoint, Vector3.up);

            if (Physics2D.OverlapBox(nextPoint, _bounds.size, 0, _contactFilter.layerMask.value))
            {
                transform.position = positionToMove;
                return;
            }

            positionToMove = nextPoint;
        }
    }

    private void CalculateRays()
    {
        Vector2 position = transform.position + _bounds.center;
        Vector2 scale = _bounds.extents * 2f;
        float width = scale.x;
        float height = scale.y;

        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        Vector2 halfWidthVector = Vector2.right * halfWidth;
        Vector2 halfHeightVector = Vector2.up * halfHeight;

        Vector2 upStart = position + halfHeightVector ;
        Vector2 rightStart = position + halfWidthVector ;
        Vector2 downStart = position + (-halfHeightVector);
        Vector2 leftStart = position + (-halfWidthVector) ;

        _up = new RayRange(upStart + (-halfWidthVector) + (Vector2.right * _offset),
            upStart + halfWidthVector + (Vector2.left * _offset), Vector2.up);
        _right = new RayRange(rightStart + halfHeightVector + (Vector2.down * _offset),
            rightStart + (-halfHeightVector) + (Vector2.up * _offset), Vector2.right);
        _down = new RayRange(downStart + (-halfWidthVector) + (Vector2.right * _offset),
            downStart + halfWidthVector + (Vector2.left * _offset), Vector2.down);
        _left = new RayRange(leftStart + halfHeightVector + (Vector2.down * _offset),
            leftStart + (-halfHeightVector) + (Vector2.up * _offset), Vector2.left);
    }

    private bool DetectCollision(RayRange range)
    {
        return EvaluateRayPositions(range, _steps).Any(point => Physics2D.Raycast(point, range.Direction, _rayDistance, _contactFilter.layerMask));
    }
    
    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range, int steps) {
        for (var i = 0; i <= steps; i++) {
            float t = (float)i / steps;
            Debug.DrawRay(Vector2.Lerp(range.StartPoint, range.EndPoint, t), range.Direction * _rayDistance);
            yield return Vector2.Lerp(range.StartPoint, range.EndPoint, t);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + _bounds.center, _bounds.extents * 2f);
        
        if (_up == null)
        {
            return;
        }
        
        Gizmos.color = Color.red;

        for (int i = 0; i <= 3; i++)
        {
            Vector2 origin = _up.GetRay(i / 3.0f).origin;
            Gizmos.DrawLine(origin, origin + _up.Direction);
        }
        
        for (int i = 0; i <= 3; i++)
        {
            Vector2 origin = _right.GetRay(i / 3.0f).origin;
            Gizmos.DrawLine(origin, origin + _right.Direction);
        }
        
        for (int i = 0; i <= 3; i++)
        {
            Vector2 origin = _down.GetRay(i / 3.0f).origin;
            Gizmos.DrawLine(origin, origin + _down.Direction);
        }
        
        for (int i = 0; i <= 3; i++)
        {
            Vector2 origin = _left.GetRay(i / 3.0f).origin;
            Gizmos.DrawLine(origin, origin + _left.Direction);
        }


    }
}