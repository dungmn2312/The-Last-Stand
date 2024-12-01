using UnityEngine;
using UnityEngine.InputSystem;

namespace GunSpace
{
    public class AimState : IGunState
    {
        private GunController gunController;
        private Animator animator;

        //private PlayerInputActions input;

        private int animPistolAim = Animator.StringToHash("pistol_aim");
        private int animRifleAim = Animator.StringToHash("rifle_aim");

        public AimState(GunController gunController)
        {
            this.gunController = gunController;
            animator = gunController.animator;
        }

        public void EnterState()
        {
            //input = InputManager.Instance.input;

            gunController.isAiming = true;

            animator?.SetBool(GetCurrentGunAnimAim(), true);

            gunController.gameObject.transform.localPosition = gunController.gunAimTransform.localPosition;
            gunController.gameObject.transform.localRotation = gunController.gunAimTransform.localRotation;

            CameraController.Instance.SetAimCamera(true);

            //input.Enable();
        }

        public void UpdateState()
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                gunController.ChangeState(gunController.idleState);
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (CameraController.Instance.gameObject.transform.localPosition == CameraController.Instance.aimCameraPos)
                {
                    gunController.Shoot();
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (gunController.currentAmmo <= 0) GunSoundManager.Instance.PlayEmptyMagazineSound();
            }
        }

        private int GetCurrentGunAnimAim()
        {
            return gunController.gunType == Gun.GunType.Pistol ? animPistolAim : animRifleAim;
        }

        public void ExitState()
        {
            gunController.isAiming = false;
            animator?.SetBool(GetCurrentGunAnimAim(), false);

            gunController.gameObject.transform.localPosition = gunController.gunIdleTransform.localPosition;
            gunController.gameObject.transform.localRotation = gunController.gunIdleTransform.localRotation;

            CameraController.Instance.SetAimCamera(false);

            //input.Disable();
        }
    }
}

