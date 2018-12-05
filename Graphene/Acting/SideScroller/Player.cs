using System;
using UnityEngine;

namespace Graphene.Acting.SideScroller
{
    public class Player : SideScrollerActor
    {
        [SerializeField] 
        private SideScrollerInputManager _input;
        public float JumpSpeed;
        
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
            _physics.Jump(false, 0);
        }

        private void JumpStart()
        {
            _physics.Jump(true, JumpSpeed);
        }


        private void Move(Vector2 dir)
        {
            _physics.Move(dir, Speed);
            _animation.SetSpeed(_physics.Speed());
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