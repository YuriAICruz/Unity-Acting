using System;
using UnityEngine;

namespace Graphene.Acting.SideScroller
{
    public class Player : SideScrollerActor
    {
        [SerializeField] 
        private SideScrollerInputManager _input;
        
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
            _physics.Jump(false);
        }

        private void JumpStart()
        {
            _physics.Jump(true);
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