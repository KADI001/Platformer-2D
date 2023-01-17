using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public class BoxMover : MonoBehaviour
    {
        [SerializeField] private LayerMask _boxMask;
        private Controller2D _controller;

        private void Awake()
        {
            _controller = GetComponent<Controller2D>();
        }

        private void FixedUpdate()
        {
            HashSet<Transform> passengers = new HashSet<Transform>();
            float distance = Mathf.Abs(_controller.Velocity.x * Time.deltaTime);
            float directionX = Mathf.Sign(_controller.Velocity.x);
            RayRange rayRange = directionX == 1 ? _controller.RightRayRange : _controller.LeftRayRange;
            Physics2DEx.RaycastWithAction(rayRange, distance, _boxMask, _controller.Steps, hit =>
            {
                if (!passengers.Contains(hit.transform))
                {
                    passengers.Add(hit.transform);
                    Controller2D controller2D = hit.transform.GetComponent<Controller2D>();
                    Vector2 deltaPos = new Vector2(distance - (hit.distance - _controller.Shell), 0) * directionX;
                    print(deltaPos);
                    controller2D.Move(deltaPos);
                }
            });
        }
    }
}