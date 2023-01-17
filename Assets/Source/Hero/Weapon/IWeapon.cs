using System;
using UnityEngine;

internal interface IWeapon
{
    Transform transform { get; }

    event Action Shot;

    void Shoot(int direction);
    void SwitchOff();
    void SwitchOn();
}