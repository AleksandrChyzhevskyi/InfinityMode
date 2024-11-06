using System;
using System.Collections;
using _Development.Scripts.InfinityMode.Data;
using _Development.Scripts.InfinityMode.View;
using BLINK.RPGBuilder.Characters;
using BLINK.RPGBuilder.Combat;
using BLINK.RPGBuilder.LogicMono;
using UnityEngine;

namespace _Development.Scripts.InfinityMode
{
    public class InfinityModeTeleportController : MonoBehaviour
    {
        [SerializeField] private Transform _infoLeveForOpenTeleport;
        [SerializeField] private Transform _teleport;
        [SerializeField] private TimerView _timerView;
        [SerializeField] private BoxCollider _collider;
        [SerializeField] private InfinityModeData _infinityModeData;

        private DateTime _timeWait;
        private PanelInfinityModeView _panelInfinity;
        private Coroutine _coroutine;
        private bool _isWatchAD;

        private void OnEnable()
        {
            if (_panelInfinity == null)
            {
                _panelInfinity = RPGBuilderEssentials.Instance.PanelInfinityMode;
                _panelInfinity.SetContentButton(_infinityModeData.CurrencyBuyEnter, _infinityModeData.CostBuyEnter);
            }

            if (Character.Instance.CharacterData.Level >= _infinityModeData.PlayerLevelForEnter)
                OpenTeleport();
            else
                GameEvents.CharacterLevelChanged += CheckPlayerLevel;

            _panelInfinity.ClickedWatchAD += OnWatchAD;
            _panelInfinity.ClickedBuyButton += TryBuyEnter;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += OnRewardedAdReceivedRewardEvent;
        }

        private void OnDisable()
        {
            GameEvents.CharacterLevelChanged -= CheckPlayerLevel;
            _panelInfinity.ClickedWatchAD -= OnWatchAD;
            _panelInfinity.ClickedBuyButton -= TryBuyEnter;
            IronSourceRewardedVideoEvents.onAdRewardedEvent -= OnRewardedAdReceivedRewardEvent;
        }

        public void GoToInfinityMode() => 
            GameEvents.FinishedTeleport += OnFinishedTeleport;

        private void OnFinishedTeleport() => 
            RPGBuilderEssentials.Instance.RunnerElements.StartWaitAction(0.5f, ActiveWaitPanel, 0.5f);

        private void ActiveWaitPanel()
        {
            _collider.enabled = true;
            _timerView.gameObject.SetActive(true);
            _teleport.gameObject.SetActive(false);
            _timeWait = DateTime.Now.AddMinutes(_infinityModeData.TimeWaitInMinutes);
            
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            _coroutine = StartCoroutine(StartCoroutineAction(_timeWait));
            GameEvents.FinishedTeleport -= ActiveWaitPanel;
        }

        private void OnWatchAD()
        {
            Advertising.instance.TryWatchRewardedAd();
            _panelInfinity.gameObject.SetActive(false);
            _isWatchAD = true;
        }

        private void OnFinishedTimer()
        {
            _collider.enabled = false;
            _timerView.gameObject.SetActive(false);
            _teleport.gameObject.SetActive(true);
        }

        private void CheckPlayerLevel(int level)
        {
            if (level < _infinityModeData.PlayerLevelForEnter)
                return;

            OpenTeleport();
        }

        private void OpenTeleport()
        {
            _infoLeveForOpenTeleport.gameObject.SetActive(false);
            _teleport.gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerCombatEntity _) == false)
                return;

            _panelInfinity.gameObject.SetActive(true);

            if (Advertising.instance.IsCanShowRewardedAd())
            {
                _panelInfinity.PanelTwoButtons.gameObject.SetActive(true);
                _panelInfinity.PanelOneButton.gameObject.SetActive(false);
            }
            else
            {
                _panelInfinity.PanelTwoButtons.gameObject.SetActive(false);
                _panelInfinity.PanelOneButton.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PlayerCombatEntity _) == false)
                return;

            _panelInfinity.gameObject.SetActive(false);
        }

        private void OnRewardedAdReceivedRewardEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo ironSourceAdInfo)
        {
            if (_isWatchAD == false)
                return;
            
            OnFinishedTimer();

            _isWatchAD = false;
        }

        private void TryBuyEnter()
        {
            if (Character.Instance.getCurrencyAmount(_infinityModeData.CurrencyBuyEnter) -
                _infinityModeData.CostBuyEnter >= 0 == false)
            {
                RPGBuilderEssentials.Instance.Shop.gameObject.SetActive(true);
                return;
            }

            _panelInfinity.gameObject.SetActive(false);

            BuyEnter();
            OnFinishedTimer();
        }

        private void BuyEnter()
        {
            int currencyAmount = Character.Instance.getCurrencyAmount(_infinityModeData.CurrencyBuyEnter);
            currencyAmount -= _infinityModeData.CostBuyEnter;
            EconomyUtilities.setCurrencyAmount(_infinityModeData.CurrencyBuyEnter, currencyAmount);
            GeneralEvents.Instance.OnPlayerCurrencyChanged(_infinityModeData.CurrencyBuyEnter);
        }

        private IEnumerator StartCoroutineAction(DateTime time)
        {
            while (time >= DateTime.Now)
            {
                _timerView.Show(time - DateTime.Now);
                yield return new WaitForSeconds(1f);
            }

            OnFinishedTimer();
        }
    }
}