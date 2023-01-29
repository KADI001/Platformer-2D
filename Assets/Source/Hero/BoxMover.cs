using System;
using System.Collections;
using System.Collections.Generic;
using Source;
using UnityEngine;

public class BoxMover : MonoBehaviour
{
    [SerializeField] private LayerMask _boxMask;
    
    private IMoveable _controller;
    private IMoveable _box;

    private float Steps => _controller.Steps;
    private float Shell => _controller.Shell;
    private RayRange Bottom => _controller.BottomRayRange;
    private RayRange Up => _controller.UpRayRange;
    private RayRange Left => _controller.LeftRayRange;
    private RayRange Right => _controller.RightRayRange;
    

    private void Update()
    {
        Vector2 deltaPosition = _controller.Velocity * Time.deltaTime;
        HorizontalMove(ref deltaPosition);
        VerticalMove(ref deltaPosition);

        _box?.Move(deltaPosition);
    }

    private void HorizontalMove(ref Vector2 deltaPosition)
    {
        float directionX = Mathf.Sign(deltaPosition.x);
        float rayDistance = Mathf.Abs(deltaPosition.x) + Shell;

        for (int i = 0; i <= Steps; i++)
        {
            Ray ray = directionX == -1 ? Left.GetRay((float)i / Steps) : Right.GetRay((float)i / Steps);
            RaycastHit2D hit = Physics2DEx.Raycast(ray, rayDistance, _boxMask);

            if (hit)
            {
                deltaPosition.x = (hit.distance - Shell) * directionX;
                rayDistance = hit.distance;
                
                _box = hit.transform.GetComponent<IMoveable>();
            }
        }
    }

    private void VerticalMove(ref Vector2 deltaPosition)
    {
        float directionY = Mathf.Sign(deltaPosition.y);
        float rayDistance = Mathf.Abs(deltaPosition.y) + Shell;
        
        for (int i = 0; i <= Steps; i++)
        {
            Ray ray = directionY == -1 ? Bottom.GetRay((float)i / Steps) : Up.GetRay((float)i / Steps);
            ray.origin += Vector3.right * deltaPosition.x;
            RaycastHit2D hit = Physics2DEx.Raycast(ray, rayDistance, _boxMask);

            if (hit)
            {
                deltaPosition.y = (hit.distance - Shell) * directionY;
                rayDistance = hit.distance;
                
                _box = hit.transform.GetComponent<IMoveable>();
            }
        }
    }
}
