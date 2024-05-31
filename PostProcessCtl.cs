using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class PostProcessCtl : MonoBehaviour
    {
        protected Vignette _vignette;
        public Volume _volume;
        protected Character _character;
        public float _value;

        
        private void Start()
        {
            _volume.profile.TryGet(out _vignette);

            _value = 0.8f;
            //_character = GameManager.Instance.StoredCharacter;
        }
        private void Update()
        {
            if (!Authorized())
            {
                return;
            }

            if (_character._health._stillKnockback)
            {
                VignetteControl(_value);
                _value -= _value * 0.03f * Time.timeScale;
            }
            else
            {
                VignetteControl(0f);
                _value = 0.8f;
            }
            
        }

        private bool Authorized()
        {
            if (_character == null)
            {
                _character = GameManager.Instance.StoredCharacter;
            }

            return true;
        }


        public void VignetteControl(float _value)
        {
            _vignette.intensity.value = _value;
        }
        
    }
}
