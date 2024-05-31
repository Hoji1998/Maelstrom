using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MoreMountains.CorgiEngine
{
    public class BossHealthBar : MonoBehaviour
    {
        [Header("BossNameText")]
        public string BossID = "BossID";
        public string BossName = "BossName";

        protected GUIManager _guiManager;

        protected Health _health;


        private void OnEnable()
        {
            if (_guiManager == null)
            {
                _guiManager = GUIManager.Instance;
            }

            if (_health == null)
            {
                _health = gameObject.GetComponent<Health>();
            }

            _guiManager.BossHealthBarObject.SetActive(true);
            BossName = LanguageSelector.Instance.GetString(BossID);
            _guiManager.BossHealthBar.TextFormat = BossName;
            _guiManager.BossHealthBar.SetBar(_health.MaximumHealth, 0, _health.MaximumHealth);
            _guiManager.UpdateBossHealthBar(_health.MaximumHealth, 0, _health.MaximumHealth, BossID);
        }

        private void OnDisable()
        {
            _guiManager.BossHealthBarObject.SetActive(false);
        }
    }
}

