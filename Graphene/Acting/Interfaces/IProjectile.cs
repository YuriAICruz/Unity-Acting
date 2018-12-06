using UnityEngine;

namespace Graphene.Acting.Interfaces
{
    public interface IProjectile
    {
        bool Idle { get; set; }

        void Shoot(Vector3 pos, Vector3 dir, GameObject ignore = null);
    }
}