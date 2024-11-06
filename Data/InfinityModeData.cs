using UnityEngine;

namespace _Development.Scripts.InfinityMode.Data
{
    [CreateAssetMenu(fileName = "InfinityModeData", menuName = "InfinityMode/InfinityModeData")]
    public class InfinityModeData : ScriptableObject
    {
        public int TimeWaitInMinutes;
        public RPGCurrency CurrencyBuyEnter;
        public int CostBuyEnter;
        public int PlayerLevelForEnter;
    }
}