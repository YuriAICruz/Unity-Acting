using System;
using UnityEngine;

namespace Graphene.Acting.Platformer
{
    public class DevPlayerWrapper : MonoBehaviour, IActorController
    {
        public bool isServer { get; private set; }
        public bool isClient { get; private set;}
        public bool isLocalPlayer { get; private set;}
        public event Action<Vector3> SetPosition;

        private void Awake()
        {
            isServer = true;
            isClient = true;
            isLocalPlayer = true;
        }
    }
}