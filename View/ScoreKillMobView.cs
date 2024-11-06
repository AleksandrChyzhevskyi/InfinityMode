using TMPro;
using UnityEngine;

namespace _Development.Scripts.InfinityMode.View
{
    public class ScoreKillMobView : MonoBehaviour
    {
        public TextMeshPro RecordScore;
        public TextMeshPro ActualScore;

        public void Show(string actualScore, string recordScore)
        {
            ActualScore.text = $"Actual score: {actualScore}";
            RecordScore.text = $"Record score: {recordScore}";
        }
    }
}