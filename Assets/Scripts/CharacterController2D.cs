using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    private const float MaxSurfaceAngle = 60f;
    public const float ShellRadius = 0.01f;
    private const float MinMoveDistance = 0.001f;

    [SerializeField] private float _jumpForce;
    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private Vector2 _velocity;
    private ContactFilter2D _collisionFilter;
    private Rigidbody2D _rigidbody2D;
    [SerializeField] private Vector2 _velocityY;
    [SerializeField] private Vector2 _velocityX;
    private Vector2 _deltaY;
    private Vector2 _deltaX;
    [SerializeField] private bool _isGrounded;
    private Vector2 _groundNormal;
    private float _surfaceYNormal;
    private RaycastHit2D[] _hits = new RaycastHit2D[16];
    private List<RaycastHit2D> _hitsList = new List<RaycastHit2D>(16);

    private float YAcceleration => -Physics2D.gravity.y;
    public Vector2 GroundNormal => _groundNormal;
    public float SpeedY => _velocityY.y;
    public float SpeedX => _velocityX.x;
    public bool IsGrounded => _isGrounded;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _surfaceYNormal = (float)Math.Cos(Mathf.Deg2Rad * MaxSurfaceAngle);
        _collisionFilter = new ContactFilter2D();
        _collisionFilter.SetLayerMask(_groundMask);
        _collisionFilter.useTriggers = false;
        _collisionFilter.useLayerMask = true;
    }

    private void FixedUpdate()
    {
        CheckGround();

        Vector2 direction = Vector2.right * GetMoveDirection();
        direction = direction - Vector2.Dot(direction, GroundNormal) * GroundNormal;
        _velocity.x = (direction * _speed).x;
        _deltaX = Vector2.right * (_velocity.x * Time.fixedDeltaTime);

        Move(ref _velocity, _deltaX, false);

        _isGrounded = false;

        _velocity.y += (Physics2D.gravity.normalized * (YAcceleration * Time.fixedDeltaTime)).y;
        _deltaY = Vector2.up * (_velocity.y * Time.fixedDeltaTime);

        Move(ref _velocity, _deltaY,true);

        if (_isGrounded == false)
            _groundNormal = Vector2.up;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && _isGrounded)
            _velocityY.y = _jumpForce;
    }

    private void Move(ref Vector2 velocity, Vector2 delta, bool yMovement)
    {
        float distance = delta.magnitude;

        if (distance > MinMoveDistance)
        {
            int collisionCount = _rigidbody2D.Cast(delta.normalized, _collisionFilter, _hits, distance + ShellRadius);
            bool hasCollision = collisionCount > 0;

            if (hasCollision && IsJumping() && yMovement)
            {
                ResetVelocity();
                return;
            }
            
            FillHitsBuffer(collisionCount);

            for (int index = 0; index < _hitsList.Count; index++)
            {
                RaycastHit2D current = _hitsList[index];
                Vector2 normal = current.normal;

                if (yMovement)
                {
                    if (normal.y > _surfaceYNormal)
                    {
                        _isGrounded = true;
                        _groundNormal = normal;
                        normal.x = 0;
                    }
                }

                UpdateVelocity(normal, ref velocity);
                UpdateDistance(current, ref distance);
            }
        }

        _rigidbody2D.position += delta.normalized * distance;
    }

    private static void UpdateDistance(RaycastHit2D current, ref float distance)
    {
        float minDistance = current.distance - ShellRadius;
        distance = minDistance < distance ? minDistance : distance;
    }

    private void UpdateVelocity(Vector2 normal, ref Vector2 velocity)
    {
        float projection = Vector2.Dot(velocity, normal);

        if (projection < 0)
            velocity = velocity - projection * normal;
    }

    private void FillHitsBuffer(int collisionCount)
    {
        _hitsList.Clear();

        for (int i = 0; i < collisionCount; i++)
        {
            _hitsList.Add(_hits[i]);
        }
    }

    private int GetMoveDirection()
    {
        if (Input.GetKey(KeyCode.D))
            return 1;

        if (Input.GetKey(KeyCode.A))
            return -1;

        return 0;
    }

    private void ResetVelocity()
    {
        _velocityY = Vector2.zero;
    }

    private bool IsJumping()
    {
        return _velocityY.y > 0;
    }

    private bool CheckGround()
    {
        _isGrounded = false;

        Vector2 center = transform.position;
        DrawPoint(center, Color.black);

        Vector2 bottomPoint = transform.position - Vector3.up * transform.localScale.y * 0.5f;
        DrawPoint(bottomPoint, Color.green);

        Vector2 rightBottomPoint = bottomPoint + Vector2.right * transform.localScale.x * 0.5f;
        Vector2 leftBottomPoint = bottomPoint - Vector2.right * transform.localScale.x * 0.5f;
        DrawPoint(bottomPoint, Color.blue);

        float radius = (leftBottomPoint - center).magnitude;
        Ray ray1 = new Ray(bottomPoint - (Vector2)transform.up * 0.01f, -transform.up);
        Ray ray2 = new Ray(leftBottomPoint - (Vector2)transform.up * 0.01f, -transform.up);
        Ray ray3 = new Ray(rightBottomPoint - (Vector2)transform.up * 0.01f, -transform.up);
        RaycastHit2D[] hits = new RaycastHit2D[1];

        Debug.DrawRay(ray1.origin, ray1.direction, Color.red);
        Debug.DrawRay(ray2.origin, ray2.direction, Color.red);
        Debug.DrawRay(ray3.origin, ray3.direction, Color.red);

        if (Physics2D.Raycast(ray2.origin, ray2.direction, _collisionFilter,  hits, transform.localScale.y * 5) > 5)
        {
            RaycastHit2D hit = hits[0];
            if (hit.distance <= 0)
            {
                _isGrounded = true;
                _groundNormal = hit.normal;
            }

            return _isGrounded;
        }

        if (Physics2D.Raycast(ray3.origin, ray3.direction, _collisionFilter, hits, transform.localScale.y * 5) > 5)
        {
            RaycastHit2D hit = hits[0];
            if (hit.distance <= 0)
            {
                _isGrounded = true;
                _groundNormal = hit.normal;
            }

            return _isGrounded;
        }

        if (Physics2D.Raycast(ray1.origin, ray1.direction, _collisionFilter, hits, transform.localScale.y * 5) > 0)
        {
            RaycastHit2D hit = hits[0];
            float normalAngel = Vector2.Angle(hit.normal, Vector2.up);
            Vector2 point = (center - leftBottomPoint).normalized;
            float normalAngel2 = Vector2.Angle(hit.normal, point);
            float temp3 = (bottomPoint - center).magnitude;
            print(normalAngel2);

            DrawPoint(hit.point, Color.yellow);

            if (normalAngel < MaxSurfaceAngle)
            {
                float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngel) - temp3 + 0.02f;
                Debug.DrawRay(bottomPoint + Vector2.right * 0.01f, Vector3.down * maxDist, Color.gray);

                if (hit.distance < maxDist)
                {
                    _isGrounded = true;
                    _groundNormal = hit.normal;
                }
            }

            Debug.DrawRay(leftBottomPoint, point);
        }

        return _isGrounded;
    }

    private static void DrawPoint(Vector2 point, Color color)
    {
        Debug.DrawRay(point, Vector3.right * 0.1f, color);
        Debug.DrawRay(point, Vector3.left * 0.1f, color);
        Debug.DrawRay(point, Vector3.down * 0.1f, color);
        Debug.DrawRay(point, Vector3.up * 0.1f, color);
    }
}
