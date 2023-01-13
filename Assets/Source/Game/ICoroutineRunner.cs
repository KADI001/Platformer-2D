using System.Collections;
using UnityEngine;

namespace Source
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}