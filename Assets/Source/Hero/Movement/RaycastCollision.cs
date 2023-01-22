using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public class RaycastCollision : MonoBehaviour
    {
        [SerializeField] protected LayerMask _collisionMask;

        public CollisionInfo Info;
        protected Bounds _collisionBound;
        
        protected int _steps = 4;
        protected float _shell = 0.01f;
        protected RayRange _bottom, _up, _right, _left;
        private Vector2 _bottomLeft, _bottomRight, _topLeft, _topRight;

        public LayerMask Mask => _collisionMask;
        public Bounds CollisionBound => _collisionBound;
        public int Steps => _steps;

        public float Shell => _shell;

        public RayRange BottomRayRange => _bottom;

        public RayRange UpRayRange => _up;

        public RayRange RightRayRange => _right;

        public RayRange LeftRayRange => _left;

        private BoxCollider2D _collider;

        protected virtual void Start() => 
            _collider = GetComponent<BoxCollider2D>();

        protected void CalculateRays()
        {
            _collisionBound = _collider.bounds;

            _bottomLeft = new Vector2 (_collisionBound.min.x, _collisionBound.min.y);
            _bottomRight = new Vector2 (_collisionBound.max.x, _collisionBound.min.y);
            _topLeft = new Vector2 (_collisionBound.min.x, _collisionBound.max.y);
            _topRight = new Vector2 (_collisionBound.max.x, _collisionBound.max.y);

            _up = new RayRange(_topLeft, _topRight, Vector2.up);
            _bottom = new RayRange(_bottomLeft, _bottomRight, Vector2.down);
            _right = new RayRange(_bottomRight, _topRight, Vector2.right);
            _left = new RayRange(_bottomLeft, _topLeft, Vector2.left);
        }
        
        protected void UpdateCollisions(Vector2 deltaPosition)
        {
            Info.Below = false;
            Info.Above = false;
            Info.Left = false;
            Info.Right = false;

            float x = Mathf.Abs(deltaPosition.x) + (_shell + Constants.Epsilon);
            float y = Mathf.Abs(deltaPosition.y) + (_shell + Constants.Epsilon);

            Raycasts(x, y);
        }

        protected virtual void Raycasts(float disX, float disY)
        {
            Physics2DEx.RaycastWithAction(_bottom, disY, _collisionMask, _steps, (_) => Info.Below = true);
            Physics2DEx.RaycastWithAction(_up, disY, _collisionMask, _steps, (_) => Info.Above = true);
            Physics2DEx.RaycastWithAction(_left, disX, _collisionMask, _steps, (_) => Info.Left = true);
            Physics2DEx.RaycastWithAction(_right, disX, _collisionMask, _steps, (_) => Info.Right = true);
        }
    
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            float sphereRadius = 0.005f;
        
            Gizmos.DrawSphere(_bottomLeft, sphereRadius);
            Gizmos.DrawSphere(_bottomRight, sphereRadius);
            Gizmos.DrawSphere(_topLeft, sphereRadius);
            Gizmos.DrawSphere(_topRight, sphereRadius);
            
            Gizmos.color = Color.yellow;
            float sphereRadius2 = 0.01f;
        
            Gizmos.DrawSphere(_collisionBound.center, sphereRadius2);
        }

    }
}