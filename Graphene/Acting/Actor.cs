using System;
using Graphene.Acting.Interfaces;
using UnityEngine;

namespace Graphene.Acting
{
    public abstract class Actor : MonoBehaviour, IDamageble {
    
        // protected CameraManagement Camera;

        protected Collider2D _collider;

        public int Id;
        
        public Life Life;
        public Stamina Stamina;

        public virtual void DoDamage(int damage, Vector3 from)
        {
            throw new NotImplementedException();
        }
    }
}
