using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;
    public int skinIndex;
    public ulong clientId;


    public bool Equals(PlayerData other)
    {
        return
            playerName == other.playerName &&
            playerId == other.playerId &&
            skinIndex == other.skinIndex &&
            playerId == other.playerId;
    }


    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref skinIndex);   
    }
}
