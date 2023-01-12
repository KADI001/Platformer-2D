using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Unity.VisualScripting;
using UnityEngine;

public class Stair : RaycastCollision
{
    [SerializeField] private float _upStairSpeed;
    [SerializeField] private int _upStairDirection;
    [SerializeField] private LayerMask _passengerMask;
    
    private Dictionary<Player, Controller2D> _controllers;
    public float UpStaringSpeed => _upStairSpeed;
    public float UpStarDirection => _upStairDirection;

    private void OnValidate()
    {
        _upStairDirection = (int)Mathf.Clamp(_upStairDirection, -1, 1);
        _upStairDirection = _upStairDirection == 0 ? 1 : _upStairDirection;
    }

    private void FixedUpdate()
    {
        CalculateRays();
    }

    protected override void Start()
    {
        base.Start();

        _controllers = new Dictionary<Player, Controller2D>();
    }

    // private void Update()
    // {
    //     CalculateRays();
    //     
    //     for (int i = 0; i < _steps; i++)
    //     {
    //         float progress = (float)i / _steps;
    //         Ray ray = _upStairDirection == -1 ? _left.GetRay(progress) : _right.GetRay(progress);
    //         float rayLength = _shell;
    //         RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, rayLength, _passengerMask);
    //
    //         if (hit)
    //         {
    //             Vector2 deltaPosition = Vector2.zero;
    //             if (hit.collider.TryGetComponent(out Player player))
    //             {
    //                 Controller2D controller;
    //                 
    //                 if (!_controllers.ContainsKey(player))
    //                 {
    //                     controller = player.GetComponent<Controller2D>();
    //                     _controllers.Add(player, controller);
    //                 }
    //                 else
    //                 {
    //                     controller = _controllers[player];
    //                 }
    //                 
    //                 if (player.InputX != 0 && Mathf.Sign(player.InputX) != _upStairDirection)
    //                 {
    //                     deltaPosition.y = _upStairSpeed * Time.deltaTime;
    //                     controller.Move(deltaPosition, true);
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             foreach (var players in _controllers.Keys)
    //             {
    //                 players.SetIsUpStaring(false);
    //             }
    //         }
    //     }
    // }
}
