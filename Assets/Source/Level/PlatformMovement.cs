using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class PlatformMovement : RaycastCollision
{
    [SerializeField] private float _time;
    [SerializeField] private float _timeToStayAtPoint;
    [SerializeField] private bool _ringWay;
    [SerializeField] [Range(0, 2)] private float _easeScale;
    [SerializeField] private LayerMask _passengersMask;
    [SerializeField] private Vector2[] _localWaypoints;
    
    private Vector2[] _globalWaypoints;
    private List<PassengersMovement> _passengersMovements;
    private Dictionary<Transform, Controller2D> _controllers;

    private float _timeToMove;
    private int _fromWaypointIndex;
    private bool _stucked;
    private Vector2 _deltaPosition;
    private float _speed;
    private float _progress;
    private float _totalWayLength;
    
    protected override void Start()
    {
        base.Start();

        _controllers = new Dictionary<Transform, Controller2D>();
        _globalWaypoints = new Vector2[_localWaypoints.Length];
        
        for (int i = 0; i < _globalWaypoints.Length; i++)
        {
            _globalWaypoints[i] = (Vector2)transform.position + _localWaypoints[i];
        }

        for (int i = 0; i < _globalWaypoints.Length; i++)
        {
            if (i < _globalWaypoints.Length - 1)
            {
                _totalWayLength += Vector2.Distance(_globalWaypoints[i], _globalWaypoints[i + 1]);
            }   
        }
        
        _timeToMove = Time.time + _timeToStayAtPoint;
    }

    private void FixedUpdate()
    {
        _speed = _totalWayLength / _time;

        CalculateRays();
        _stucked = false;
        _deltaPosition = CalculatePlatformDeltaPosition();
        Handle(_deltaPosition);
        
        MovePassengers(true);

        if (!_stucked)
        {
            transform.Translate(_deltaPosition);
        }
        
        MovePassengers(false);
    }

    private Vector2 CalculatePlatformDeltaPosition()
    {
        if (Time.time < _timeToMove)
        {
            return Vector2.zero;
        }
        
        int toWaypointIndex = (_fromWaypointIndex + 1) % _localWaypoints.Length;
        float distanceBetweenWaypoints =
            Vector2.Distance(_globalWaypoints[_fromWaypointIndex], _globalWaypoints[toWaypointIndex]);
        _progress += Time.deltaTime * _speed / distanceBetweenWaypoints;
        _progress = Mathf.Clamp01(_progress);
        float easedProgress = Ease(_progress);
        
        Vector2 deltaPosition = Vector2.Lerp(_globalWaypoints[_fromWaypointIndex],
            _globalWaypoints[toWaypointIndex], easedProgress);

        if (_progress >= 1)
        {
            _progress = 0;
            _fromWaypointIndex++;


            if (!_ringWay)
            {
                if (_fromWaypointIndex >= _localWaypoints.Length - 1)
                {
                    _fromWaypointIndex = 0;
                    Array.Reverse(_globalWaypoints);
                }   
            }

            _timeToMove = Time.time + _timeToStayAtPoint;
        }
        
        return deltaPosition - (Vector2) transform.position;
    }

    private float Ease(float progress)
    {
        float a = _easeScale + 1;
        return Mathf.Pow(progress, a) / (Mathf.Pow(progress, a) + Mathf.Pow(1 - progress, a));
    }

    private void MovePassengers(bool before)
    {
        foreach (var passenger in _passengersMovements)
        {
            int stuckVertical = 0;
            int stuckHorizontal = 0;
            
            if (!_controllers.ContainsKey(passenger.Transform))
            {
                _controllers.Add(passenger.Transform, passenger.Transform.GetComponent<Controller2D>());
            }

            if (passenger.MoveBefore == before)
            {
                Controller2D controller2D = _controllers[passenger.Transform];
                controller2D.Move(passenger.DeltaPosition, out Vector2 deltaPosition, passenger.IsGrounded);

                if (passenger.AboveOrBelow)
                {
                    if (Mathf.Sign(_deltaPosition.y) == -1 && controller2D.OnGround ||
                        Mathf.Sign(_deltaPosition.y) == 1 && controller2D.Above)
                    {
                        stuckVertical++;

                        if (stuckVertical == 1)
                        {
                            transform.Translate(deltaPosition);
                        }
                    }
                }
                
                if (passenger.LeftOrRight)
                {
                    if (Mathf.Sign(_deltaPosition.x) == -1 && controller2D.Left ||
                        Mathf.Sign(_deltaPosition.x) == 1 && controller2D.Right)
                    {
                        stuckHorizontal++;

                        if (stuckHorizontal == 1)
                        {
                            transform.Translate(deltaPosition);
                        }
                    }
                }

                if (stuckHorizontal > 0 || stuckVertical > 0)
                {
                    _stucked = true;
                }
            }
        }
    }

    public void Handle(Vector2 deltaPosition)
    {
        HashSet<Transform> passengers = new HashSet<Transform>();
        _passengersMovements = new List<PassengersMovement>();
        float directionX = Mathf.Sign(deltaPosition.x);
        float directionY = Mathf.Sign(deltaPosition.y);

        if (deltaPosition.y != 0)
        {
            float rayDistance = Mathf.Abs(deltaPosition.y) + _shell;

            for (int i = 0; i <= _steps; i++)
            {
                float progress = (float)i / _steps;
                Ray ray = directionY == -1 ? _bottom.GetRay(progress) : _up.GetRay(progress);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, rayDistance, _passengersMask);

                if (hit)
                {
                    if (!passengers.Contains(hit.transform))
                    {
                        passengers.Add(hit.transform);
                        float passengerX = (directionY == 1) ? deltaPosition.x : 0;
                        float passengerY = deltaPosition.y - (hit.distance - _shell) * directionY;
                        _passengersMovements.Add(new PassengersMovement(hit.transform, new Vector2(passengerX, passengerY), true, directionY == 1, true, false));
                    }
                }
            }
        }

        if (deltaPosition.x != 0)
        {
            float rayDistance = Mathf.Abs(deltaPosition.x) + _shell;
            for (int i = 0; i <= _steps; i++)
            {
                float progress = (float)i / _steps;
                Ray ray = directionX == -1 ? _left.GetRay(progress) : _right.GetRay(progress);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, rayDistance, _passengersMask);
                
                if (hit)
                {
                    if (!passengers.Contains(hit.transform))
                    {
                        passengers.Add(hit.transform);
                        float passengerX = deltaPosition.x - (hit.distance - _shell) * directionX;
                        float passengerY = 0;
                        _passengersMovements.Add(new PassengersMovement(hit.transform, new Vector2(passengerX, passengerY), true, false, false, true));
                    }
                }
            }
        }
        
        if (directionY == -1 || deltaPosition.y == 0 && deltaPosition.x != 0)
        {
            float rayDistance = 2 * _shell;
            for (int i = 0; i <= _steps; i++)
            {
                float progress = (float)i / _steps;
                Ray ray = _up.GetRay(progress);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, rayDistance, _passengersMask);

                if (hit)
                {
                    if (!passengers.Contains(hit.transform))
                    {
                        passengers.Add(hit.transform);
                        float passengerX = deltaPosition.x;
                        float passengerY = deltaPosition.y;
                        _passengersMovements.Add(new PassengersMovement(hit.transform, new Vector2(passengerX, passengerY), false, true, true, false));
                    }
                }
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        if(_localWaypoints == null)
            return;
        
        Gizmos.color = Color.red;

        for (int i = 0; i < _localWaypoints.Length; i++)
        {
            Vector2 origin = (Application.isPlaying) ? _globalWaypoints[i] : (Vector2)transform.position + _localWaypoints[i];
            float sphereRadius = 0.1f;
            
            Gizmos.DrawSphere(origin, sphereRadius);
        }
    }
}
