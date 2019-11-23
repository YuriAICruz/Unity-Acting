using System.Security.Cryptography.X509Certificates;
using Graphene.Acting.Interfaces;
using UnityEngine;

namespace Graphene.Acting.FirsPerson
{
    public abstract class Gun : MonoBehaviour
    {
        private IDamageble _owner;

        public int MaxBullets = 6;
        private int _bullets = 0;
        public float maxDistance = 100;

        public LayerMask mask;

        public Transform Tip;

        public void SetOwner(IDamageble owner)
        {
            _owner = owner;
            Reload();
        }

        public void Use(Ray ray)
        {
            if (_bullets <= 0) return;

            _bullets--;

            RaycastHit hit;

            if (UnityEngine.Physics.Raycast(ray, out hit, maxDistance, mask))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 10);
                Debug.DrawLine(Tip.position, hit.point, Color.green, 10);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, 10);
                Debug.DrawLine(Tip.position, ray.GetPoint(maxDistance), Color.red, 10);
            }
        }

        public void Reload()
        {
            _bullets = MaxBullets;
        }
    }
}