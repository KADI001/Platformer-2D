using UnityEngine;

namespace Source
{
    public class DelayBetweenShots : MonoBehaviour, IComponent
    {
        [SerializeField] private float _delay;

        private IWeapon _weapon;
        private Timer _timerForDelay;

        private void Awake()
        {
            _weapon = GetComponent<IWeapon>();
            _timerForDelay = new Timer(_delay);
        }
        
        private void Update() => 
            _timerForDelay.Update(Time.deltaTime);

        private void OnEnable() => 
            _weapon.Shot += OnShot;

        private void OnDisable() => 
            _weapon.Shot -= OnShot;

        private void OnShot()
        {
            if(!enabled)
                return;
            
            _weapon.SwitchOff();
            _timerForDelay.Start(() => _weapon.SwitchOn());
        }
        
        public void SwitchOff() => 
            enabled = false;

        public void SwitchOn() => 
            enabled = true;
    }
}