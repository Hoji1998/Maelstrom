using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class ActiveButton : MonoBehaviour
    {
        public int order = -1;
        public bool select = false;
        

        private int number = -1;
        private bool active = false;
        private Teleporter teleporter;

        public void SetNumber(int number)
        {
            this.number = number;
        }

        public void SetButtonActive(bool active)
        {
            this.active = active;
        }

        public void SetTeleporter(Teleporter teleporter)
        {
            this.teleporter = teleporter;
        }

        public int GetNumber()
        {
            return this.number;
        }

        public bool GetButtonActive()
        {
            return this.active;
        }

        public Teleporter GetTeleporter()
        {
            return this.teleporter;
        }
    }
}
