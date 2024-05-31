using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MoreMountains.CorgiEngine
{
    public class LazerTrap : MonoBehaviour
    {
        Animator _anim;

        private void Start()
        {
            _anim = this.GetComponent<Animator>();

            _anim.SetBool("AttackOn", true);         
        }
    }


}

