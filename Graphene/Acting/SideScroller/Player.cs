using System;
using System.Collections;
using System.Linq;
using Graphene.Acting.Interfaces;
using UnityEngine;
using UnityEngine.Analytics;

namespace Graphene.Acting.SideScroller
{
    public class Player : SideScrollerActor
    {
        [SerializeField] private SideScrollerInputManager _input;
        public float JumpSpeed;
        public float JumpTime;
        public float WallJumpSpeed;
        private Coroutine _jumpreset;
        private float _dir = 1;

        public int BulletsCount;
        public int BulletsSpeed;
        private IProjectile[] _bullets;
        private IProjectile[] _chargedBullets;
        public string BulletResource = "Bullet_A";
        public string BulletChargedResource = "Bullet_A_Charged";
        public Vector3 BulletSpawn;

        public float DashSpeed;
        public float DashDuration;
        private bool _shootHolding;
        private float _shootHoldingTime;
        public float ChargeTime;

        private SpriteRenderer[] _renderers;
        private bool _charged;

        public float YKill;
        private Vector3 _respawnPosition;

        protected override void OnStart()
        {
            base.OnStart();

            SetRespawn(transform.position);

            _renderers = transform.GetComponentsInChildren<SpriteRenderer>();

            var res = Resources.Load<Bullet>(BulletResource);
            _bullets = new IProjectile[BulletsCount];

            for (int i = 0; i < BulletsCount; i++)
            {
                var tmp = Instantiate(res);
                _bullets[i] = tmp.GetComponent<IProjectile>();
            }

            res = Resources.Load<Bullet>(BulletChargedResource);
            _chargedBullets = new IProjectile[BulletsCount];

            for (int i = 0; i < BulletsCount; i++)
            {
                var tmp = Instantiate(res);
                _chargedBullets[i] = tmp.GetComponent<IProjectile>();
            }

            if (_actorController.isLocalPlayer)
            {
                _input.Init();
                OnEnabled();
            }

            _dir = 1;
            SetSide();
        }

        private void SetRespawn(Vector3 pos)
        {
            _respawnPosition = pos;
        }

        void SetSide()
        {
            var f = _dir > 0;
            foreach (var tr in _renderers)
            {
                tr.flipX = f;
            }
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            if (_input == null)
                return;

            _input.Left_Axis += Move;
            _input.JumpStart += JumpStart;
            _input.JumpEnd += JumpEnd;
            _input.DashStart += DashStart;
            _input.DashEnd += DashEnd;
            _input.ShootHold += ShootHold;
            _input.ShootRelease += ShootRelease;
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            if (_input == null)
                return;

            _input.Left_Axis -= Move;
            _input.JumpStart -= JumpStart;
            _input.JumpEnd -= JumpEnd;
            _input.DashStart -= DashStart;
            _input.DashEnd -= DashEnd;
            _input.ShootHold -= ShootHold;
            _input.ShootRelease -= ShootRelease;
        }

        protected override void OnDie()
        {
            base.OnDie();

            transform.position = _respawnPosition;
        }

        private void ShootHold()
        {
            _shootHolding = true;
            _shootHoldingTime = Time.time;
            _animation.Shooting(true);
        }

        private void ShootRelease()
        {
            if (!_shootHolding) return;

            _shootHolding = false;
            _animation.Shooting(false);

            Shoot();
        }

        IProjectile GetNextBullet()
        {
            foreach (var bullet in _charged ? _chargedBullets : _bullets)
            {
                if (bullet.Idle)
                    return bullet;
            }
            return null;
        }

        private void Shoot()
        {
            var b = GetNextBullet();

            var d = _physics.Sliding ? -_dir : _dir;
            b?.Shoot(transform.TransformPoint(new Vector3(BulletSpawn.x * d, BulletSpawn.y)), Vector3.right * d * BulletsSpeed, gameObject);
        }

        private void DashEnd()
        {
            _physics.DashStop();
            _animation.Dash(false);
        }

        private void DashStart()
        {
            if (!_charged) return;

            _shootHoldingTime = Time.time;

            _physics.Dash(_dir, DashSpeed, DashDuration);
            _animation.Dash(true);
        }

        private void JumpEnd()
        {
            _physics.Jump(false, 0, 0);
        }

        private void JumpStart()
        {
            if (!_physics.Grounded && !_physics.Sliding) return;

            _physics.Jump(true, JumpSpeed, WallJumpSpeed);

            if (_jumpreset != null)
                StopCoroutine(_jumpreset);

            _jumpreset = StartCoroutine(EndJumpRoutine());
        }

        public override void DoDamage(int damage, Vector3 from)
        {
            base.DoDamage(damage, from);

            _physics.Throw(transform.position - from, 20);
        }

        IEnumerator EndJumpRoutine()
        {
            yield return new WaitForSeconds(JumpTime * (_physics.Sliding ? 0.5f : 1));
            JumpEnd();
        }

        private void Move(Vector2 dir)
        {
            if (Mathf.Abs(dir.x) > 0)
            {
                _dir = Mathf.Sign(dir.x);
                SetSide();
            }

            _charged = Time.time - _shootHoldingTime >= ChargeTime && _shootHolding;

            _physics.Move(dir, Speed);
            _animation.Charged(_charged);
            _animation.SetSpeed(_physics.Speed());
            _animation.SetSliding(_physics.Sliding);

            if (transform.position.y < YKill)
            {
                OnDie();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var ch = other.GetComponent<ICheckpoint>();
            if (ch == null) return;

            SetRespawn(ch.GetPosition());
        }
    }

    internal interface ICheckpoint
    {
        Vector3 GetPosition();
    }
}