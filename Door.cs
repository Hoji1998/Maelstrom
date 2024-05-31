using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{
    public class Door : MonoBehaviour
    {
        public bool tokenDoor;
        public string doorKey;
        public bool mirrorDoor;
        public DoorKeyWarningScript doorWarning;
        protected List<int> findList = new List<int>();

        [MMCondition("mirrorDoor", true)]
        [SerializeField][Range(0,50f)] protected float detectLength = 2f;
        protected Color _gizmoColor = MMColors.Purple;
        protected Collider2D _detectionCollider = null;
        protected bool mirrorActivated = false;
        protected CharacterItemAbilities _characterItemAbilities;

        [Header("Lever State")]
        public List<Lever> levers = new List<Lever>();
        public bool[] leversState;
        public List<SpriteRenderer> leversSprite = new List<SpriteRenderer>();
        public Sprite leverOpenSprite;
        public Sprite leverCloseSprite;
        public GameObject[] doorStones = new GameObject[5];
        public List<MMFeedbacks> stoneFeedbacks;

        [Header("Open OneTime")]
        public bool openOneTime = false;
        
        SpriteRenderer _sprite;

        [Header("Additional Door")]
        public Door[] _additoinalDoor;
        [Tooltip("if you checked camera move, don't check this parameter")]
        public bool _triggerDoor = false;
        public bool _closedDoor = false;

        [Header("Camera Move")]
        public GameObject _mainCamera;
        public Animator _overLay;
        Vector3 _cameraTr;
        Coroutine _coroutine;

        Character _character;
        CharacterHorizontalMovement _horizontalMovement;
        Health _characterHealth;
        CorgiController _controller;
        InventoryInputManager _inventoryInputManager;
        [MMReadOnly]
        public Animator _anim;

        [Header("OnFeedback")]
        public MMFeedbacks _doorOnFeedback;
        public MMFeedbacks _doorSoundFeedback;

        [MMReadOnly]
        public bool _firstOpen = false;

        BoxCollider2D _collider;

        protected virtual void InitializeFeedbacks()
        {
            _doorOnFeedback?.Initialization(this.gameObject); //!
            _doorOnFeedback?.Initialization(this.gameObject); //!
        }
        void Awake()
        {
            InitializeFeedbacks();

            _anim = GetComponent<Animator>();
            for (int i = 0;i < leversState.Length; i++)
            {
                leversState[i] = false;
            }
            _collider = this.GetComponent<BoxCollider2D>();
            _sprite = this.GetComponent<SpriteRenderer>();

            if (_closedDoor)
            {
                _anim.SetBool("isOpen", true);
                _collider.enabled = false;
            }
            else
            {
                _anim.SetBool("isOpen", false);
                if(_overLay != null)
                _overLay.SetBool("Blind", false);
                _collider.enabled = true;
            }
        }

        private void Start()
        {
            SetDoorStones();
            CheckForInteractive();
            if(mirrorDoor)
                _characterItemAbilities = GameManager.Instance.StoredCharacter.GetComponent<CharacterItemAbilities>();
            else if (tokenDoor)
                _inventoryInputManager = GUIManager.Instance._inventoryInputManager;
            else
                GetDoorStonesFeedbacks();
        }

        private void Update()
        {
            if (!mirrorDoor && !tokenDoor)
                return;
            else if(mirrorDoor && !mirrorActivated)
            {
                if(MirrorOwnerCheck())
                {
                    mirrorActivated = true;
                    OpenDoor();
                }
            }
            if(TokenGateCheck())
            {
                OpenDoor();
            }
        }

        protected virtual bool MirrorOwnerCheck()
        {
            _detectionCollider = null;
            _detectionCollider = Physics2D.OverlapCircle(transform.position, detectLength, LayerManager.PlayerLayerMask);

            if (_detectionCollider == null)
            {
                return false;
            }
            else if (_characterItemAbilities.mirrorAuthorized)
            {
                return true;
            }
            else return false;
        }

        protected virtual bool TokenGateCheck()
        {
            _detectionCollider = null;
            _detectionCollider = Physics2D.OverlapCircle(transform.position, detectLength, LayerManager.PlayerLayerMask);

            if (_detectionCollider == null)
            {
                if (doorWarning != null)
                {
                    doorWarning.warningScriptObject.SetActive(false);
                }
                return false;
            }
            else
            {
                findList = _inventoryInputManager.TargetAdditionalDisplay._targetInventory.InventoryContains(doorKey);
                if (findList.Count == 0)
                {
                    if(doorWarning !=null)
                    {
                        if(doorWarning.warningScriptObject.activeSelf == false)
                        {
                            doorWarning.warningScriptObject.SetActive(true);
                        }
                       
                    }
                    return false;
                }
                else
                {
                    tokenDoor = false;
                    return true;
                }
            }

        }

        public void SetOpenState()
        {
            if (_anim == null)
                _anim = GetComponent<Animator>();

            if (_collider == null)
                _collider = GetComponent<BoxCollider2D>();

            _anim.SetBool("isOpen", true);
            _collider.enabled = false;
        }

        public void SetCloseState()
        {
            if (_anim == null)
                _anim = GetComponent<Animator>();

            if (_collider == null)
                _collider = GetComponent<BoxCollider2D>();

            _anim.SetBool("isOpen", false);
            _collider.enabled = true;
        }

        public void OpenDoor()
        {
            //_collider.enabled = false;
            //_sprite.color = new Color(1, 1, 1, 0.5f);

            if (_firstOpen && openOneTime)
                return;

            _firstOpen = true;

            if(_firstOpen && openOneTime)
            {
                SaveInteractive();
            }

            for (int i = 0; i < _additoinalDoor.Length; i++)
            {
                _additoinalDoor[i].OpenDoor();
            }

            if (_mainCamera == null)
            {
                _anim.SetBool("isOpen", true);
                _doorOnFeedback?.PlayFeedbacks();

                CancelInvoke("WaitForOpen");
                Invoke("WaitForOpen", 2f);

                _collider.enabled = false;
                return;
            }

            _overLay.SetBool("Blind", true);

            _character = GameManager.Instance.StoredCharacter;
            _characterHealth = _character._health;

            _controller = _character.gameObject.GetComponent<CorgiController>();
            _horizontalMovement = _character.FindAbility<CharacterHorizontalMovement>();

            //_controller.SetForce(Vector2.zero);

            //_character._changingRoom = true;
            //_character.GetComponent<CharacterRewind>()._RewindHealth = _characterHealth.CurrentHealth;
            //_character.GetComponent<CharacterRewind>().StopRewind();

            //_character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);
            //_character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);

            _character.FindAbility<CharacterPause>().TriggerPause();

            _character.GetComponent<CharacterRewind>()._doorOpen = true;
            MMSoundManager.Instance.UnmuteSfx();
            GUIManager.Instance.SetPause(false);

            _coroutine = StartCoroutine(OpenDoorCameraMove());
            if (mirrorDoor || tokenDoor)
                return;

            int index=0;
            foreach(GameObject obj in doorStones)
            {
                if(obj.activeSelf)
                {
                    stoneFeedbacks[index].PlayFeedbacks();
                    obj.SetActive(false);
                }
                index++;
            }
        }

        protected virtual void SaveInteractive()
        {
            MaelstromDataManager.Instance.TestObject.InteractiveObjectList.Add(gameObject.name);
        }

        protected virtual void CheckForInteractive()
        {
            foreach (string name in MaelstromDataManager.Instance.TestObject.InteractiveObjectList)
            {
                if (name == gameObject.name)
                {
                    _collider.enabled = false;
                    _anim.SetBool("isOpen", true);
                    _firstOpen = true;
                    if(!mirrorDoor && !tokenDoor)
                    {
                        foreach (GameObject stone in doorStones)
                        {
                            stone.SetActive(false);
                        }
                    }
                    break;
                }
            }
        }


        private void WaitForOpen()
        {
            _doorOnFeedback?.StopFeedbacks();

            if (_triggerDoor)
                return;

            _doorOnFeedback?.PlayFeedbacks();
            _collider.enabled = true;
            _anim.SetBool("isOpen", false);

            CancelInvoke("WaitForClose");
            Invoke("WaitForClose", 2f);
        }

        private void WaitForClose()
        {
            _doorOnFeedback.StopFeedbacks();

            _firstOpen = false;
        }

        public void CloseDoor()
        {
            _doorOnFeedback?.PlayFeedbacks();
            _doorSoundFeedback?.PlayFeedbacks();
            _collider.enabled = true;
            _anim.SetBool("isOpen", false);

            CancelInvoke("WaitForClose");
            Invoke("WaitForClose", 2f);
        }



        IEnumerator OpenDoorCameraMove()
        {
            float curTime = 0;
            float moveSpeed = 0;

            Vector3 dirVec;
            bool _moveToDoor = true;
            bool _moveStop = false;

            Vector3 _worldTransform = this.transform.TransformPoint(Vector2.zero);
            _cameraTr = _mainCamera.transform.TransformPoint(Vector2.zero);

            dirVec = (new Vector3(_worldTransform.x, _worldTransform.y, 0) - new Vector3(_cameraTr.x, _cameraTr.y, 0)).normalized;

            _characterHealth.Invulnerable = true;

            GUIManager.Instance._inventoryInputManager.cutScene = true;

            while (true)
            {
                yield return new WaitForSecondsRealtime(0.02f);

                curTime += Time.unscaledDeltaTime;

                if (_moveToDoor && !_moveStop) //camera move to door
                {
                    dirVec = (new Vector3(_worldTransform.x, _worldTransform.y, 0) - new Vector3(_cameraTr.x, _cameraTr.y, 0)).normalized;

                    moveSpeed += Time.unscaledDeltaTime;

                    _mainCamera.transform.Translate(dirVec * moveSpeed * 0.3f);
                }
                else if (!_moveToDoor && !_moveStop) //camera move to character
                {
                    dirVec = (new Vector3(_character.transform.TransformPoint(Vector2.zero).x, _character.transform.TransformPoint(Vector2.zero).y, 0)
                        - new Vector3(_worldTransform.x, _worldTransform.y, 0)).normalized;

                    moveSpeed += Time.unscaledDeltaTime;

                    _mainCamera.transform.Translate(dirVec * moveSpeed * 0.5f);
                }


                if (_moveToDoor && !_moveStop) //change direction
                {
                    if (_character.transform.TransformPoint(Vector2.zero).x > this.transform.TransformPoint(Vector2.zero).x)
                    {
                        if (_mainCamera.transform.TransformPoint(Vector2.zero).x <= this.transform.TransformPoint(Vector2.zero).x)
                        {
                            _moveToDoor = false;
                            _moveStop = true;
                            _anim.SetBool("isOpen", true);
                            _doorOnFeedback?.PlayFeedbacks();
                            _doorSoundFeedback?.PlayFeedbacks();
                            moveSpeed = 0;
                            curTime = 0;
                        }
                    }
                    else if (_character.transform.TransformPoint(Vector2.zero).x < this.transform.TransformPoint(Vector2.zero).x)
                    {
                        if (_mainCamera.transform.TransformPoint(Vector2.zero).x >= this.transform.TransformPoint(Vector2.zero).x)
                        {
                            _moveToDoor = false;
                            _moveStop = true;
                            _anim.SetBool("isOpen", true);
                            _doorOnFeedback?.PlayFeedbacks();
                            _doorSoundFeedback?.PlayFeedbacks();
                            moveSpeed = 0;
                            curTime = 0;
                        }
                    }

                    if (_character.transform.TransformPoint(Vector2.zero).y > this.transform.TransformPoint(Vector2.zero).y)
                    {
                        if (_mainCamera.transform.TransformPoint(Vector2.zero).y <= this.transform.TransformPoint(Vector2.zero).y)
                        {
                            _moveToDoor = false;
                            _moveStop = true;
                            _anim.SetBool("isOpen", true);
                            _doorOnFeedback?.PlayFeedbacks();
                            _doorSoundFeedback?.PlayFeedbacks();
                            moveSpeed = 0;
                            curTime = 0;
                        }
                    }
                    else if (_character.transform.TransformPoint(Vector2.zero).y < this.transform.TransformPoint(Vector2.zero).y)
                    {
                        if (_mainCamera.transform.TransformPoint(Vector2.zero).y >= this.transform.TransformPoint(Vector2.zero).y)
                        {
                            _moveToDoor = false;
                            _moveStop = true;
                            _anim.SetBool("isOpen", true);
                            _doorOnFeedback?.PlayFeedbacks();
                            _doorSoundFeedback?.PlayFeedbacks();
                            moveSpeed = 0;
                            curTime = 0;
                        }
                    }
                }
                else if (!_moveToDoor && !_moveStop) //stop coroutine
                {
                    if (_character.transform.TransformPoint(Vector2.zero).x > this.transform.TransformPoint(Vector2.zero).x)
                    {
                        if (_mainCamera.transform.TransformPoint(Vector2.zero).x >= _character.transform.TransformPoint(Vector2.zero).x)
                        {
                            _mainCamera.transform.localPosition = Vector3.zero;
                            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
                            _overLay.SetBool("Blind", false);
                            _doorOnFeedback?.StopFeedbacks();
                            _doorSoundFeedback?.StopFeedbacks();
                            _characterHealth.Invulnerable = false;

                            _character.FindAbility<CharacterPause>().TriggerUnPause();
                            _character.GetComponent<CharacterRewind>()._doorOpen = false;
                            GUIManager.Instance._inventoryInputManager.cutScene = false;

                            StopCoroutine(_coroutine);
                            break;
                        }
                    }
                    else if (_character.transform.TransformPoint(Vector2.zero).x < this.transform.TransformPoint(Vector2.zero).x)
                    {
                        if (_mainCamera.transform.TransformPoint(Vector2.zero).x <= _character.transform.TransformPoint(Vector2.zero).x)
                        {
                            _mainCamera.transform.localPosition = Vector3.zero;
                            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
                            _overLay.SetBool("Blind", false);
                            _doorOnFeedback?.StopFeedbacks();
                            _doorSoundFeedback?.StopFeedbacks();
                            _character.FindAbility<CharacterPause>().TriggerUnPause();
                            _character.GetComponent<CharacterRewind>()._doorOpen = false;
                            GUIManager.Instance._inventoryInputManager.cutScene = false;
                            _characterHealth.Invulnerable = false;

                            StopCoroutine(_coroutine);
                            break;
                        }
                    }

                    if (_character.transform.TransformPoint(Vector2.zero).y > this.transform.TransformPoint(Vector2.zero).y)
                    {
                        if (_mainCamera.transform.TransformPoint(Vector2.zero).y >= _character.transform.TransformPoint(Vector2.zero).y)
                        {
                            _mainCamera.transform.localPosition = Vector3.zero;
                            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
                            _overLay.SetBool("Blind", false);
                            _doorOnFeedback?.StopFeedbacks();
                            _doorSoundFeedback?.StopFeedbacks();
                            _character.FindAbility<CharacterPause>().TriggerUnPause();
                            _character.GetComponent<CharacterRewind>()._doorOpen = false;
                            GUIManager.Instance._inventoryInputManager.cutScene = false;

                            _characterHealth.Invulnerable = false;
                            StopCoroutine(_coroutine);
                            break;
                        }
                    }
                    else if (_character.transform.TransformPoint(Vector2.zero).y < this.transform.TransformPoint(Vector2.zero).y)
                    {
                        if (_mainCamera.transform.TransformPoint(Vector2.zero).y <= _character.transform.TransformPoint(Vector2.zero).y)
                        {
                            _mainCamera.transform.localPosition = Vector3.zero;
                            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
                            _overLay.SetBool("Blind", false);
                            _doorOnFeedback?.StopFeedbacks();
                            _doorSoundFeedback?.StopFeedbacks();
                            _character.FindAbility<CharacterPause>().TriggerUnPause();
                            _character.GetComponent<CharacterRewind>()._doorOpen = false;
                            GUIManager.Instance._inventoryInputManager.cutScene = false;

                            _characterHealth.Invulnerable = false;
                            
                            StopCoroutine(_coroutine);
                            break;
                        }
                    }
                }

                if (_moveStop && curTime > 1.0f) //close door
                {
                    _collider.enabled = false;
                    //_sprite.color = new Color(1, 1, 1, 0.5f);
                }

                if (_moveStop && curTime > 1.3f) //return camera position
                {
                    _moveStop = false;
                }

            }
        }

        protected virtual void SetDoorStones()
        {
            switch (levers.Count)
            {
                case 1:
                    doorStones[0].SetActive(true);
                    break;
                case 2:
                    for (int i = 1; i < 3; i++)
                    {
                        doorStones[i].SetActive(true);
                    }
                    break;
                case 3:
                    for (int i = 0; i < 3; i++)
                    {
                        doorStones[i].SetActive(true);
                    }
                    break;
                case 4:
                    for (int i = 1; i < 4; i++)
                    {
                        doorStones[i].SetActive(true);
                    }
                    break;
                case 5:
                    foreach(GameObject stone in doorStones)
                    {
                        stone.SetActive(true);
                    }
                    break;
                default:
                    break;
            }

        }

        public virtual void GetDoorStonesFeedbacks()
        {
            foreach(GameObject stone in doorStones)
            {
                stoneFeedbacks.Add(stone.GetComponentInChildren<MMFeedbacks>());
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (!mirrorDoor && !tokenDoor)
                return;

            _gizmoColor.a = 0.75f;
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(transform.position, detectLength);
        }
    }
}