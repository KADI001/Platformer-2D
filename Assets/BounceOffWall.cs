using System;
using System.Collections;
using System.Collections.Generic;
using Source;
using UnityEngine;

[RequireComponent(typeof(IMoveable))]
[RequireComponent(typeof(UpStair))]
public class BounceOffWall : MonoBehaviour
{
    [SerializeField] private float _force;
    
    private IMoveable _controller;
    private UpStair _upStair;
    private HorizontalMove _horizontalMove;

    private void Awake()
    {
        _controller = GetComponent<IMoveable>();
        _upStair = GetComponent<UpStair>();
        _horizontalMove = GetComponent<HorizontalMove>();
    }

    private void Update()
    {
        _horizontalMove?.SwitchOn();

        if (_upStair.isClimbing && Input.GetKeyDown(KeyCode.Space))
        {
            print("Work!");
            _controller.SetVelocity(Vector2.left * _force);
            _horizontalMove?.SwitchOff();
        }
    }
}
