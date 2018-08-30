using CameraSystem;
using Shooter;
using UnityEngine;

namespace Acting
{
    public class Actor : MonoBehaviour {
    
        protected CameraManagement Camera;

        protected Collider2D _collider;

        public int Id;
        
        public Life Life;
        public Stamina Stamina;
        

        protected virtual void OnTriggered(RaycastHit2D hit)
        {
        }

        protected virtual void OnCollided(RaycastHit2D hit)
        {
        }
    }
}
