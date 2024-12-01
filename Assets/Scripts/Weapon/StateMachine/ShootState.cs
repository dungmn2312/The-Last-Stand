using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunSpace
{
    public class ShootState : MonoBehaviour, IGunState
    {
        private GunController gunController;
        private Animator animator;

        public ShootState(GunController gunController)
        {
            this.gunController = gunController;
        }

        public void EnterState()
        {

        }

        public void UpdateState()
        {

        }

        public void ExitState()
        {

        }
    }
}


