namespace Prototype.Data
{
    public class PlayerData : IPlayerData
    {
        private int _coins = PlayerPrefsHelper.Coins;

        public void AddCoin()
        {
            _coins += 1;

            PlayerPrefsHelper.Coins = _coins;
        }

        public int GetCoins() => _coins;
    }
}