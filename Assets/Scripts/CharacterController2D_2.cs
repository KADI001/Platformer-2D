using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D_2 : MonoBehaviour
{
    [SerializeField] private float _speedX;
    [SerializeField] private float _jumpForce;
    [SerializeField] private LayerMask _groundLayer;

    private Vector2 _velocity;
    private Vector2 _gravityVelocity;
    private float _inputX;
    private Vector2 _groundNormal;

    private ContactFilter2D _contactFilter2D;
    private Rigidbody2D _rigidbody2D;

    private List<RaycastHit2D> _hitsBuffer;
    private RaycastHit2D[] _tempHitsBuffer;
    private bool _onGround;
    private float _shell = 0.01f;
    private float _minGroundNormal = 0.35f;
    private float _minMoveDistance = 0.001f;

    #region UnityMethods

    private void Awake()
    {
        _contactFilter2D = new ContactFilter2D();
        _contactFilter2D.SetLayerMask(_groundLayer);
        _contactFilter2D.useLayerMask = true;

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _tempHitsBuffer = new RaycastHit2D[16];
        _hitsBuffer = new List<RaycastHit2D>(16);
    }

    private bool temp = false;

    private void FixedUpdate()
    {
        if (temp)
        {
            _inputX = 1;
        }
        
        Gravity(Vector2.down);

        _velocity.x = _speedX * _inputX;
        _gravityVelocity.y += (Physics2D.gravity * Time.deltaTime).y;

        Vector2 xMove = new Vector2(_groundNormal.y, -_groundNormal.x);
        xMove *= _velocity.x;
        Move(xMove, false);
        
        Vector2 yMove = new Vector2(0, _gravityVelocity.y);
        Move(yMove, true);
    }

    private void Update()
    {
        _inputX = GetMoveDirection();
        if (Input.GetKeyDown(KeyCode.F))
        {
            temp = !temp;
        }
        if (Input.GetKeyDown(KeyCode.Space) && _onGround)
        {
            _gravityVelocity.y = _jumpForce;
        }
    }

    #endregion

    private void Move(Vector2 velocity, bool yMovement)
    {
        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 direction = deltaPosition.normalized;
        float distance = deltaPosition.magnitude;

        if (_minMoveDistance < distance)
        {
            int collisionCount = _rigidbody2D.Cast(direction, _contactFilter2D, _tempHitsBuffer, distance + _shell);

            _hitsBuffer.Clear();

            for (int i = 0; i < collisionCount; i++)
            {
                _hitsBuffer.Add(_tempHitsBuffer[i]);
            }
        
            for (int i = 0; i < _hitsBuffer.Count; i++)
            {
                var hit = _hitsBuffer[i];
                Vector2 normal = hit.normal;
                float dis = hit.distance - _shell;

                if (normal.y > _minGroundNormal)
                {
                    if (yMovement)
                    {
                        normal.x = 0;
                    }
                }

                if (yMovement == false)
                {
                    float projection = Vector2.Dot(_velocity, normal);
                    if (projection < 0)
                    {
                        _velocity = _velocity - projection * normal;
                    }
                }
                else
                {
                    float projection = Vector2.Dot(_gravityVelocity, normal);
                    if (projection < 0)
                    {
                        _gravityVelocity = _gravityVelocity - projection * normal;
                    }
                }
                

                distance = distance > dis ? dis : distance;
            }
        }

        _rigidbody2D.position += direction * distance;
    }
    
    private void Gravity(Vector2 direction)
    {
        _onGround = false;
        _groundNormal = Vector2.up;

        int collisionCount = _rigidbody2D.Cast(direction, _contactFilter2D, _tempHitsBuffer, _shell);

        _hitsBuffer.Clear();

        for (int i = 0; i < collisionCount; i++)
        {
            _hitsBuffer.Add(_tempHitsBuffer[i]);
        }
        
        for (int i = 0; i < _hitsBuffer.Count; i++)
        {
            var hit = _hitsBuffer[i];
            Vector2 normal = hit.normal;
            
            if (normal.y > _minGroundNormal)
            {
                _groundNormal = normal;
                _onGround = true;
            }
        }
    }
    
    /*private float _sideOffset = 0.01f;
    private RayRange _down;
    private float _rayDistance = Mathf.Infinity;
    private int _numberSteps = 3;
    private RaycastHit2D[] _tempHits = new RaycastHit2D[3 * 3];
    private List<RaycastHit2D> _hits = new List<RaycastHit2D>(3 * 3); //Number steps = 3
    
    private void CheckGround()
    {
        Vector2 position = transform.position;
        Vector2 offsetY = Vector2.up * (transform.localScale.y * 0.5f);
        Vector2 offsetX = Vector2.right * (transform.localScale.x * 0.5f);
        Vector2 sideOffset = Vector2.right * _sideOffset;
        _down = new RayRange(position - offsetY - offsetX + sideOffset, position - offsetY + offsetX - sideOffset, Vector2.down, _rayDistance);

        _hits.Clear();
        
        IEnumerable<Vector2> startPoints = EvaluateRayPositions(_down, _numberSteps);
        List<RaycastHit2D> tempHits = new List<RaycastHit2D>(_numberSteps);

        foreach (var point in startPoints)
        {
            int count = Physics2D.Raycast(point, _down.Direction, _contactFilter2D, tempHits, _down.Distance);
            Debug.DrawRay(point, _down.Direction, Color.green);
            foreach (var hit in tempHits)
            {
                _hits.Add(hit);
            }
        }

        Vector2 groundNormal = Vector2.zero;
        float maxDistance = float.MaxValue;
        RaycastHit2D closestHit = new RaycastHit2D();

        foreach (var hit in _hits)
        {
            if (hit.distance - _shell < maxDistance)
            {
                maxDistance = hit.distance - _shell;
                closestHit = hit;
            }
        }
        
        Debug.DrawRay(closestHit.point, Vector3.right, Color.magenta);
        groundNormal = closestHit.normal;

        if (maxDistance <= _shell && groundNormal.y > _minGroundNormal)
        {
            _onGround = true;
            _groundNormal = groundNormal;
        }
        else
        {
            _onGround = false;
            _groundNormal = Vector2.zero;
        }
    }
    
    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range, int numberSteps) {
        for (var i = 0; i <= numberSteps; i++) {
            float t = (float)i / numberSteps;
            yield return Vector2.Lerp(range.StartPoint, range.EndPoint, t);
        }
    }*/

    private int GetMoveDirection()
    {
        if (Input.GetKey(KeyCode.D))
            return 1;

        if (Input.GetKey(KeyCode.A))
            return -1;

        return 0;
    }
}
