using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class Photogene : MonoBehaviour
    {
        public float _outputInterval = 0.5f;

        private Character _character;
        private SpriteRenderer _modelSprite;
        private GameObject[] _photogene;

        private SpriteRenderer[] _sprite;
        private Animator[] _anim;

        private int _curNumber = 0;

        void Awake()
        {
            _photogene = GameObject.FindGameObjectsWithTag("Photogene");

            _character = GameManager.Instance.StoredCharacter;
            _modelSprite = _character.CharacterModel.gameObject.GetComponent<SpriteRenderer>();

            _sprite = new SpriteRenderer[_photogene.Length];
            _anim = new Animator[_photogene.Length];

            for (int i = 0; i < _photogene.Length; i++)
            {
                _sprite[i] = _photogene[i].GetComponent<SpriteRenderer>();
                _anim[i] = _photogene[i].GetComponent<Animator>();
            }

            _curNumber = 0;
        }

        void OnEnable()
        {
            Initialize();

            Invoke("PhotogeneStart", _outputInterval);
        }

        void Initialize()
        {
            for (int i = 0; i < _photogene.Length; i++)
            {
                _photogene[i].SetActive(false);
                _anim[i].SetBool("Blind", false);
            }

            CancelInvoke("PhotogeneStart");
            CancelInvoke("PhotogeneStop");

            _curNumber = 0;
        }

        public void PhotogeneStart()
        {
            
            if (_curNumber >= _photogene.Length || !this.gameObject.activeSelf)
            {
                //Initialize();
                return;
            }

            _photogene[_curNumber].SetActive(true);
            _photogene[_curNumber].transform.rotation = _character.CharacterModel.transform.rotation;
            _photogene[_curNumber].transform.position = _character.CharacterModel.transform.position;

            _sprite[_curNumber].sprite = _modelSprite.sprite;

            _curNumber++;

            Invoke("PhotogeneStart", _outputInterval);
        }

        public void PhotogeneStop()
        {
            if (_curNumber <= 0)
                return;

            _photogene[_curNumber].SetActive(true);
            _anim[_curNumber].SetBool("Blind", true);

            _curNumber--;

            Invoke("PhotogeneStop", 0.05f);
        }
    }

}

