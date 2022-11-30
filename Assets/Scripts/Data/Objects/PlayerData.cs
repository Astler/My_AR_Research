namespace Data.Objects
{
    public class PlayerData : IPlayerData
    {
        private int _coins = PlayerPrefsHelper.Coins;

        public void AddCoins(int amount)
        {
            _coins += amount;
            PlayerPrefsHelper.Coins = _coins;
        }

        public int GetCoins() => _coins;
    }
}