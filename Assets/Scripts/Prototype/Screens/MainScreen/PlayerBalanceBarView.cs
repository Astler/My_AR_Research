using TMPro;
using UnityEngine;

namespace Prototype.Screens
{
    public class PlayerBalanceBarView: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinsText;
        
        public void SetCoins(int coins)
        {
            coinsText.text = coins.ToString();
        }
    }
}