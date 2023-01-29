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
    private Walk _walk;

    private void Awake()
    {
        _controller = GetComponent<IMoveable>();
        _upStair = GetComponent<UpStair>();
        _walk = GetComponent<Walk>();
    }

    private void Update()
    {
        _walk?.SwitchOn();

        if (_upStair.isClimbing && Input.GetKeyDown(KeyCode.Space))
        {
            print("Work!");
            _controller.SetVelocity(Vector2.left * _force);
            _walk?.SwitchOff();
        }
    }
}
