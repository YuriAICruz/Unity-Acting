using Graphene.Acting.Interfaces;
using Graphene.Inventory;
using Graphene.Inventory.Wearables;
using Graphene.Physics.TwinStick;
using UnityEngine;

namespace Graphene.Acting.TwinStick
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class TwinStickActor : Actor
    {
        public ShipStatus ShipStatus;
        protected TwinStickShipPhysics _physics;

        public IWeaponTopDown LeftWeapon;
        public IWeaponTopDown RightWeapon;

        private void Awake()
        {
            _physics = new TwinStickShipPhysics(GetComponent<Rigidbody>(), ShipStatus);
            
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        private void OnEnable()
        {
            OnEnabled();
        }

        private void OnDisable()
        {
            OnDisabled();
        }

        protected virtual void OnEnabled()
        {
        }

        protected virtual void OnDisabled()
        {
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnStart()
        {
        }
    }
}