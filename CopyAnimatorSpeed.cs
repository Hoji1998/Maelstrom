using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class CopyAnimatorSpeed : MonoBehaviour
    {
        public Character _parrentTimeObj;

        public Weapon _weapon;

        private Animator _anim;
        private float _initialDelayBeforeUse;
        private float _initialTimeBetweenUses;

        void Awake()
        {
            _anim = GetComponent<Animator>();

            if (_parrentTimeObj == null)
            {
                _parrentTimeObj = GameManager.Instance.StoredCharacter;
            }

            if (_weapon == null)
            {
                _weapon = _parrentTimeObj.FindAbility<CharacterHandleWeapon>().InitialWeapon;
            }

            _initialDelayBeforeUse = _weapon.DelayBeforeUse;
            _initialTimeBetweenUses = _weapon.TimeBetweenUses;
            
        }

        private void OnEnable()
        {
            if (_anim != null)
            {
                _anim.Play("swordAttack");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!gameObject.activeSelf)
                return;

            if (_anim != null)
            {
                _anim.speed = _parrentTimeObj.TimeValue;
            }
            

            _weapon.DelayBeforeUse =  _initialDelayBeforeUse / _parrentTimeObj.TimeValue;
            _weapon.TimeBetweenUses =  _initialTimeBetweenUses / _parrentTimeObj.TimeValue;
        }
    }

}

