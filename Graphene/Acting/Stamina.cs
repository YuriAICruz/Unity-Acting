using System;
using System.Collections;
using System.IO;
using Graphene.Utils;
using UnityEngine;

namespace Graphene.Acting
{
    [Serializable]
    public class Stamina
    {
        [HideInInspector] public float Stm;
        public float MaxStm = 1;
        public float RecoveryRate = 1;
        private Coroutine _routine;

        public event Action Stun; 

        public void Reset()
        {
            Stm = MaxStm;

            if (_routine == null)
                _routine = GlobalCoroutineManager.Instance.StartCoroutine(Recovery());
        }

        public bool DoAction(int cost)
        {
            if (Stm <= 0) return false;

            Stm -= cost;

            if (Stm <= 0)
                if (Stun != null) Stun();

            return true;
        }

        IEnumerator Recovery()
        {
            while (true)
            {
                if (Stm < MaxStm)
                    Stm += RecoveryRate * Time.deltaTime;
                        
                if (Stm > MaxStm)
                    Stm = MaxStm;

                yield return new WaitForChangedResult();
            }
        }
    }
}