using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    public class Rock : MonoBehaviour
    {
        public float Vec;
        public float power = 0.1f;
        public GameObject rockImage;

        [Header("GroundFeedback")]
        public MMFeedbacks GroundFeedback;

        private CorgiController _controller;

        private void Start()
        {
            _controller = this.GetComponent<CorgiController>();
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.gameObject.layer != 8 && collider.gameObject.layer != 11)
                return;
            if (Vec == 0)
            {
                _controller.SetForce(Vector2.right * power);
            }
            else if(Vec == 1)
            {
                _controller.SetForce(Vector2.left * power);
            }
        }

        private void FixedUpdate()
        {
            if (_controller.State.IsGrounded)
            {
                GroundFeedback.PlayFeedbacks();
            }

            if (Vec == 0)
            {
                rockImage.transform.Rotate(new Vector3(0, 0, -5f));
            }
            else if (Vec == 1)
            {
                rockImage.transform.Rotate(new Vector3(0, 0, 5f));
            }
        }
    }
}


