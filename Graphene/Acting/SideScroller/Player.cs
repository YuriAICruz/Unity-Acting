using System;
using System.Collections;
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
        public string BulletResource = "Bullet_A";
        public Vector3 BulletSpawn;

        public float DashSpeed, DashDuration;
        private bool _shootHoolding;

        protected override void OnStart()
        {
            base.OnStart();

            var res = Resources.Load<Bullet>(BulletResource);
            _bullets = new IProjectile[BulletsCount];

            for (int i = 0; i < BulletsCount; i++)
            {
                var tmp = Instantiate(res);
                _bullets[i] = tmp.GetComponent<IProjectile>();
            }

            if (_actorController.isLocalPlayer)
            {
                _input.Init();
                OnEnabled();
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

        private void ShootHold()
        {
            _shootHoolding = true;
        }

        private void ShootRelease()
        {
            if(!_shootHoolding) return;
            _shootHoolding = false;
            Shoot();
        }

        IProjectile GetNextBullet()
        {
            foreach (var bullet in _bullets)
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
        }

        private void DashStart()
        {
            _physics.Dash(_dir, DashSpeed, DashDuration);
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
            }
            _physics.Move(dir, Speed);
            _animation.SetSpeed(_physics.Speed());
            _animation.SetSliding(_physics.Sliding);
        }
    }
}