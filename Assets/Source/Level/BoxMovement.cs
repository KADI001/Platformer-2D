using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public class BoxMovement : RaycastCollision, IMoveable
    {
        private const float _maxWalkeableSurfaceAngel = 60;
        
        [SerializeField] private LayerMask _passengersMask;
        
        private Vector2 _velocity;
        private List<PassengersMovement> _passengersMovements = new List<PassengersMovement>();
        private Dictionary<Transform, IMoveable> _controllers = new Dictionary<Transform, IMoveable>();

        public bool OnGround => Info.Below;
        public Vector2 Velocity => _velocity;
        public CollisionInfo CollisionInfo => Info;


        private void FixedUpdate()
        {
            if (Info.Below || Info.DescendingExcessiveSlope) 
                _velocity.y = _velocity.y < 0 ? 0 : _velocity.y;

            if (Info.Above) 
                _velocity.y = _velocity.y > 0 ? 0 : _velocity.y;

            Move(_velocity * Time.deltaTime);
        }

        public void SetVelocity(Vector2 newVelocity)
        {
            _velocity = newVelocity;
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
            
            if (deltaPosition.x != 0)
            {
                HorizontalCollision(ref deltaPosition);
            }

            if (deltaPosition.y != 0)
            {
                VerticalCollision(ref deltaPosition);
            }

            //Handle(deltaPosition);

            //MovePassengers(true);
            transform.Translate(deltaPosition);
            CalculateRays();
            //MovePassengers(false);
        }

        private void MovePassengers(bool before)
        {
            foreach (var passenger in _passengersMovements)
            {
                if (!_controllers.ContainsKey(passenger.Transform))
                {
                    _controllers.Add(passenger.Transform, passenger.Transform.GetComponent<IMoveable>());
                }

                if (passenger.MoveBefore == before)
                {
                    IMoveable controller2D = _controllers[passenger.Transform];
                    controller2D.Move(passenger.DeltaPosition);
                }
            }
        }
        
         private void Handle(Vector2 deltaPosition)
        {
            HashSet<Transform> passengers = new HashSet<Transform>();
            _passengersMovements = new List<PassengersMovement>();
            float directionX = Mathf.Sign(deltaPosition.x);
            float directionY = Mathf.Sign(deltaPosition.y);

            if (deltaPosition.y != 0)
            {
                float rayDistance = Mathf.Abs(deltaPosition.y) + _shell;
                RayRange rayRange = directionY == -1 ? _bottom : _up;
                Physics2DEx.RaycastWithAction(rayRange, rayDistance, _passengersMask, _steps, hit =>
                {
                    if (!passengers.Contains(hit.transform))
                    {
                        passengers.Add(hit.transform);
                        Vector2 deltaPos = new Vector2((directionY == 1) ? deltaPosition.x : 0,
                            deltaPosition.y - (hit.distance - _shell) * directionY);
                        _passengersMovements.Add(new PassengersMovement(hit.transform, deltaPos, true, directionY == 1,
                            true, false));
                    }
                });
            }

            if (deltaPosition.x != 0)
            {
                float rayDistance = Mathf.Abs(deltaPosition.x) + _shell;
                RayRange rayRange = directionX == -1 ? _left : _right;
                Physics2DEx.RaycastWithAction(rayRange, rayDistance, _passengersMask, _steps, hit =>
                {
                    if (!passengers.Contains(hit.transform))
                    {
                        passengers.Add(hit.transform);
                        Vector2 deltaPos = new Vector2(deltaPosition.x - (hit.distance - _shell) * directionX,
                            deltaPosition.y);
                        _passengersMovements.Add(new PassengersMovement(hit.transform, deltaPos, true, false, false,
                            true));
                    }
                });
            }

            if (directionY == -1 || deltaPosition.y == 0 && deltaPosition.x != 0)
            {
                float rayDistance = 2 * _shell;
                Physics2DEx.RaycastWithAction(_up, rayDistance, _passengersMask, _steps, hit =>
                {
                    if (!passengers.Contains(hit.transform))
                    {
                        passengers.Add(hit.transform);
                        _passengersMovements.Add(new PassengersMovement(hit.transform, deltaPosition, false, true, true,
                            false));
                    }
                });
            }
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
            throw new System.NotImplementedException();
        }
    }
}
