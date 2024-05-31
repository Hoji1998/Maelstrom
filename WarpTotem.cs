using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks; //!
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class WarpTotem : MonoBehaviour
    {
        [Header("Warp")]
        public WarpManager _warpTotemCanvas;
        public GameObject _warpTotemUi;
        public MMFeedbacks TotemOnFireFeedback;  //!
        public MMFeedbacks TotemFiringFeedback; //!
        public MMFeedbacks TotemPressFeedback;

        [Header("Icon Number")]
        public int _iconNumber;

        [Header("Marker")]
        public GameObject marker;

        private bool FirstTime = true; //!

        protected InputManager _inputmanager;
        protected Character _character;
        public Animator _anim;
        protected MiniMapControl _miniMap;
        protected Teleporter _teleporter;

        protected virtual void InitializeFeedbacks()
        {
            TotemOnFireFeedback?.Initialization(this.gameObject); //!
            TotemFiringFeedback?.Initialization(this.gameObject); //!
            TotemPressFeedback?.Initialization(this.gameObject);
        }

        protected virtual void Awake()
        {
            InitializeFeedbacks();  //!
            _inputmanager = InputManager.Instance;
            if(_anim == null)
                _anim = this.GetComponent<Animator>();
        }

        void Start()
        {
            _miniMap = _warpTotemCanvas._minimap;
            _teleporter = this.GetComponent<Teleporter>();
            LoadWarpTotemInformation();
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            _character = collider.GetComponent<Character>();

            if (_character == null)
            {
                return;
            }

            if (_character.CharacterType != Character.CharacterTypes.Player)
            {
                return;
            }

            HandledInput();
        }

        void HandledInput()
        {
            if (_inputmanager.PrimaryMovement.y >= 0.9f && !_warpTotemCanvas._warpTotemOn)
            {
                if (_character.MovementState.CurrentState == CharacterStates.MovementStates.LookingUp)
                {
                    CallFeedBack(); //!
                    _warpTotemCanvas._warpTotemOn = true;
                    _character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
                    _character.GetComponent<CorgiController>().SetForce(Vector2.zero);

                    _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);

                    WarpTotemOn();
                }
                else if (_character.MovementState.CurrentState == CharacterStates.MovementStates.Idle)
                {
                    TotemPressFeedback?.PlayFeedbacks(this.transform.position);
                }
                
            }
        }

        public void WarpTotemOn()
        {
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);

            _warpTotemUi.SetActive(true);
            _anim.SetBool("TotemOn", true);
            _character.isPause = true;
            _character._totemOn = true;
            _miniMap.OpenMiniMap();

            _character._changingRoom = true;

            _warpTotemCanvas._currentTeleporter = _teleporter;
        }

        public void CallFeedBack() //!
        {
            if (FirstTime)
            {
                TotemOnFireFeedback?.PlayFeedbacks(this.transform.position); //!
                FirstTime = false;
                //WarpSave
                SaveWarpTotemInformation();
                _warpTotemCanvas.MarkerLengthUpdate(_iconNumber, _teleporter);

                if (marker != null)
                {
                    marker.SetActive(true);
                }
            }
            else
            {
                TotemFiringFeedback?.PlayFeedbacks(this.transform.position); //!
            }
        }

        public void SaveWarpTotemInformation()
        {
            MaelstromDataManager.Instance.TestObject.InteractiveObjectList.Add(gameObject.name);
        }

        public void LoadWarpTotemInformation()
        {
            foreach (string name in MaelstromDataManager.Instance.TestObject.InteractiveObjectList)
            {
                if (name == gameObject.name)
                {
                    _anim.SetBool("TotemOn", true);
                    FirstTime = false;
                    _warpTotemCanvas.MarkerLengthUpdate(_iconNumber, _teleporter);

                    if (marker != null)
                    {
                        marker.SetActive(true);
                    }
                    break;
                }
            }
        }
    }
}


