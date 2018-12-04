using System;

namespace Graphene.Acting.SideScroller
{
    [Serializable]
    public class SideScrollerInputManager : InputManager.InputSystem
    {
        public event Action JumpStart, JumpEnd;

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
            }
        }
    }
}