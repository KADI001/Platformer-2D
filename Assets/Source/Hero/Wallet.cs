using System;

namespace Source
{
    public class Wallet
    {
        private int _coins;

        public event Action CoinAdded;
        public event Action CoinRemoved;

        public Wallet()
        {
        }

        public Wallet(int coins)
        {
            _coins = coins;
        }

        public int Coins => _coins;

        public void AddCoin()
        {
            _coins++;
            CoinAdded?.Invoke();
        }

        public bool TryRemoveCoin()
        {
            if (_coins - 1 < 0) 
                return false;
            
            _coins--;
            CoinRemoved?.Invoke();
            return true;
        }

        public void Clear() => 
            _coins = 0;
    }
}
