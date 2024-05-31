using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class CompressorManager : MonoBehaviour
    {
        private void OnTriggerStay2D(Collider2D collision)
        {
            GameObject rope = collision.gameObject;

            if (rope == null)
                return;

            if (!rope.activeSelf)
                return;

            if (rope.layer != 31)
                return;

            Debug.Log("Enter");
            rope.SetActive(false);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            GameObject rope = collision.gameObject;

            if (rope == null)
                return;

            if (rope.activeSelf)
                return;

            if (rope.layer != 31)
                return;

            Debug.Log("Exit");
            rope.SetActive(true);
        }
    }

}

