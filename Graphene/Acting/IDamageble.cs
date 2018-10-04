using UnityEngine;

namespace Graphene.Acting
{
    public interface IDamageble
    {
        void DoDamage(int damage, Vector3 from);
    }
}