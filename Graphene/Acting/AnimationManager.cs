using UnityEngine;

namespace Graphene.Acting
{
    public class AnimationManager
    {
        private Animator _animator;

        public AnimationManager(Animator animator)
        {
            _animator = animator;
        }
        
        public void Interact()
        {
            _animator.SetTrigger("Interact");            
        }

        public void Attack()
        {
            _animator.SetTrigger("Attack");   
        }
        
        public void AttackSeq()
        {
            _animator.SetTrigger("Attack_Seq"); 
        }

        public void SetSpeed(float speed)
        {
            _animator.SetFloat("Speed", speed);   
        }

        public void Jump(bool state)
        {
            _animator.SetBool("Jumping", state);
        }

        public void SetGroundState(bool state)
        {
            _animator.SetBool("Grounded", state);   
        }

        public void TouchWall(Vector2Int dir)
        {
            _animator.SetFloat("WallX", dir.x);   
            _animator.SetFloat("WallY", dir.y);   
        }

        public void ReceiveDamage()
        {
            _animator.SetTrigger("Damage"); 
        }

        public void Die()
        {
            _animator.SetTrigger("Die");
        }

        public void Dodge()
        {
            _animator.SetTrigger("Dodge");
        }

        public void Climb(float height)
        {
            _animator.SetTrigger("Climb");
            _animator.SetFloat("ClimbHeight", height);
        }

        public void SetSliding(bool sliding)
        {
            _animator.SetBool("Sliding", sliding);
        }
    }
}