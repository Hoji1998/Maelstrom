using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class AutoDestroy : MonoBehaviour
    {
        public float _destroyTime = 0.5f;
        // Start is called before the first frame update
        private void OnEnable()
        {
            Destroy(this.gameObject, _destroyTime);
        }
    }

}

