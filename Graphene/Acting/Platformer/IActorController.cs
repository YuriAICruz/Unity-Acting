using System;
using UnityEngine;

namespace Graphene.Acting.Platformer
{
    public interface IActorController
    {
        bool isServer { get; }
        bool isClient { get; }
        bool isLocalPlayer { get; }

        event Action<Vector3> SetPosition;
    }
}