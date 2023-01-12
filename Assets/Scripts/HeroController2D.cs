using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController2D : MonoBehaviour
{
    [Header("COLLISION")] 
    [SerializeField] private Bounds _collisionBounds;

    #region UnityMethods

    private void OnValidate()
    {
        _offsetShell = _offsetShell > 5 ? 5 : _offsetShell;
        _offsetShell = _offsetShell < 0.01f ? 0.01f : _offsetShell;
    }

    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        Gravity();
    }

    #endregion

    #region Old gravity

    /*
    private void Gravity2()
    {
        Vector2 position = (Vector2)transform.position + _boxCollider2D.offset;
        Vector2 velocity = _gravityDirection *  (_gravityAcceleration * Time.deltaTime);
        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 dir = deltaPosition.normalized;
        float distance = deltaPosition.magnitude + _offsetShell;
        Vector2 targetPosition = position + (dir * distance);

        int colCount = Physics2D.OverlapBox(targetPosition, _boxCollider2D.size, transform.rotation.eulerAngles.z, _groundLayer, _collidedObjects);

        for (int i = 0; i < colCount; i++)
        {
            var obj = _collidedObjects[i];
            
            if(obj == null)
                break;

            var objPosition = new Vector2(position.x, obj.transform.position.y);
            Vector2 dir2 = new Vector2(0, (position - (Vector2)objPosition).y).normalized;
            Vector2 offset = dir2 * (obj.transform.localScale.y * 0.5f);
            Vector2 surfacePosition = ((Vector2)objPosition + offset);
            Debug.DrawRay(surfacePosition, Vector3.right, Color.blue);
            print("Sur pos " + surfacePosition);
            Vector2 modifiedPosition = position + Vector2.down * (transform.localScale.y * 0.5f + _offsetShell);
            Debug.DrawRay(modifiedPosition, Vector3.right, Color.red);
            print("Mod pos " + modifiedPosition);
            
            if (surfacePosition.y > modifiedPosition.y)
            {
                dir = -dir;
            }
            
            float maxDistance = (surfacePosition - modifiedPosition).magnitude;
            print(maxDistance);
            distance = distance > maxDistance ? maxDistance : distance;
        }

        for (int i = 0; i < _collidedObjects.Length; i++)
        {
            _collidedObjects[i] = null;
        }
        
        if(distance > _minGravityMoveDistance)
            transform.position += (Vector3)dir * distance;
    }*/

    #endregion

    [Header("GRAVITY")]
    [SerializeField] [InspectorName("Direction")] private Vector2 _gravityDirection;
    [SerializeField] [InspectorName("Acceleration")] private float _gravityAcceleration;
    [SerializeField] private ContactFilter2D _groundLayer;
    [SerializeField] private float _offsetShell;
    [SerializeField] private float _slideSpeed;
    private float _sideOffset = 0.01f;
    private float _minGravityMoveDistance = 0.01f;

    private RayRange _down;
    private float _rayDistance = 5f;
    private int _numberSteps = 3;
    private List<RaycastHit2D> _hits = new List<RaycastHit2D>(3 * 3); //Number steps = 3
    private BoxCollider2D _boxCollider2D;
    private Collider2D[] _collidedObjects = new Collider2D[5];

    private void Gravity()
    {
        Vector2 position = transform.position + _collisionBounds.center;
        Vector2 velocity = _gravityDirection *  (_gravityAcceleration * Time.deltaTime);
        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 dir = deltaPosition.normalized;
        float distance = deltaPosition.magnitude;
        Vector2 targetPosition = position + (dir * distance);

        Vector2 offsetY = Vector2.up * _collisionBounds.extents.y;
        Vector2 offsetX = Vector2.right * _collisionBounds.extents.x;
        Vector2 sideOffset = Vector2.right * _sideOffset;
        _down = new RayRange(position - offsetY - offsetX + sideOffset, position - offsetY + offsetX - sideOffset, _gravityDirection);

        _hits.Clear();
        
        IEnumerable<Vector2> startPoints = EvaluateRayPositions(_down, _numberSteps);
        List<RaycastHit2D> tempHits = new List<RaycastHit2D>(_numberSteps);
        bool hasTarget = false;
        
        foreach (var point in startPoints)
        {
            int count = Physics2D.Raycast(point, _down.Direction, _groundLayer, tempHits, _rayDistance);
            foreach (var hit in tempHits)
            {
                _hits.Add(hit);
            }
        }

        Vector2 groundNormal = Vector2.zero;
        float maxDistance = float.MaxValue;

        foreach (var hit in _hits)
        {
            if (hit.distance - _offsetShell < maxDistance && hit.distance > 0)
            {
                hasTarget = true;
                maxDistance = hit.distance - _offsetShell;
                groundNormal = hit.normal;
            }
        }
        
        if (distance > maxDistance && hasTarget)
        {
            distance = maxDistance;

            if (distance > 0)
            {
                float projection = Vector2.Dot(dir, groundNormal);

                if (projection <= 0)
                {
                    dir = dir - projection * groundNormal;
                }

                distance = (dir * (_slideSpeed * Time.deltaTime)).magnitude;
            }
        }
        
       

        transform.position += (Vector3)dir * distance;
    }

    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range, int numberSteps) {
        for (var i = 0; i <= numberSteps; i++) {
            float t = (float)i / numberSteps;
            yield return Vector2.Lerp(range.StartPoint, range.EndPoint, t);
        }
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)
            return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + _collisionBounds.center, _collisionBounds.extents * 2f);
        
        Gizmos.color = Color.red;
        for (int i = 0; i <= 3; i++)
        {
            Vector2 origin = _down.GetRay(i / 3.0f).origin;
            Gizmos.DrawLine(origin, origin + _down.Direction);
        }
    }
}
