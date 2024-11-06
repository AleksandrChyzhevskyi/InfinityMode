using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BLINK.RPGBuilder.AI;
using BLINK.RPGBuilder.LogicMono;
using UnityEngine;

namespace _Development.Scripts.InfinityMode
{
    public class ManagerSpawnerForInfinity : MonoBehaviour
    {
        [SerializeField] private List<NPCSpawner> _managerElements;

        private NPCSpawner _currentSpawner;
        private NPCSpawner _nextSpawner;
        private DateTime _respawnTime;
        private Coroutine _coroutine;
        private int _countKillPlayer;

        private void OnEnable() =>
            CombatEvents.PlayerDeathAccepted += ResetSpawnerWave;

        private void OnDisable() =>
            CombatEvents.PlayerDeathAccepted -= ResetSpawnerWave;

        public void StartSpawnWave()
        {
            GeneralEvents.Instance.OnPlayerEnteredInfinityZone();
            _coroutine = StartCoroutine(StartInfinityMode());
        }

        private IEnumerator StartInfinityMode()
        {
            if (_currentSpawner == null)
            {
                _currentSpawner = _managerElements.FirstOrDefault(x => x.InfinityDataMode.StartSpawner);
                _currentSpawner.SetReadyInfinityMode(true);
                _countKillPlayer = _currentSpawner.npcCountMax;
            }
            
            while (_currentSpawner.IsActive == false)
                yield return new WaitForSeconds(0.1f);

            RPGBuilderEssentials.Instance.QuestsController.Timer.gameObject.SetActive(false);

            CombatEvents.Instance.OnNewWaveInfinityMode();
            
            while (_currentSpawner.GetTotalNPCs() < _currentSpawner.npcCountMax)
                yield return new WaitForSeconds(0.1f);

            while (_countKillPlayer >= 0)
            {
                if (_countKillPlayer > _currentSpawner.GetTotalNPCs())
                {
                    CombatEvents.Instance.OnPlayerKilledNPCInInfinityMode(_countKillPlayer -
                                                                          _currentSpawner.GetTotalNPCs());
                    _countKillPlayer = _currentSpawner.GetTotalNPCs();
                }

                if (_countKillPlayer == 0)
                    _countKillPlayer--;

                yield return new WaitForSeconds(0.1f);
            }

            SetTimerSpawn();
            
            while (_respawnTime >= DateTime.Now)
            {
                if( RPGBuilderEssentials.Instance.QuestsController.Timer.gameObject.activeInHierarchy == false)
                    RPGBuilderEssentials.Instance.QuestsController.Timer.gameObject.SetActive(true);
                
                RPGBuilderEssentials.Instance.QuestsController.Timer.Show(_respawnTime - DateTime.Now);
                yield return new WaitForSeconds(1f);
            }

            RPGBuilderEssentials.Instance.QuestsController.Timer.Show();
            
            _currentSpawner.SetReadyInfinityMode(false);
            _currentSpawner.gameObject.SetActive(false);
            _currentSpawner = _managerElements.FirstOrDefault(y => y.gameObject.activeInHierarchy);


            if (_currentSpawner != null)
            {
                _currentSpawner.SetReadyInfinityMode(true);
                _respawnTime = default;
                _countKillPlayer = _currentSpawner.npcCountMax;
                _coroutine = StartCoroutine(StartInfinityMode());
            }
        }

        public void ResetSpawnerWave()
        {
            if (_coroutine == null || _currentSpawner == null)
                return;

            StopCoroutine(_coroutine);

            _currentSpawner.gameObject.SetActive(false);

            foreach (NPCSpawner npcSpawner in _managerElements)
            {
                if (npcSpawner.gameObject.activeInHierarchy == false)
                {
                    npcSpawner.gameObject.SetActive(true);
                    npcSpawner.SetReadyInfinityMode(npcSpawner.InfinityDataMode.StartSpawner);
                    npcSpawner.spawnedCount = 0;
                    GameState.Instance.RemoveSpawnerFromList(npcSpawner);
                    npcSpawner.IsActive = false;
                }
            }

            _respawnTime = default;
            _currentSpawner = null;
            GeneralEvents.Instance.OnPlayerExitInfinityZone();
        }

        private void SetTimerSpawn()
        {
            if(_respawnTime != default)
                return;
            
            DateTime time = DateTime.Now;
            _respawnTime = time.AddSeconds(_currentSpawner.InfinityDataMode.CooldownBetweenWave);
        }
    }
}