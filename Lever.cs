using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
	public class Lever : MonoBehaviour
	{
		[Header("Connect Door")]
		public GameObject door;
		public bool Isdoor = true;

        [Header("Connect Moving Platform")]
        public MovingPlatform[] _movingPlatformLogic;

        [Header("Lever State")]
		public bool leverState = false;
		public float timeLimit = 1f;
		public bool inf = false;

		[Header("elevator")]
		public bool elevatorReturn = false;
		public bool elevatorLever = false;

		[Header("Lever Sprite")]
		public Sprite onSprite;
		public Sprite offSprite;

		private Coroutine curCoroutine;
		private Coroutine lineColorCoroutine;

		private SpriteRenderer curSprite;
		private Door doorLogic;
		private MovingPlatform elevatorLogic;

		[Header("OnFeedback")]
		public MMFeedbacks _leverOnFeedback;
        public MMFeedbacks _elevatorStartFeedback;
		/// </summary>
		/// <param name="collider">The object that collides with the KillPlayerOnTouch object.</param>
		/// 
		protected virtual void InitializeFeedbacks()
		{
			_leverOnFeedback?.Initialization(this.gameObject); //!
            _elevatorStartFeedback.Initialization(this.gameObject);
		}
		void Awake()
        {
			InitializeFeedbacks();

			curSprite = this.GetComponent<SpriteRenderer>();

			if(Isdoor == false)
            {
				return;
            }
			if(door!=null)
            {
				if (door.GetComponent<Door>() != null)
				{
					doorLogic = door.GetComponent<Door>();
                    doorLogic.levers.Add(this);

                }

				if (door.GetComponent<MovingPlatform>() != null)
				{
					elevatorLogic = door.GetComponent<MovingPlatform>();
					elevatorLogic.ChangeDirection();
					elevatorLogic.MoveTowardsEnd();
				}
			}
		}

        protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
			DamageOnTouch weapon = collider.GetComponent<DamageOnTouch>();
			GameObject _gameobject = collider.gameObject;

			if (_gameobject == null)
				return;

			if (_gameobject.layer != 28)
				return;

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

            ChooseLever();
		}

		public void ChooseLever()
        {
            curCoroutine = StartCoroutine(TimeLimit());
            LeverStateUpdate();

            if (door.GetComponent<MovingPlatform>() != null)
            {
                timeLimit = 0.2f;

                ElevatorOn();
            }
        }

		public void ElevatorOn()
        {
			if (!elevatorLever)
            {
				//움직이지 않을때만 실행
				if (elevatorLogic.CurrentSpeed != Vector3.zero)
				{
					return;
				}

				if (elevatorReturn && elevatorLogic.forwardDirection)
				{
					elevatorLogic.MoveTowardsStart();

					elevatorLogic.forwardDirection = false;

                    _elevatorStartFeedback.PlayFeedbacks();
                    elevatorLogic.PointReachedFeedback.PlayFeedbacks();
                }
				else if (!elevatorReturn && !elevatorLogic.forwardDirection)
				{
					elevatorLogic.MoveTowardsEnd();

					elevatorLogic.forwardDirection = true;

                    _elevatorStartFeedback.PlayFeedbacks();
                    elevatorLogic.PointReachedFeedback.PlayFeedbacks();
                }
            }
            else
            {
				if (elevatorLogic.forwardDirection)
				{
					elevatorLogic.MoveTowardsStart();

					elevatorLogic.forwardDirection = false;
				}
				else if (!elevatorLogic.forwardDirection)
				{
					elevatorLogic.MoveTowardsEnd();

					elevatorLogic.forwardDirection = true;
				}

                if (elevatorLogic.CurrentSpeed.y == 0)
                {
                    _elevatorStartFeedback.PlayFeedbacks();
                    elevatorLogic.PointReachedFeedback.PlayFeedbacks();
                }
            }
			
		}

		private IEnumerator TimeLimit()
        {
			float curTime = 0;

			while (true)
            {
				yield return null;
				curTime += Time.deltaTime;

				if (curTime >= timeLimit && !inf)
                {
					LeverStateUpdate();
					StopCoroutine(curCoroutine);
                }
            }
        }

		public void LeverStateUpdate()
        {
			if (leverState)
            {
				leverState = false;
				curSprite.sprite = offSprite;
			}
			else
            {
				leverState = true;
				curSprite.sprite = onSprite;
			}

			//문일 경우 실행
            if (door != null)
            {
                if (door.GetComponent<Door>() != null)
                {
                    PassFrom_Lever_To_Door();
                    DoorStateUpdate();
                }
            }

            if (_movingPlatformLogic.Length > 0)
            {
                if (_movingPlatformLogic[0] != null)
                {
                    MovingPlatforms();
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

                        for (int index = 0; index < doorLogic.levers.Count; index++)
                        {
                            if(doorLogic.levers[index] == this)
                            {
                                DoorStoneLogic(index, true);
                            }
                        }
                    
                        //if (doorLogic.leversSprite[i] != null)
                        //{
                        //    doorLogic.leversSprite[i].sprite = doorLogic.leverOpenSprite;
                        //}

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

                        for (int index = 0; index < doorLogic.levers.Count; index++)
                        {
                            if (doorLogic.levers[index] == this)
                            {
                                DoorStoneLogic(index, false);
                            }
                        }

                        //if (doorLogic.leversSprite[i] != null)
                        //{
                        //    doorLogic.leversSprite[i].sprite = doorLogic.leverCloseSprite;
                        //}

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

        public virtual void DoorStoneLogic(int index, bool open = true)
        {
            if(open)
            {
                switch (doorLogic.levers.Count)
                {
                    case 1:
                        doorLogic.leversSprite[index].sprite = doorLogic.leverOpenSprite;
                        break;
                    case 2:
                        doorLogic.leversSprite[index + 1].sprite = doorLogic.leverOpenSprite;
                        break;
                    case 3:
                        doorLogic.leversSprite[index].sprite = doorLogic.leverOpenSprite;
                        break;
                    case 4:
                        doorLogic.leversSprite[index + 1].sprite = doorLogic.leverOpenSprite;
                        break;
                    case 5:
                        doorLogic.leversSprite[index].sprite = doorLogic.leverOpenSprite;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (doorLogic.levers.Count)
                {
                    case 1:
                        doorLogic.leversSprite[index].sprite = doorLogic.leverCloseSprite;
                        break;
                    case 2:
                        doorLogic.leversSprite[index + 1].sprite = doorLogic.leverCloseSprite;
                        break;
                    case 3:
                        doorLogic.leversSprite[index].sprite = doorLogic.leverCloseSprite;
                        break;
                    case 4:
                        doorLogic.leversSprite[index + 1].sprite = doorLogic.leverCloseSprite;
                        break;
                    case 5:
                        doorLogic.leversSprite[index].sprite = doorLogic.leverCloseSprite;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}