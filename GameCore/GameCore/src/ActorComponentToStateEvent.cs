using System;
using System.Collections.Generic;

public struct ActorComponentToStateEvent : IActorComponentEvent
{
    #region single
    private sealed class Singleton
    {
        private Singleton() { }
        internal static readonly ActorComponentToStateEvent Value = new ActorComponentToStateEvent();
    }

    public static ActorComponentToStateEvent Default
    {
        get { return Singleton.Value; }
    }
    #endregion
    public IStateEvent m_stateEvent;
}
