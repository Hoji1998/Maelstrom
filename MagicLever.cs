using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    public class MagicLever : MonoBehaviour
    {
        [Header("Connect Door")]
        public GameObject door;

        [Header("Connect Moving Platform")]
        public MovingPlatform[] _movingPlatformLogic;

        [Header("Lever State")]
        public bool leverState = false;
        public float timeLimit = 1f;

        [Header("OnFeedback")]
        public MMFeedbacks _leverOnFeedback;

        private Animator _anim;
        private Door doorLogic;
        private Coroutine curCoroutine;

        protected virtual void InitializeFeedbacks()
        {
            _leverOnFeedback?.Initialization(this.gameObject); //!
            _leverOnFeedback?.Initialization(this.gameObject); //!
        }

        void Awake()
        {
            InitializeFeedbacks();

            _anim = GetComponent<Animator>();

            if (door == null)
                return;

            if (door.GetComponent<Door>() != null)
            {
                doorLogic = door.GetComponent<Door>();
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            DamageOnTouch weapon = collider.GetComponent<DamageOnTouch>();
            GameObject _gameobject = collider.gameObject;
            

            if (_gameobject == null)
                return;

            if (_gameobject.layer != 31)
            {
                return;
            }

            if (weapon == null)
                return;

            if (weapon.DamageCaused <= 0)
            {
                return;
            }

            _leverOnFeedback.PlayFeedbacks();

            if (leverState)
            {
                return;
            }

            LeverStateUpdate();
            curCoroutine = StartCoroutine(TimeLimit());
        }

        public void LeverStateUpdate()
        {
            if (leverState)
            {
                leverState = false;
                _anim.SetBool("LeverState", false);
            }
            else
            {
                leverState = true;
                _anim.SetBool("LeverState", true);            
            }

            if (door != null)
            {
                PassFrom_Lever_To_Door();
                DoorStateUpdate();
            }

            if (_movingPlatformLogic != null)
            {
                MovingPlatforms();
            }
        }

        private IEnumerator TimeLimit()
        {
            float curTime = 0;

            while (true)
            {
                yield return null;
                curTime += Time.deltaTime;

                if (curTime >= timeLimit)
                {
                    LeverStateUpdate();
                    StopCoroutine(curCoroutine);
                }
            }
        }


        private void MovingPlatforms()
        {
            if (leverState)
            {
                for (int i = 0; i < _movingPlatformLogic.Length; i++)
                {
                    _movingPlatformLogic[i].MoveTowardsEnd();
                }
            }
            else
            {
                for (int i = 0; i < _movingPlatformLogic.Length; i++)
                {
                    _movingPlatformLogic[i].MoveTowardsStart();
                }
            }
        }

        public void PassFrom_Lever_To_Door()
        {
            //레버의 상태를 문에 전달
            if (leverState)
            {
                for (int i = 0; i < doorLogic.leversState.Length; i++)
                {
                    if (!doorLogic.leversState[i])
                    {
                        doorLogic.leversState[i] = true;

                        if (doorLogic.leversSprite[i] != null)
                        {
                            doorLogic.leversSprite[i].sprite = doorLogic.leverOpenSprite;
                        }
                        
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < doorLogic.leversState.Length; i++)
                {
                    if (doorLogic.leversState[i])
                    {
                        doorLogic.leversState[i] = false;

                        if (doorLogic.leversSprite[i] != null)
                        {
                            doorLogic.leversSprite[i].sprite = doorLogic.leverCloseSprite;
                        }

                        return;
                    }
                }
            }
        }

        public void DoorStateUpdate()
        {
            //문의 모든상태가 참이면 문을 연다
            for (int i = 0; i < doorLogic.leversState.Length; i++)
            {
                if (!doorLogic.leversState[i])
                {
                    //doorLogic.CloseDoor();
                    return;
                }
            }

            doorLogic.OpenDoor();
        }
    }
}


