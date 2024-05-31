using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using System;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this class to a character and it'll be able to time rewind
    /// Animator parameters : Rewind
    /// </summary>
    public class CharacterRewind : CharacterAbility
    {
        /// This method is only used to display a helpbox text at the beginning of the ability's inspector
        public override string HelpBoxText()
        {
            return "Character Time Rewind";
        }
        [Header("magicalOrbs")]
        public GameObject magicalOrbs;

        [Header("Rewind")]
        public bool infiniteEnergy = false;

        [Header("Cooldown")]
        /// the duration of the cooldown between 2 Rewind (in seconds)
        [Tooltip("the duration of the cooldown between 2 Rewind (in seconds)")]
        public float RewindCooldown = 0.1f;
        public float RewindingTime = 3f;

        [Header("GUI")]
        public GameObject _rewindStartPoint;

        public float RewindFuelDurationLeft = 0f;
        [Tooltip("the maximum duration (in seconds) of the rewind")]
        public float RewindFuelDuration = 5f;
        /// the jetpack refuel cooldown, in seconds
        [Tooltip("the rewind refuel cooldown, in seconds")]
        public float RewindRefuelCooldown = 1f;
        [Tooltip("true if the character has unlimited fuel for its rewind")]
        public bool RewindUnlimited = false;
        [Tooltip("the speed at which the Rewind refuels")]
        public float RefuelSpeed = 0.5f;
        protected float originRefuelSpeed = 0.5f;
        [Tooltip("the minimum amount of fuel required in the tank to be able to Rewind again")]
        public float MinimumFuelRequirement = 0.2f;
        public bool _stillFuelLeft = true;
        public bool _doorOpen = false;

        protected WaitForSeconds _rewindRefuelCooldownWFS;

        protected float _cooldownTimeStamp = 0;
        protected float _startTime;
        protected Vector3 _initialPosition;
        protected Vector2 _rewindDirection;
        protected float _distanceTraveled = 0f;
        public bool _shouldKeepRewinding = false;
        protected float _slopeAngleSave = 0f;
        protected bool _rewindEndedNaturally = true;
        protected IEnumerator _rewindCoroutine;
        protected IEnumerator _rewindTimeStopCoroutine;
        protected IEnumerator _rewindTimeReturnCoroutine;
        protected CharacterDive _characterDive;
        protected float _lastRewindAt = 0f;
        protected MMLineRendererDriver _rewindLine;
        protected bool _shouldKeepRewindingEffect = false;

        // animation parameters
        protected const string _rewindingAnimationParameterName = "Rewinding";
        protected int _rewindingAnimationParameter;

        protected InventoryEngine.InventoryInputManager _inventoryInputManager;

        //save health
        public int _RewindHealth = 0;
        Health _characterHealth;
        CharacterJump _characterJump;
        CharacterHorizontalMovement _horizontalMovement;
        CharacterWallClinging _characterWallClinging;
        CharacterItemAbilities _characterItemAbilities;
        /// <summary>
        /// Initializes our aim instance
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _characterHealth = _character.GetComponent<Health>();
            _characterJump = _character.GetComponent<CharacterJump>();
            _horizontalMovement = _character.FindAbility<CharacterHorizontalMovement>();
            _characterWallClinging = _character.FindAbility<CharacterWallClinging>();
            _rewindStartPoint = GameManager.Instance._rewindStartPoint;
            _rewindLine = _rewindStartPoint.GetComponent<MMLineRendererDriver>();

            _rewindLine.Targets[0] = _character.transform;
            _rewindLine.Targets[1] = _rewindStartPoint.transform;
            _rewindLine.BindPositionsToTargets();

            RewindFuelDurationLeft = RewindFuelDuration;
            _rewindRefuelCooldownWFS = new WaitForSeconds(RewindRefuelCooldown);

            if (GUIManager.Instance != null && _character.CharacterType == Character.CharacterTypes.Player)
            {
                GUIManager.Instance.SetJetpackBar(!RewindUnlimited, _character.PlayerID);
                UpdateRewindBar();
            }

            _RewindHealth = _health.InitialHealth;
            _inventoryInputManager = GUIManager.Instance._inventoryInputManager;

            _characterItemAbilities = GetComponent<CharacterItemAbilities>();
            //InitiateRewind();
            //StopRewind();
        }

        public virtual void UpdateRewindBar()
        {
            if (Application.isPlaying)
            {
                if ((GUIManager.Instance != null) && (_character.CharacterType == Character.CharacterTypes.Player))
                {
                    GUIManager.Instance.UpdateRewindBar(RewindFuelDurationLeft, 0f, RewindFuelDuration, _character.PlayerID);
                }
            }
            
        }

        /// <summary>
        /// At the start of each cycle, we check if we're pressing the Rewind button. If we
        /// </summary>
        protected override void HandleInput()
        {
            if (_inventoryInputManager.InventoryIsOpen || _inventoryInputManager.cutScene)
            {
                return;
            }

            if ((_inputManager.RollButton.State.CurrentState == MMInput.ButtonStates.ButtonDown) && _stillFuelLeft)
            {
                if (_character._stillRewinding)
                {
                    //StopRewind();
                    //_stillFuelLeft = false;

                    return;
                }

                if ((!RewindUnlimited) && (RewindFuelDurationLeft <= 0f))
                {
                    StopRewind();
                    //_stillFuelLeft = false;
                    return;
                }

                StartRewind();
            }
        }

        /// <summary>
        /// The second of the 3 passes you can have in your ability. Think of it as Update()
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            Refuel();
        }

        /// <summary>
        /// Causes the character to dash or dive (depending on the vertical movement at the start of the dash)
        /// </summary>
        /// 
        
        public virtual void StartRewind()
        {
            if (!RewindAuthorized())
            {
                return;
            }

            if (!RewindConditions())
            {
                return;
            }
            
            InitiateRewind();
        }
        
        /// <summary>
        /// This method evaluates the internal conditions for a dash (cooldown between dashes, amount of dashes left) and returns true if a dash can be performed, false otherwise
        /// </summary>
        /// <returns></returns>
        public virtual bool RewindConditions()
        {
            // if we're in cooldown between two dashes, we prevent dash
            if (_cooldownTimeStamp > Time.time)
            {
                return false;
            }
            // if we don't have dashes left, we prevent dash

            return true;
        }

        /// <summary>
        /// Checks if conditions are met to reset the amount of dashes left
        /// </summary>

        /// <summary>
        /// This method evaluates the external conditions (state, other abilities) for a dash, and returns true if a dash can be performed, false otherwise
        /// </summary>
        /// <returns></returns>
        public virtual bool RewindAuthorized()
        {
            if (!_character._rewindAuthroized)
            {
                return false;
            }

            if (_shouldKeepRewindingEffect)
            {
                return false;
            }

            // if the rewind action is enabled in the permissions, we continue, if not we do return position
            if (_shouldKeepRewinding && _condition.CurrentState != CharacterStates.CharacterConditions.Dead)
            {
                StopRewind();
                return false;
            }

            if (RewindFuelDurationLeft < 1f)
            {
                return false;
            }
            
            if (!AbilityAuthorized
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                || (_movement.CurrentState == CharacterStates.MovementStates.LedgeHanging)
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping)
                || (_movement.CurrentState == CharacterStates.MovementStates.Dashing))
                return false;

            // If the user presses the dash button and is not aiming down
            if (_characterDive != null)
            {
                if ((_characterDive.AbilityAuthorized) && (_characterDive.enabled))
                {
                    if (_verticalInput < -_inputManager.Threshold.y)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// initializes all parameters prior to a dash and triggers the pre dash feedbacks
        /// </summary>
		public virtual void InitiateRewind()
        {
            if(_characterItemAbilities.magicalOrbAuthorized)
            {
                ActiveOrbs();
            }
            // we set its dashing state to true
            _movement.ChangeState(CharacterStates.MovementStates.Rewind);

            // we start our sounds
            PlayAbilityStartFeedbacks();

            // we initialize our various counters and checks
            _startTime = Time.time;
            _rewindEndedNaturally = false;
            _initialPosition = this.transform.position;
            _distanceTraveled = 0;
            _shouldKeepRewinding = true;
            _character._rewinding = true;
            _lastRewindAt = Time.time;
            

            _rewindStartPoint.SetActive(true);
            _rewindStartPoint.transform.position = _character.transform.position;

            // we launch the boost corountine with the right parameters
            _rewindCoroutine = Rewind();
            StartCoroutine(_rewindCoroutine);
        }

        /// <summary>
        /// Computes the dash direction based on the selected options
        /// </summary>


        /// <summary>
        /// Prevents the character from dashing into the ground when already grounded and if AutoCorrectTrajectory is checked
        /// </summary>

        /// <summary>
        /// Checks whether or not a character flip is required, and flips the character if needed
        /// </summary>
     

        /// <summary>
        /// Coroutine used to move the player in a direction over time
        /// </summary>
        protected virtual IEnumerator Rewind()
        {
            // if the character is not in a position where it can move freely, we do nothing.
            if (!AbilityAuthorized
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                yield break;
            }

            float curTime = 0;
            
            _movement.RestorePreviousState();

            _characterHealth.CurrentBlackRegainHealth = _characterHealth.CurrentHealth;
            GUIManager.Instance.UpdateBlackHealthRegainBar(true, _characterHealth.CurrentBlackRegainHealth, 0, _characterHealth.MaximumHealth, _character.PlayerID);
            // we keep dashing until we've reached our target distance or until we get interrupted

            RewindFuelDurationLeft -= 0.5f;

            if (RewindFuelDurationLeft < 0)
            {
                RewindFuelDurationLeft = 0f;
                StopRewind();
            }

            _character._changingRoom = false;

            while (true)//curTime < RewindingTime)
            {
                curTime += Time.deltaTime;
                yield return null;

                if (!RewindUnlimited)
                {
                    if ((RewindFuelDurationLeft > 0))
                    {
                        RewindFuelDurationLeft -= Time.deltaTime;

                        if (RewindFuelDurationLeft < 0)
                        {
                            RewindFuelDurationLeft = 0f;
                            StopRewind();
                        }

                        UpdateRewindBar();
                    }
                }

                if (_character._changingRoom)
                {
                    StopRewind();
                }

                if (_condition.CurrentState == CharacterStates.CharacterConditions.Dead || _condition.CurrentState == CharacterStates.CharacterConditions.Stunned)
                {
                    if (_characterHealth.CurrentHealth <= 0)
                    {
                        _character._changingRoom = true;
                        StopRewind();
                    }
                }
            }

            //StopRewind();
        }
        protected virtual void Refuel()
        {
            if (RewindUnlimited)
            {
                
            }
            else if (_shouldKeepRewinding)
            {
                return;
            }

            // we wait for a while before starting to refill
            if (Time.time - _lastRewindAt < RewindRefuelCooldown)
            {
                return;
            }

            //_refueling = false;

            // then we progressively refill the jetpack fuel
            if ((RewindFuelDurationLeft < RewindFuelDuration))
            {
                //_refueling = true;

                if (infiniteEnergy)
                {
                    if(_characterItemAbilities.littleGearAuthorized)
                    {
                        RefuelSpeed = originRefuelSpeed;
                        RefuelSpeed += LittleGear(originRefuelSpeed);
                        RewindFuelDurationLeft += Time.deltaTime * RefuelSpeed;
                    }
                    else
                    {
                        RewindFuelDurationLeft += Time.deltaTime * originRefuelSpeed;
                    }

                }

                UpdateRewindBar();
                // we prevent the character to jetpack again while at low fuel and refueling
                if ((!_stillFuelLeft) && (RewindFuelDurationLeft > MinimumFuelRequirement))
                {
                    _stillFuelLeft = true;
                }

                // if we're full, we play our refueled sound 
                /*
                if (System.Math.Abs(RewindFuelDurationLeft - RewindFuelDuration) < RewindFuelDuration / 100)
                {
                    RewindFuelDurationLeft = RewindFuelDuration;
                    PlayJetpackRefueledSfx();
                }
                */
            }
        }

        /// <summary>
        /// Stops the rewind coroutine and resets all necessary parts of the character
        /// </summary>
        public virtual void StopRewind()
        {
            if (_rewindCoroutine != null)
            {
                StopCoroutine(_rewindCoroutine);
            }

            // once our rewind is complete, we reset our various states
            _rewindEndedNaturally = true;

            //_rewindStartPoint.SetActive(false);

            // we reset our forces
            /*
            if (_condition.CurrentState == CharacterStates.CharacterConditions.Dead && _characterHealth.CurrentHealth <= 0)
            {
                _shouldKeepRewinding = false;
                _character._stillRewinding = false;
                _character._rewinding = false;
                return;
            }
            */

            if (_characterHealth.CurrentHealth <= 0)
            {
                _rewindStartPoint.SetActive(false);

                _shouldKeepRewinding = false;
                _character._changingRoom = false;
                _character._stillRewinding = false;
                _character._rewinding = false;

                GameObject[] _photogene = GameObject.FindGameObjectsWithTag("Photogene");
                //int _startNumber = _photogene.Length - 1;

                for (int i = 0; i < _photogene.Length; i++)
                {
                    _photogene[i].SetActive(false);
                }

                _rewindStartPoint.GetComponent<Photogene>().CancelInvoke("PhotogeneStart");
                _rewindStartPoint.GetComponent<Photogene>().PhotogeneStop();

                StopAllCoroutines();

                return;
            }
            //return position
            //_shouldKeepRewinding = false;

            MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 1f, 0f, false, 0f, true);
            //_character.TimeSetting(1f);
            //_character.MonsterTimeValue = 1f;

            if (_character._changingRoom || _character._stillRewinding)
            {
                StopRewindEffect();
            }
            else if (!_character._health._stillKnockback || RewindFuelDurationLeft <= 0)
            {
                // we play our exit sound
                StopStartFeedbacks();
                PlayAbilityStartFeedbacks();
                _horizontalMovement.StopStartFeedbacks();

                if (RewindFuelDurationLeft <= 0)
                {
                    //this.transform.position = _initialPosition;
                    //StopRewindEffect();

                    RewindFuelDurationLeft = 0.001f;

                    _rewindTimeReturnCoroutine = RewindTimeStopReturnEffect();
                    StartCoroutine(_rewindTimeReturnCoroutine);
                    return;
                }
                _rewindTimeStopCoroutine = RewindTimeStopEffect();
                StartCoroutine(_rewindTimeStopCoroutine);
            }
        }

        IEnumerator RewindTimeStopEffect()
        {
            MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0.1f, 0f, false, 0f, true);
            //_character.TimeSetting(0.1f);
            //_character.MonsterTimeValue = 0.1f;
            float curTime = 0;

            _character.GetComponent<BoxCollider2D>().enabled = false;
            _controller.CollisionsOff();
            //_character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);

            _character._stillRewinding = true;

            while (true)
            {
                yield return null;

                if (_inventoryInputManager.InventoryIsOpen)
                {
                    continue;
                }

                curTime += Time.unscaledDeltaTime;

                _controller.SetForce(Vector2.zero);
                _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);

                if ((_inputManager.RollButton.State.CurrentState == MMInput.ButtonStates.ButtonUp) || curTime >= 2.0f)
                {
                    StopCoroutine(_rewindTimeStopCoroutine);

                    //_characterWallClinging.

                    _rewindTimeReturnCoroutine = RewindTimeStopReturnEffect();
                    StartCoroutine(_rewindTimeReturnCoroutine);
                }
            }
        }

        
        IEnumerator RewindTimeStopReturnEffect()
        {
            _shouldKeepRewindingEffect = true;
            MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0.1f, 0f, false, 0f, true);

            float curTime = 0;
            float _dirDistanceX = 0;
            float _dirDistanceY = 0;
            bool _dirDistanceXBetter = false;

            bool _returnInverseX = false;
            bool _returnInverseY = false;
            

            bool _stopReturnEffect = false;
            Vector3 curTr = this.transform.position;

            _character.GetComponent<BoxCollider2D>().enabled = false;
            _controller.CollisionsOff();
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);
            //_characterWallClinging.ExitWallClinging();
            //_character.MovementState.ChangeState(CharacterStates.MovementStates.Rewind);

            Vector3 dirVec = _initialPosition - this.transform.position;

            if (_initialPosition.x > this.transform.position.x)
            {
                _returnInverseX = true;
            }
            else if (_initialPosition.x < this.transform.position.x)
            {
                _returnInverseX = false;
            }

            if (_initialPosition.y > this.transform.position.y)
            {
                _returnInverseY = true;
            }
            else if (_initialPosition.y < this.transform.position.y)
            {
                _returnInverseY = false;
            }

            _dirDistanceX = Mathf.Abs(_initialPosition.x - this.transform.position.x);
            _dirDistanceY = Mathf.Abs(_initialPosition.y - this.transform.position.y);

            if (_dirDistanceX > _dirDistanceY)
            {
                _dirDistanceXBetter = true;
            }
            else
            {
                _dirDistanceXBetter = false;
            }

            while (true)
            {
                yield return new WaitForSecondsRealtime(0.02f);

                if (_doorOpen)
                {
                    _controller.SetForce(Vector2.zero);
                    continue;
                }

                dirVec = _initialPosition - curTr;

                _controller.SetForce(dirVec * curTime);

                curTime += curTime + 0.1f;
                if (curTime >= 50f)
                {
                    curTime = 50f;
                }

                if (_returnInverseX && _dirDistanceXBetter)
                {
                    if (_initialPosition.x <= this.transform.position.x)
                    {
                        _stopReturnEffect = true;
                    }
                }
                else if (!_returnInverseX && _dirDistanceXBetter)
                {
                    if (_initialPosition.x >= this.transform.position.x)
                    {
                        _stopReturnEffect = true;
                    }
                }

                if (_returnInverseY && !_dirDistanceXBetter)
                {
                    if (_initialPosition.y <= this.transform.position.y)
                    {
                        _stopReturnEffect = true;
                    }
                }
                else if (!_returnInverseY && !_dirDistanceXBetter)
                {
                    if (_initialPosition.y >= this.transform.position.y)
                    {
                        _stopReturnEffect = true;
                    }
                }

                if (_stopReturnEffect)
                {
                    _controller.SetForce(Vector2.zero);

                    _shouldKeepRewindingEffect = false;
                    
                    MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 1f, 0f, false, 0f, true);
                    //_character.TimeSetting(1f);
                    //_character.MonsterTimeValue = 1f;
                    _character.GetComponent<BoxCollider2D>().enabled = true;
                    _controller.CollisionsOn();

                    StopCoroutine(_rewindTimeReturnCoroutine);

                    _characterJump.SetNumberOfJumpsLeft(0);
                    PlayAbilityStopFeedbacks();
                    this.transform.position = _initialPosition;
                    _rewindStartPoint.SetActive(false);

                    StopRewindEffect();
                }
            }
        }

        protected void StopRewindEffect()
        {
            _cooldownTimeStamp = 0;
            _cooldownTimeStamp = Time.time + RewindCooldown; 

            MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 1f, 0f, false, 0f, true);
            //return helath

            if (_characterHealth.CurrentHealth <= 0)
            {
                _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);
                _rewindStartPoint.SetActive(false);
                return;
            }
            else
            {
                _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
            }

            if (!_character._changingRoom)
            {
                //_characterHealth.CurrentHealth = _RewindHealth;
            }
            
            _characterHealth.CurrentBlackHealth = _character._health.CurrentBlackRegainHealth;
            _characterHealth.CurrentBlackRegainHealth = 0;
            
            GUIManager.Instance.UpdateBlackHealthBar(false, _characterHealth.CurrentBlackHealth, 0, _characterHealth.MaximumHealth, _character.PlayerID);
            GUIManager.Instance.UpdateBlackHealthRegainBar(false, _characterHealth.CurrentBlackRegainHealth, 0, _characterHealth.MaximumHealth, _character.PlayerID);
            _characterHealth.CurrentHealth = _characterHealth.CurrentBlackHealth;

            _characterHealth.UpdateHealthBar(true, true);

            _rewindStartPoint.SetActive(false);

            _shouldKeepRewinding = false;
            _character._changingRoom = false;
            _character._stillRewinding = false;
            _character._rewinding = false;

            GameObject[] _photogene = GameObject.FindGameObjectsWithTag("Photogene");
            //int _startNumber = _photogene.Length - 1;

            for (int i = 0; i < _photogene.Length; i++)
            {
                _photogene[i].SetActive(false);
            }

            _rewindStartPoint.GetComponent<Photogene>().CancelInvoke("PhotogeneStart");
            _rewindStartPoint.GetComponent<Photogene>().PhotogeneStop();
            
            StopAllCoroutines();
            if (_characterItemAbilities.magicalOrbAuthorized)
            {
                InactiveOrbs();
            }
            // once the boost is complete, if we were rewind, we make it stop and start the rewind cooldown
            if (_movement.CurrentState == CharacterStates.MovementStates.Rewind)
            {
                if (_controller.State.IsGrounded)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                }
                else
                {
                    _movement.RestorePreviousState();
                }
            }
            
        }
        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_rewindingAnimationParameterName, AnimatorControllerParameterType.Bool, out _rewindingAnimationParameter);
        }

        /// <summary>
        /// At the end of the cycle, we update our animator's Dashing state 
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _rewindingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Rewind), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }

        /// <summary>
        /// On reset ability, we cancel all the changes made
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();
            if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
            {
                StopRewind();
            }

            _stillFuelLeft = true;

            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _rewindingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }

        public virtual void ActiveOrbs()
        {
            magicalOrbs.SetActive(true);
        }

        public virtual void InactiveOrbs()
        {
            magicalOrbs.SetActive(false);
        }

        public virtual float LittleGear(float speed)
        {
            speed = (speed * 0.2f);
            return speed;
        }
    }
}