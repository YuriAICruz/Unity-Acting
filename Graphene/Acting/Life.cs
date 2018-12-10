using System;
using UnityEngine;

namespace Graphene.Acting
{
    [Serializable]
    public class Life
    {
        [HideInInspector] public int Hp;
        public int MaxHp = 1;

        public event Action OnDie;

        public void Reset()
        {
            Hp = MaxHp;
        }
        
        public void RecoverHp(int life)
        {
            Hp += life;
            Hp = Math.Min(MaxHp, Hp);
        }

        public void ReceiveDamage(int damage)
        {
            // if (Hp <= 0) return;

            Hp -= damage;

            if (Hp <= 0)
            {
                if (OnDie != null) OnDie();
            }
        }
    }
}