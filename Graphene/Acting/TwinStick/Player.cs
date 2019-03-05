using UnityEngine;

namespace Graphene.Acting.TwinStick
{
    public class Player : TwinStickActor
    {
        [SerializeField] private TwinStickInputManager _input;

        protected override void OnAwake()
        {
            _input.Init();
            OnEnabled();
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
            throw new System.NotImplementedException();
        }

        private void ShootRRelease()
        {
            throw new System.NotImplementedException();
        }

        private void ShootLRelease()
        {
            throw new System.NotImplementedException();
        }

        private void ShootLHold()
        {
            throw new System.NotImplementedException();
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
            _physics.Look(dir);
        }

        private void Move(Vector2 dir)
        {
            _physics.Move(dir);
        }
    }
}