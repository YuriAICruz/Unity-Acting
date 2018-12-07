using Graphene.Acting.Interfaces;
using UnityEngine;

namespace Graphene.Packages.Acting.Graphene.Acting.SideScroller
{
    [RequireComponent(typeof(Collider2D))]
    public class SimpleCheckpoint : MonoBehaviour, ICheckpoint
    {
        private Collider2D _col;

        private void Awake()
        {
            _col = GetComponent<Collider2D>();
            _col.isTrigger = true;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}