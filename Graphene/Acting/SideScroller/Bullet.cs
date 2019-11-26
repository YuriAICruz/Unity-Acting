using System.Linq;
using Graphene.Acting.Interfaces;
using Graphene.Inventory;
using UnityEngine;

namespace Graphene
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Bullet : MonoBehaviour, IProjectile
    {
        [SerializeField] private float _radius;
        private Camera _cam;
        private Vector3 _dir;
        private Vector3 _lastPos;
        public int Damage;
        private Vector3 _iniLocPos;
        private Plane[] _planes;
        private GameObject _ignore;

        private SpriteRenderer _renderer;
        private Transform _parent;

        public bool Idle { get; set; }

        private void Start()
        {
            _cam = Camera.main;
            DisableBulet();
            _planes = GeometryUtility.CalculateFrustumPlanes(_cam);
            _iniLocPos = transform.localPosition;

            _renderer = GetComponent<SpriteRenderer>();

            _parent = transform.parent;
        }

        public void Shoot(Vector3 pos, Vector3 dir, GameObject ignore = null)
        {
            Idle = false;
            _dir = dir;
            _lastPos = transform.position = pos;
            _ignore = ignore;

            _renderer.flipX = _dir.x > 0;
            
            transform.SetParent(null);
        }

        private void Update()
        {
            if (Idle) return;

            var dist = (transform.position - _lastPos).magnitude;

            transform.Translate(_dir * Time.deltaTime);

            var hits = Physics2D.CircleCastAll(_lastPos, _radius, _dir, dist);

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    var go = hit.collider.gameObject;
                    if (go == gameObject || transform.IsChildOf(go.transform) || _ignore == go) continue;

                    var dmg = go.GetComponent<IDamageble>();
                    if (dmg != null)
                    {
                        dmg.DoDamage(Damage, transform.position);
                        DisableBulet();
                    }
                }
            }

            GeometryUtility.CalculateFrustumPlanes(_cam,_planes);
            if (!GeometryUtility.TestPlanesAABB(_planes, new Bounds(transform.position, Vector3.one * _radius)))
            {
                DisableBulet();
            }

            _lastPos = transform.position;
        }


        private void DisableBulet()
        {
            transform.position = Vector3.one*-9999;
            transform.SetParent(_parent);
            Idle = true;
        }
    }
}