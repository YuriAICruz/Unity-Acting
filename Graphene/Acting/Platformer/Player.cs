using System.Collections;
using Graphene.Acting;
using Graphene.Acting.Collectables;
using Graphene.InputManager;
using Graphene.InputManager.Platformer;
using UnityEngine;

namespace Graphene.Acting.Platformer
{
    public class Player : PlatformerActor
    {
        private PlatformerInputManager _input;

        protected override void OnStart()
        {
            base.OnStart();
            
            if (_actorController.isLocalPlayer)
            {
                _input = GetComponent<PlatformerInputManager>();
                _input.Init();
                OnEnabled();
            }
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
            _physics.Dodge(_dodgeDuration, Speed*2, () =>
            {
                _input.UnblockInputs();
            });
        }

        private void Attack()
        {
            if (_canInteract) return;
            _animation.Attack();
            _weapon.Use(0,0.6f);
        }

        private void AttackSeq()
        {
            if (_canInteract) return;
            _animation.AttackSeq();
            _weapon.Use(0.2f,0.3f);
        }

        private void Interact()
        {
            if (!_canInteract) return;
            _animation.Interact();
        }

        private void Move(Vector2 dir)
        {
            _physics.Move(dir, Speed);

            Look(dir);

            _animation.SetSpeed(_physics.Speed());
        }

        protected override IEnumerator ReceiveDamage()
        {
            _input.BlockInputs();
            Move(Vector2.zero);
            yield return new WaitForSeconds(0.6f);
            _input.UnblockInputs();
        }
    }
}