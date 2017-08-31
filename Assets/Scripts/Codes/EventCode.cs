// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EventCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Events
{
    public enum EventCode
    {
        /// <summary>
        ///   Specifies that no event code is set.
        /// </summary>
        NoCodeSet = 0,

        /// <summary>
        ///   The event code for the <see cref="JoinEvent"/>.
        /// </summary>
        Join = 255,

        /// <summary>
        ///   The event code for the <see cref="LeaveEvent"/>.
        /// </summary>
        Leave = 254,

        /// <summary>
        ///   The event code for the <see cref="PropertiesChangedEvent"/>.
        /// </summary>
        PropertiesChanged = 253,

        /// <summary>
        /// The event code for the <see cref="DisconnectEvent"/>.
        /// </summary>
        Disconnect = 252,

        /// <summary>
        /// The event code for the <see cref="ErrorInfoEvent"/>.
        /// </summary>
        ErrorInfo = 251,

        CacheSliceChanged = 250,

        EventCacheSlicePurged = 249,
        GameList = 230,
        GameListUpdate = 229,
        QueueState = 228,
        AppStats = 226,
        GameServerOffline = 225,
        LobbyStats = 224,
    }
}