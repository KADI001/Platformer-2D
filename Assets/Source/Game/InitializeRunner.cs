using System;
using System.Collections;
using System.Collections.Generic;
using Source;
using UnityEngine;

public class InitializeRunner : MonoBehaviour
{
    [SerializeField] private Bootstrapper _bootstrapperPrefab;
    
    private void Awake()
    {
        Bootstrapper bootstrapper = FindObjectOfType<Bootstrapper>();

        if (bootstrapper == null)
        {
            Instantiate(_bootstrapperPrefab);
        }
    }
}
