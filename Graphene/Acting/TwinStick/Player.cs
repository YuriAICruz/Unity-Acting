using Graphene.Acting.Interfaces;
using Graphene.Inventory;
using Graphene.Inventory.Wearables;
using UnityEngine;

namespace Graphene.Acting.TwinStick
{
    [RequireComponent(typeof(TwinStickInputManager))]
    public class Player : TwinStickActor
    {
        private TwinStickInputManager _input;

        protected override void OnAwake()
        {
            _input = GetComponent<TwinStickInputManager>();
            _input.Init();
            OnEnabled();

            GenerateTempWeapons();
        }

        private void GenerateTempWeapons()
        {
            var wp = Resources.Load<GameObject>("Weapons/FixedMachineGun");

            LeftWeapon = Instantiate(wp, transform).GetComponent<IWeaponTopDown>();
            LeftWeapon.SetTip(new Vector3(-0.4f, 0, 0.2f));

            RightWeapon = Instantiate(wp, transform).GetComponent<IWeaponTopDown>();
            RightWeapon.SetTip(new Vector3(0.4f, 0, 0.2f));
        }

        private void OnDestroy()
        {
            _input.DeInit();
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            if (_input == null)
                return;

            _input.Left_Axis += Move;
            _input.Right_Axis += Aim;
            _input.Boost += Boost;
            _input.Cut += Cut;
            _input.ShootLHold += ShootLHold;
            _input.ShootLRelease += ShootLRelease;
            _input.ShootRHold += ShootRHold;
            _input.ShootRRelease += ShootRRelease;
        }

        private void ShootRHold()
        {
            RightWeapon.Use(transform.forward);
        }

        private void ShootRRelease()
        {
        }

        private void ShootLRelease()
        {
        }

        private void ShootLHold()
        {
            LeftWeapon.Use(transform.forward);
        }

        private void Cut()
        {
            throw new System.NotImplementedException();
        }

        private void Boost()
        {
            throw new System.NotImplementedException();
        }

        private void Aim(Vector2 dir)
        {
            _physics.Look(dir, _input.IsKeyboardMouse);
        }

        private void Move(Vector2 dir)
        {
            _physics.Move(dir);
        }
    }
}