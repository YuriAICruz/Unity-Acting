using System;
using System.Collections;
using UnityEngine;

namespace Graphene.Acting.SideScroller
{
    public class Player : SideScrollerActor
    {
        [SerializeField] private SideScrollerInputManager _input;
        public float JumpSpeed;
        public float JumpTime;
        public float WallJumpSpeed;
        private Coroutine _jumpreset;

        protected override void OnStart()
        {
            base.OnStart();

            if (_actorController.isLocalPlayer)
            {
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
            _input.JumpStart += JumpStart;
            _input.JumpEnd += JumpEnd;
        }

        private void JumpEnd()
        {
            _physics.Jump(false, 0, 0);
        }

        private void JumpStart()
        {
            _physics.Jump(true, JumpSpeed, WallJumpSpeed);

            if (_jumpreset != null)
                StopCoroutine(_jumpreset);

            _jumpreset = StartCoroutine(EndJumpRoutine());
        }

        public override void DoDamage(int damage, Vector3 from)
        {
            base.DoDamage(damage, from);

            _physics.Throw(transform.position - from, 20);
        }

        IEnumerator EndJumpRoutine()
        {
            yield return new WaitForSeconds(JumpTime * (_physics.Sliding ? 0.5f : 1));
            JumpEnd();
        }


        private void Move(Vector2 dir)
        {
            _physics.Move(dir, Speed);
            _animation.SetSpeed(_physics.Speed());
            _animation.SetSliding(_physics.Sliding);
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            if (_input == null)
                return;

            _input.Left_Axis -= Move;
            _input.JumpStart -= JumpStart;
            _input.JumpEnd -= JumpEnd;
        }
    }
}