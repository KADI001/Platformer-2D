using System;
using UnityEngine;

namespace Source
{
    public class CoinPicker : MonoBehaviour
    {
        private Wallet _wallet;

        public Wallet Wallet => _wallet;

        private void Awake()
        {
            _wallet = new Wallet();
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.transform.TryGetComponent(out Coin coin)) 
                _wallet.AddCoin();
        }
    }
}