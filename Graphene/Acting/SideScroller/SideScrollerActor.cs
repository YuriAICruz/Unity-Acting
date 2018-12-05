using Graphene.Acting.ActorController;
using Graphene.CameraManagement;
using Graphene.Physics.SideScroller;
using UnityEngine;

namespace Graphene.Acting.SideScroller
{
    [RequireComponent(typeof(CapsuleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class SideScrollerActor: Actor
    {
        protected IActorController _actorController;
        
        public float Speed;
        public float Gravity;

        protected CameraBehavior _camera;

        protected SideScrollerCharacterPhysics _physics;

        protected AnimationManager _animation;
        
        private void Awake()
        {
            var cap = GetComponent<CapsuleCollider2D>();
            _physics = new SideScrollerCharacterPhysics(GetComponent<Rigidbody2D>(), cap, Camera.main.transform, Gravity);

            _animation = new AnimationManager(GetComponent<Animator>());

            Life.Reset();
            Life.OnDie += OnDie;

            OnAwake();
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
        
        private void OnEnable()
        {
            if (_actorController == null || !_actorController.isLocalPlayer) return;

            _physics.OnWallClose += TouchWall;
            _physics.JumpState += _animation.Jump;
            _physics.GroundState += _animation.SetGroundState;

            OnEnabled();
        }

        private void OnDisable()
        {
            if (_actorController == null || !_actorController.isLocalPlayer) return;

            _physics.OnWallClose -= TouchWall;
            _physics.JumpState -= _animation.Jump;
            _physics.GroundState -= _animation.SetGroundState;

            OnDisabled();
        }

        private void TouchWall(int side)
        {
            //Debug.Log(side);
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

        protected virtual  void OnStart()
        {
        }


        protected virtual void OnDie()
        {   
        }
    }
}