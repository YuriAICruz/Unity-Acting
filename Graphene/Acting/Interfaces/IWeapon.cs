using UnityEngine;

namespace Graphene.Acting.Interfaces
{
    public interface IWeapon
    {
        void SetTip(Vector3 tip);
        void Use(Vector3 dir);
    }
}