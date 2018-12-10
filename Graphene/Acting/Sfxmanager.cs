using System;
using UnityEngine;

namespace Graphene.Acting
{
    [Serializable]
    public class Sfxmanager
    {
        public AudioClip JumpClip, SlidingClip, ReboundClip, DashClip, ShootClip, DamageClip, DieClip, ChargeClip, CollectClip;

        public AudioSource Source;
        private bool _lastSliding;
        private bool _lastCharged;
        private bool _lastCharging;

        void Play(AudioClip clip)
        {
            if (Source.isPlaying)
                Source.Stop();

            Source.clip = clip;
            Source.Play();
        }

        public void Interact()
        {
            throw new NotImplementedException();
        }

        public void Attack()
        {
            throw new NotImplementedException();
        }

        public void AttackSeq()
        {
            throw new NotImplementedException();
        }

        public void SetSpeed(float speed)
        {
        }

        public void Jump(bool state)
        {
            if (state)
                Play(JumpClip);
        }

        public void SetGroundState(bool state)
        {
        }

        public void TouchWall(Vector2Int dir)
        {
        }

        public void ReceiveDamage()
        {
            Play(DamageClip);
        }

        public void Die()
        {
            Play(DieClip);
        }

        public void Dodge()
        {
            throw new NotImplementedException();
        }

        public void Climb(float height)
        {
            throw new NotImplementedException();
        }

        public void SetSliding(bool sliding)
        {
            if (sliding && _lastSliding != sliding)
                Play(SlidingClip);

            _lastSliding = sliding;
        }

        public void Shoot()
        {
            Play(ShootClip);
        }

        public void Dash(bool state)
        {
            if (state)
                Play(DashClip);
        }

        public void Charging(bool state)
        {
            if (state && _lastCharging != state)
                Play(ChargeClip);

            _lastCharging = state;
        }

        public void Charged(bool state)
        {
            if (state && _lastCharged != state)
                Play(SlidingClip);

            _lastCharged = state;
        }

        public void CanRebound(bool state)
        {
            if (state)
                Play(ReboundClip);
        }

        public void Collect()
        {
                Play(CollectClip);
        }
    }
}