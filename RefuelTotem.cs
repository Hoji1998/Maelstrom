using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MoreMountains.CorgiEngine
{
    public class RefuelTotem : MonoBehaviour
    {
        private CharacterItemAbilities _characterItemAbilities;
        private Character _character;
        private CharacterRewind _characterRewind;
        private bool checkMultihit = false;

        [SerializeField]
        protected int healingAmount=5;
        protected int originHealingAmount = 5;

        public float refuelPower = 0;

        public void RefuelEnergy()
        {
            if (checkMultihit)
                return;

            /*
            if (_character == null)
            {
                _character = GameManager.Instance.StoredCharacter;
                _characterItemAbilities = _character.GetComponent<CharacterItemAbilities>();
            }

            if (_characterRewind == null)
            {
                _characterRewind = _character.gameObject.GetComponent<CharacterRewind>();
            }

            if (_characterRewind.RewindFuelDurationLeft < _characterRewind.RewindFuelDuration)
            {
                _characterRewind.RewindFuelDurationLeft += refuelPower;

                if (_characterRewind.RewindFuelDurationLeft >= _characterRewind.RewindFuelDuration)
                {
                    _characterRewind.RewindFuelDurationLeft = _characterRewind.RewindFuelDuration;
                }

                _characterRewind.UpdateRewindBar();
            }
            */

            if (_character == null)
            {
                _character = GameManager.Instance.StoredCharacter;
                _characterItemAbilities = _character.GetComponent<CharacterItemAbilities>();
            }

            if (_characterRewind == null)
            {
                _characterRewind = _character.gameObject.GetComponent<CharacterRewind>();
            }

            RefuelHealth();

            checkMultihit = true;
            Invoke("ReturnMultiHitCheck", 0.15f);
        }

        private void RefuelHealth()
        {
            if (_characterRewind._shouldKeepRewinding)
            {
                if (_characterItemAbilities.sandGlassAuthorized)
                {
                    healingAmount = originHealingAmount;
                    healingAmount += SandGlass(originHealingAmount);
                    _character._health.CurrentBlackRegainHealth += healingAmount;
                }
                else
                {
                    _character._health.CurrentBlackRegainHealth += originHealingAmount;
                }


                if (_character._health.CurrentBlackRegainHealth >= _character._health.MaximumHealth)
                {
                    _character._health.CurrentBlackRegainHealth = _character._health.MaximumHealth;
                }

                if (_character._health.CurrentBlackRegainHealth >= _character._health.CurrentBlackHealth)
                {
                    _character._health.CurrentBlackRegainHealth = _character._health.CurrentBlackHealth;
                }

                GUIManager.Instance.UpdateBlackHealthRegainBar(true, _character._health.CurrentBlackRegainHealth, 0, _character._health.MaximumHealth, _character.PlayerID);
            }
        }

        private void ReturnMultiHitCheck()
        {
            checkMultihit = false;
        }

        public virtual int SandGlass(int amount)
        {
            amount = (int)(amount * 1f);
            return amount;
        }
    }
}

