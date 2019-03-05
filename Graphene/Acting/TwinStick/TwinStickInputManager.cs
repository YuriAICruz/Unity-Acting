using System;

namespace Graphene.Acting.TwinStick
{
    [Serializable]
    public class TwinStickInputManager : InputManager.InputSystem
    {
        public event Action Boost, Cut, ShootLHold, ShootLRelease, ShootRHold, ShootRRelease;

        protected override void ExecuteCombo(int id)
        {
            switch (id)
            {
                case 10:
                    Boost?.Invoke();
                    break;
                case 15:
                    Cut?.Invoke();
                    break;
                case 20:
                    ShootLHold?.Invoke();
                    break;
                case 21:
                    ShootLRelease?.Invoke();
                    break;
                case 30:
                    ShootRHold?.Invoke();
                    break;
                case 31:
                    ShootRRelease?.Invoke();
                    break;
            }
        }
    }
}