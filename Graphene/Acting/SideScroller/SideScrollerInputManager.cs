using System;
using UnityEngine;

namespace Graphene.Acting.SideScroller
{
    [Serializable]
    public class SideScrollerInputManager : InputManager.InputSystem
    {
        public event Action JumpStart, JumpEnd, DashStart, DashEnd, ShootHold, ShootRelease;

        protected override void ExecuteCombo(int id)
        {
            switch (id)
            {
                case 10:
                    JumpStart?.Invoke();
                    break;
                case 11:
                    JumpEnd?.Invoke();
                    break;
                case 20:
                    DashStart?.Invoke();
                    break;
                case 21:
                    DashEnd?.Invoke();
                    break;
                case 30:
                    ShootHold?.Invoke();
                    break;
                case 31:
                    ShootRelease?.Invoke();
                    break;
            }
        }
    }
}