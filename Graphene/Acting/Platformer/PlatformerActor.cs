using System.Collections;
using System.Collections.Generic;
using Graphene.Acting.ActorController;
using Graphene.Acting.Collectables;
using Graphene.Acting.Interfaces;
using Graphene.CameraManagement;
using Graphene.Inventory;
using Graphene.Inventory.Wearables;
using Graphene.Physics.Platformer;
using UnityEngine;

namespace Graphene.Acting.Platformer
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterPhysics))]
    public abstract class PlatformerActor : Actor
    {
        protected IActorController _actorController;

        protected CameraBehavior _camera;

        protected CharacterPhysics _physics;

        protected AnimationManager _animation;

        private Transform _target;

        public float Speed;
        [SerializeField] protected float _dodgeDuration;

        protected bool _canInteract = false;
        protected IInteractible _currentIntreactible;
        protected bool _canClear;
        
        protected IWeaponPlatformer _weapon;

        private void Awake()
        {
            _physics = GetComponent<CharacterPhysics>();

            _animation = new AnimationManager(GetComponent<Animator>());

            _weapon = transform.GetComponentInChildren<IWeaponPlatformer>();
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

            _camera = FindObjectOfType<CameraBehavior>();
            
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

            _physics.OnEdge += Jump;
            _physics.OnWallClose += TouchWall;
            _physics.JumpState += _animation.Jump;
            _physics.GroundState += _animation.SetGroundState;
            _physics.OnWallClimb += WallClimb;

            OnEnabled();
        }

        private void OnDisable()
        {
            if (_actorController == null || !_actorController.isLocalPlayer) return;

            _physics.OnEdge -= Jump;
            _physics.OnWallClose -= TouchWall;
            _physics.JumpState -= _animation.Jump;
            _physics.GroundState -= _animation.SetGroundState;
            _physics.OnWallClimb -= WallClimb;

            OnDisabled();
        }

        protected void Look(Vector2 dir)
        {
            if (dir.magnitude <= 0) return;

            if (_target != null)
            {
                Debug.DrawLine(transform.position, _target.position, Color.red);
                var tdir = _target.transform.position - transform.position;
                tdir.y = 0;
                transform.rotation = Quaternion.LookRotation(tdir);
                return;
            }

            var wdir = _camera.transform.TransformDirection(new Vector3(dir.x, 0, dir.y));
            wdir.y = 0;
            wdir.Normalize();

            transform.rotation = Quaternion.LookRotation(wdir);
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
            _physics.Jump(Speed);
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


        protected virtual void LockOff()
        {
            _target = null;
            _physics.SetTarget(_target);
        }

        protected virtual void LockOn()
        {
            var hits = UnityEngine.Physics.SphereCastAll(transform.position, 5, transform.forward, 5);

            foreach (var hit in hits)
            {
                var actor = hit.collider.GetComponent<Actor>();
                Debug.Log(actor);
                if (actor == null)
                {
                    var dmg = hit.collider.GetComponent<IDamageble>();

                    Debug.Log(dmg);
                    if (dmg == null) continue;

                    _target = hit.collider.transform;
                    _physics.SetTarget(_target);
                    return;
                }
                if (actor == this) continue;

                _target = actor.transform;
                _physics.SetTarget(_target);
            }
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