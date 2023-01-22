using System;
using UnityEngine;

namespace Source
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private Controller2D _player;
        [SerializeField] private PlatformMovement _platform;

        public float distance;

        private void Update()
        {
            distance = Mathf.Abs(_player.BottomRayRange.GetRay(0.5f).origin.y -
                _platform.UpRayRange.GetRay(0.5f).origin.y);
        }
    }
}
