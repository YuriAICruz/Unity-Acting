using System;
using System.Collections;
using Graphene.InputManager;
using UnityEngine;

namespace Graphene.Acting.ThirdPersonShooter
{
    [RequireComponent(typeof(TpsInputManager))]
    public class Player : ThirdPersonAhooterActor
    {
        private TpsInputManager _input;

        protected override void OnStart()
        {
            base.OnStart();

            if (_actorController.isLocalPlayer)
            {
                if (_input == null)
                    _input = GetComponent<TpsInputManager>();

                _input.Init();

                OnEnabled();
            }

            _input.Right_Axis += _camera.Aim;
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            if (_input == null)
                return;

            _input.Left_Axis += Move;
            _input.Interact += Interact;
            _input.Attack += Attack;
            _input.AttackSeq += AttackSeq;
            _input.Jump += Jump;
            _input.Dodge += Dodge;
            _input.LockOn += LockOn;
            _input.LockOff += LockOff;
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            if (_input == null)
                return;

            _input.Left_Axis -= Move;
            _input.Interact -= Interact;
            _input.Attack -= Attack;
            _input.AttackSeq -= AttackSeq;
            _input.Jump -= Jump;
            _input.Dodge -= Dodge;
            _input.LockOn -= LockOn;
            _input.LockOff -= LockOff;
        }

        private void Dodge()
        {
            _animation.Dodge();
            _input.BlockInputs();
            _physics.Dodge(_dodgeDuration, Speed * 2, () => { _input.UnblockInputs(); });
        }

        private void Attack()
        {
            if (_canInteract) return;
            _animation.Attack();

            if (_weapon)
                _weapon.Use(0, 0.6f);
        }

        private void AttackSeq()
        {
            if (_canInteract) return;
            _animation.AttackSeq();

            if (_weapon)
                _weapon.Use(0.2f, 0.3f);
        }

        private void Interact()
        {
            if (!_canInteract) return;
            _animation.Interact();
        }

        private void Move(Vector2 dir)
        {
            var wdir = _camera.transform.TransformDirection(new Vector3(dir.x, 0, dir.y));
            wdir.y = 0;
            wdir.Normalize();
            
            _physics.Move(wdir, Speed);

            var f = _camera.transform.forward;
            f.y = 0;
            Look(f);

            _animation.SetSpeed(_physics.Speed());
        }

        protected override IEnumerator ReceiveDamage()
        {
            _input.BlockInputs();
            Move(Vector3.zero);
            yield return new WaitForSeconds(0.6f);
            _input.UnblockInputs();
        }
        
        protected virtual void LockOff()
        {
            throw new NotImplementedException();
        }

        protected virtual void LockOn()
        {
            throw new NotImplementedException();
        }
    }
}