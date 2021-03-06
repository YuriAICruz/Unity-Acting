﻿using System;
using System.Collections;
using Graphene.Acting.ActorController;
using Graphene.Acting.Collectables;
using Graphene.Acting.Interfaces;
using Graphene.Acting.Platformer;
using Graphene.CameraManagement;
using Graphene.Inventory;
using Graphene.Inventory.Wearables;
using Graphene.Physics.Platformer;
using UnityEngine;
using BasicPhysics = Graphene.Physics.ThirdPersonShooter.BasicPhysics;

namespace Graphene.Acting.ThirdPersonShooter
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class ThirdPersonAhooterActor : Actor
    {
        protected IActorController _actorController;

        protected ShooterCameraBehavior _camera;

        protected BasicPhysics _physics;

        protected AnimationManager _animation;

        public float Speed = 3;
        public float JumpForce = 2;

        [SerializeField] protected float _dodgeDuration;

        protected bool _canInteract = false;
        protected IInteractible _currentIntreactible;
        protected bool _canClear;
        
        protected IWeaponPlatformer _weapon;

        private void Awake()
        {
            _physics = GetComponent<BasicPhysics>();

            _animation = new AnimationManager(GetComponent<Animator>());

            _weapon = transform.GetComponentInChildren<WeaponPlatformer>();
            ((IWearable)_weapon)?.SetOwner(this);
            
            Life.Reset();
            Life.OnDie += OnDie;

            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        private void Start()
        {
            _actorController = Utils.InterfaceHelper.GetInterfaceComponent<IActorController>(this);

            if (_actorController == null)
            {
                Debug.LogError("No IActorController found");
                gameObject.SetActive(false);
                return;
            }

            if (_actorController.isLocalPlayer)
            {
                OnEnable();
            }

            _camera = FindObjectOfType<ShooterCameraBehavior>();

            if (_camera == null)
            {
                Debug.LogError("No CameraBehavior found");
                return;
            }

            if (_actorController.isLocalPlayer)
                _camera.SetTarget(this.transform);

            OnStart();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnEnabled()
        {
        }

        protected virtual void OnDisabled()
        {
        }


        private void OnEnable()
        {
            if (_actorController == null || !_actorController.isLocalPlayer) return;

            _physics.JumpState += _animation.Jump;
            _physics.GroundState += _animation.SetGroundState;

            OnEnabled();
        }

        private void OnDisable()
        {
            if (_actorController == null || !_actorController.isLocalPlayer) return;

            _physics.JumpState -= _animation.Jump;
            _physics.GroundState -= _animation.SetGroundState;

            OnDisabled();
        }

        protected void Look(Vector3 dir)
        {
            if (dir.magnitude <= 0) return;
//
//            var wdir = _camera.transform.TransformDirection(new Vector3(dir.x, 0, dir.y));
//            wdir.y = 0;
//            wdir.Normalize();

            transform.rotation = Quaternion.LookRotation(dir);
        }

        private void OnTriggerEnter(Collider other)
        {
            var col = other.transform.GetComponent<ICollectable>();
            if (col != null)
            {
                col.Collect(this);
            }

            var intreactible = other.transform.GetComponent<IInteractible>();

            if (intreactible != null)
            {
                _currentIntreactible = intreactible;
                _canClear = false;
            }
        }

        private void TouchWall(int side)
        {
            _animation.TouchWall(
                new Vector2Int(
                    side % 2 * ((side + 1) % 2 - side + 2),
                    (side + 1) % 2 * Mathf.Min(1, side) * ((side + 1) % 2 - side + 2))
            );
        }

        protected virtual void Jump()
        {
            _physics.Jump(JumpForce);
        }

        public override void DoDamage(int damage, Vector3 from)
        {
            StartCoroutine(ReceiveDamage());
            Life.ReceiveDamage(damage);
            _animation.ReceiveDamage();
        }

        protected virtual IEnumerator ReceiveDamage()
        {
            yield break;
        }

        protected virtual void OnDie()
        {
            Debug.Log(gameObject + "Died");
            _animation.Die();
        }

        protected virtual void WallClimb(float height)
        {
            _physics.Climb(height, Speed);
            _animation.Climb(height);
        }


        private void OnTriggerExit(Collider other)
        {
        }

        private void OnCollisionEnter(Collision other)
        {
            var col = other.transform.GetComponent<ICollectable>();
            if (col != null)
            {
                col.Collect(this);
            }
        }

        private void OnCollisionExit(Collision other)
        {
        }

        public void FootR()
        {
        }

        public void FootL()
        {
        }

        public void Hit()
        {
        }

        public void Land()
        {
        }
    }
}