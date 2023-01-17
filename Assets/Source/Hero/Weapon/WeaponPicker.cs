using System;
using System.Collections;
using System.Collections.Generic;
using Source;
using UnityEngine;


public class WeaponPicker : MonoBehaviour
{
    [SerializeField] private Transform _center;
    
    private IWeapon _pickedWeapon;
    private WeaponSlot _weaponSlot;
    private bool _hasWeapon;
    
    private int Direction => _center.rotation.eulerAngles.y == 0 ? 1 : -1;

    private void Awake()
    {
        _weaponSlot = GetComponentInChildren<WeaponSlot>();
    }

    private void Update()
    {
        Flip();

        if (_hasWeapon)
        {
            //TODO: Refactoring
            _pickedWeapon.transform.position = _weaponSlot.transform.position;
            _pickedWeapon.transform.rotation = _center.transform.rotation;
            
            if (Input.GetMouseButtonDown(0))
            {
                _pickedWeapon.Shoot(Direction);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out IWeapon weapon) && !_hasWeapon)
        {
            Pick(weapon);
        }
    }

    private void Flip()
    {
        Vector2 newRotation = _center.rotation.eulerAngles;
        newRotation.y = HorizontalMove.GetMoveDirection() == 0 ? newRotation.y :
            HorizontalMove.GetMoveDirection() == 1 ? 0 : 180;
        _center.transform.rotation = Quaternion.Euler(newRotation);
    }

    private void Pick(IWeapon weapon)
    {
        _hasWeapon = true;
        _pickedWeapon = weapon;
    }
}