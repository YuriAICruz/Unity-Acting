using System;
using System.Collections;
using System.Linq;
using Graphene.Acting.Collectables;
using Graphene.Acting.Interfaces;
using Graphene.Packages.Acting.Graphene.Acting.SideScroller;
using UnityEngine;

namespace Graphene.Acting.SideScroller
{
    [RequireComponent(typeof(SideScrollerInputManager))]
    public class Player : SideScrollerActor
    {
        public Action<Vector3> OnSetWayPoint;

        private SideScrollerInputManager _input;
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
        public float ChargeReboundTime;
        public float ChargeReboundDuration;

        private SpriteRenderer[] _renderers;
        private bool _charged;

        public float YKill;
        private Vector3 _respawnPosition;
        private Coroutine _rebound;
        private bool _dash, _dashButtom;
        private bool _dead;
        private HpContainer[] _collectables;

        protected override void OnStart()
        {
            base.OnStart();
            
            _camera.BlockScene += BlockScene;
            _camera.UnblockScene += UnblockScene;

            _collectables = FindObjectsOfType<HpContainer>();

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
                _input = GetComponent<SideScrollerInputManager>();
                _input.Init();
                OnEnabled();
            }

            _dir = 1;
            SetSide();
        }

        private void BlockScene()
        {
            _physics.Block(true);
        }

        private void UnblockScene()
        {
            _physics.Block(false);
        }

        private void SetRespawn(Vector3 pos)
        {
            _respawnPosition = pos;
            OnSetWayPoint?.Invoke(pos);
        }

        private void SetSide()
        {
            var f = _dir > 0;
            if (_physics.Sliding)
                f = !f;
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

            _dead = true;

            foreach (var collectable in _collectables)
            {
                collectable.gameObject.SetActive(true);
            }

            StartCoroutine(DeadRoutine());
        }

        IEnumerator DeadRoutine()
        {
            yield return new WaitForSeconds(0.6f);

            _dead = false;
            Respawn();
        }

        private void Respawn()
        {
            transform.position = _respawnPosition;
            Life.Reset();
        }

        private void ShootHold()
        {
            _shootHolding = true;
            _shootHoldingTime = Time.time;
            _animation.Shooting(true);
            Sfx.Charging(true);
        }

        private void ShootRelease()
        {
            if (!_shootHolding) return;

            _shootHolding = false;
            _animation.Shooting(false);
            Sfx.Charging(false);

            Shoot();
        }

        private IProjectile GetNextBullet()
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

            if (b != null)
                Sfx.Shoot();
        }

        private void DashEnd()
        {
            _dashButtom = false;

            if (!_dash) return;

            _dash = false;

            if (_rebound != null)
                StopCoroutine(_rebound);
            _rebound = StartCoroutine(Rebound());

            _physics.DashStop();
        }

        private void DashStart()
        {
            _dashButtom = true;

            if (!_charged || _dash) return;

            _dash = true;

            _shootHoldingTime = Time.time;

            if (_rebound != null)
                StopCoroutine(_rebound);

            _rebound = StartCoroutine(Rebound(true));

            _physics.Dash(_dir, DashSpeed, DashDuration);
        }

        IEnumerator Rebound(bool init = false)
        {
            if (init)
                yield return new WaitForSeconds(DashDuration);

            var t = 0f;
            while (t < ChargeReboundTime - ChargeReboundDuration * 0.5f)
            {
                t += Time.deltaTime;
                yield return null;
            }

            t = 0;
            var mapShoot = !_shootHolding;
            var mapDash = !_dashButtom;

            _animation.CanRebound(false);
            Sfx.CanRebound(false);

            while (t < ChargeReboundDuration)
            {
                if (!_shootHolding)
                    mapShoot = true;
                if (!_dashButtom)
                    mapDash = true;

                _animation.CanRebound(true);
                Sfx.CanRebound(true);
                if (mapShoot && _shootHolding || mapDash && _dashButtom)
                {
                    if (_dashButtom && mapDash)
                    {
                        _charged = true;
                        DashStart();
                    }
                    else
                    {
                        _shootHoldingTime = Time.time - ChargeTime;
                        _shootHolding = true;
                    }

                    yield break;
                }

                t += Time.deltaTime;
                yield return null;
            }

            _animation.CanRebound(false);
            Sfx.CanRebound(false);
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

            if (_rebound != null)
                StopCoroutine(_rebound);

            _shootHolding = false;

            _physics.Throw(transform.position - from, 20);
        }

        private IEnumerator EndJumpRoutine()
        {
            yield return new WaitForSeconds(JumpTime * (_physics.Sliding ? 0.5f : 1));
            JumpEnd();
        }

        private void Move(Vector2 dir)
        {
            if (_dead) return;

            if (Mathf.Abs(dir.x) > 0)
            {
                _dir = Mathf.Sign(dir.x);
                SetSide();
            }

//            if(transform.eulerAngles.magnitude > 0)
//                transform.eulerAngles = Vector3.zero;

            _charged = Time.time - _shootHoldingTime >= ChargeTime && _shootHolding;

            _physics.Move(dir, Speed);
            _animation.Charged(_charged);
            Sfx.Charged(_charged);
            _animation.SetSpeed(_physics.Speed());
            Sfx.SetSpeed(_physics.Speed());
            _animation.SetSliding(_physics.Sliding);
            Sfx.SetSliding(_physics.Sliding);
            _animation.Dash(_physics.Dashing);
            Sfx.Dash(_physics.Dashing);

            if (transform.position.y < YKill)
            {
                OnDie();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var ch = other.GetComponent<ICheckpoint>();
            if (ch == null)
            {
                var hp = other.GetComponent<HpContainer>();


                if (hp != null)
                {
                    Life.RecoverHp(hp.Hp);
                    hp.gameObject.SetActive(false);
                    Sfx.Collect();
                }

                return;
            }

            SetRespawn(ch.GetPosition());
        }
    }
}