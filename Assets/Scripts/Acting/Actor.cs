using UnityEngine;

namespace Graphene.Acting
{
    public abstract class Actor : MonoBehaviour, IDamageble {
    
        // protected CameraManagement Camera;

        protected Collider2D _collider;

        public int Id;
        
        public Life Life;
        public Stamina Stamina;

        protected virtual void OnTriggered(RaycastHit2D hit)
        {
            throw new System.NotImplementedException();
        }

        protected virtual void OnCollided(RaycastHit2D hit)
        {
            throw new System.NotImplementedException();
        }

        public virtual void DoDamage(int damage)
        {
            throw new System.NotImplementedException();
        }
    }
}
