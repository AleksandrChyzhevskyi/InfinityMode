using _Development.Scripts.SaveLoadDatesPlayer.SaveLoadPrefs;
using UnityEngine;

namespace _Development.Scripts.InfinityMode.View
{
    public class PresenterScoreKillMobView : MonoBehaviour
    {
        [SerializeField] private ScoreKillMobView _view;

        private int _recordScore;
        private int _bestRecordScore;
        private int _actualScore;

        private void OnEnable()
        {
            CombatEvents.PlayerKilledNPCInInfinityMode += OnPlayerKilledNPCInfinityMode;
            CombatEvents.PlayerDeathAccepted += ResetActualScore;
            GeneralEvents.PlayerEnteredInfinityZone += StartShowScore;
            GeneralEvents.PlayerExitedInfiniteZone += ResetActualScore;
        }

        private void OnDisable()
        {
            CombatEvents.PlayerKilledNPCInInfinityMode -= OnPlayerKilledNPCInfinityMode;
            CombatEvents.PlayerDeathAccepted -= ResetActualScore;
            GeneralEvents.PlayerEnteredInfinityZone -= StartShowScore;
            GeneralEvents.PlayerExitedInfiniteZone -= ResetActualScore;
        }

        private void OnPlayerKilledNPCInfinityMode(int countNPC)
        {
            _actualScore += countNPC;
            _recordScore += countNPC;

            if (_recordScore > _bestRecordScore)
            {
                _bestRecordScore = _recordScore;
                SaveLoadPlayerPrefs.SaveRecordScoreInfinityMode(_bestRecordScore);
            }

            ShowScore();
        }

        private void StartShowScore()
        {
            if (SaveLoadPlayerPrefs.IsRecordScoreInfinityMode())
                _bestRecordScore = SaveLoadPlayerPrefs.LoadRecordScoreInfinityMode();

            _view.gameObject.SetActive(true);
            
            ShowScore();
        }

        private void ResetActualScore()
        {
            _view.gameObject.SetActive(false);
            _actualScore = 0;
        }

        private void ShowScore() =>
            _view.Show(_actualScore.ToString(), _bestRecordScore.ToString());
    }
}