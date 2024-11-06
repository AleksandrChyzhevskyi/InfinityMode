using System;
using System.Collections;
using BLINK.RPGBuilder.Characters;
using BLINK.RPGBuilder.LogicMono;
using DTT.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Development.Scripts.InfinityMode.View
{
    public class PanelInfinityModeView : MonoBehaviour
    {
        public event Action ClickedWatchAD;
        public event Action ClickedBuyButton;

        public Button CloseButton;
        public Button ADButton;
        public Button CostButtonWithAD;
        public TextMeshProUGUI TextCostButtonWithAD;
        public Button CostButton;
        public TextMeshProUGUI TextCostButton;
        public Transform PanelTwoButtons;
        public Transform PanelOneButton;
        public Color DefaultColorInTextButton;
        public Color ColorNotEnoughFunds;

        private RPGCurrency _currency;
        private int _cost;
        private Coroutine _coroutine;

        private void OnEnable()
        {
            CloseButton.onClick.AddListener(ClosePanel);
            ADButton.onClick.AddListener(WatchAD);
            CostButton.onClick.AddListener(BuyEnter);
            CostButtonWithAD.onClick.AddListener(BuyEnter);

            if (_currency == null)
                return;

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine =
                RPGBuilderEssentials.Instance.RunnerElements.UpdateWhileObjectIsActive(gameObject,
                    CheckCurrentCurrency);
        }

        private void OnDisable()
        {
            CloseButton.onClick.RemoveListener(ClosePanel);
            ADButton.onClick.RemoveListener(WatchAD);
            CostButton.onClick.RemoveListener(BuyEnter);
            CostButtonWithAD.onClick.RemoveListener(BuyEnter);
        }

        public void SetContentButton(RPGCurrency Currency, int cost)
        {
            SetText(Currency, cost, DefaultColorInTextButton);

            _currency = Currency;
            _cost = cost;
        }

        private void CheckCurrentCurrency()
        {
            SetText(_currency, _cost, Character.Instance.getCurrencyAmount(_currency) -
                _cost >= 0
                    ? DefaultColorInTextButton
                    : ColorNotEnoughFunds);
        }

        private void SetText(RPGCurrency Currency, int cost, Color color)
        {
            TextCostButtonWithAD.color = color;
            TextCostButton.color = color;

            TextCostButtonWithAD.text = $"<sprite name={Currency.entryName}> {cost}";
            TextCostButton.text = $"<sprite name={Currency.entryName}> {cost}";
        }

        private void BuyEnter() =>
            ClickedBuyButton?.Invoke();

        private void WatchAD() =>
            ClickedWatchAD?.Invoke();

        private void ClosePanel() =>
            gameObject.SetActive(false);
    }
}