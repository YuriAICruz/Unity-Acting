using UnityEngine;

namespace Graphene.Acting.Interfaces
{
    public interface IDamageble
    {
        void DoDamage(int damage, Vector3 from);
    }
}