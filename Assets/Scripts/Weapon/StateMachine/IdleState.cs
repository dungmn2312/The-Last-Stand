using UnityEngine;
using UnityEngine.InputSystem;

namespace GunSpace
{
    public class IdleState : IGunState
    {
        private GunController gunController;
        private Animator animator;

        //private PlayerInputActions input;

        private int animPistolReload = Animator.StringToHash("pistol_reload");
        private int animRifleReload = Animator.StringToHash("rifle_reload");
        private int animPistolIdle = Animator.StringToHash("pistol_idle");
        private int animRifleIdle = Animator.StringToHash("rifle_idle");

        public IdleState(GunController gunController)
        {
            this.gunController = gunController;
            animator = gunController.animator;
        }

        public void EnterState()
        {
            //input = InputManager.Instance.input;
            //input.Enable();

            animator.SetBool(GetCurrentGunAnimIdle(), true);

            gunController.gunRes.SetActive(false);

            //input.Weapon.Reload.performed += ctx =>
            //{
            //    animator.SetTrigger(GetCurrentGunAnimReload());
            //    gunController.Reload();
            //};
            //input.Weapon.Switch1.started += ctx =>
            //{
            //    if (!gunController.isReloading)
            //    {
            //        gunController.SwitchGun(1);
            //    }
            //};
            //input.Weapon.Switch2.started += ctx =>
            //{
            //    if (!gunController.isReloading)
            //    {
            //        gunController.SwitchGun(2);
            //    }
            //};

        }

        public void UpdateState()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (!gunController.isReloading) gunController.ChangeState(gunController.aimState);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (!gunController.isReloading)
                {
                    gunController.SwitchGun(1);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (!gunController.isReloading)
                {
                    gunController.SwitchGun(2);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!gunController.isReloading && gunController.totalAmmo > 0 && gunController.currentAmmo < gunController.ammoPerMag)
                {
                    animator.SetTrigger(GetCurrentGunAnimReload());
                    gunController.PlayReloadSound();
                    gunController.Reload();
                }
            }
        }

        private int GetCurrentGunAnimReload()
        {
            return gunController.gunType == Gun.GunType.Pistol ? animPistolReload : animRifleReload;
        }

        private int GetCurrentGunAnimIdle()
        {
            return gunController.gunType == Gun.GunType.Pistol ? animPistolIdle : animRifleIdle;
        }

        public void ExitState()
        {
            //input.Disable();

            //input.Weapon.Reload.performed -= ctx =>
            //{
            //    animator.SetTrigger(GetCurrentGunAnimReload());
            //    gunController.Reload();
            //};

            //input.Weapon.Switch1.started -= ctx =>
            //{
            //    if (!gunController.isReloading)
            //    {
            //        gunController.SwitchGun(1);
            //    }
            //};
            //input.Weapon.Switch2.started -= ctx =>
            //{
            //    if (!gunController.isReloading)
            //    {
            //        gunController.SwitchGun(2);
            //    }
            //};


        }
    }
}