using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

// This code defines a structure called PlayerData that implements the IEquatable<PlayerData> and INetworkSerializable interfaces.
//
// The PlayerData structure represents the data of a player in a game. It contains the following properties:
//
// - playerName: the name of the player, represented as a FixedString64Bytes.
// - clientId: the unique identifier of the player's client, represented as a ulong.
// - skinIndex: the index of the player's appearance, represented as an int.
// - color: the color of the player, represented as a Color.
// - playerId: the unique identifier of the player, represented as a FixedString64Bytes.
// - isPlayerReady: a boolean indicator that indicates whether the player is ready, represented as a bool.
//
// The implementation of IEquatable<PlayerData> allows comparing two instances of PlayerData to determine if they are equal.
// This is achieved by comparing all the properties of both instances.
//
// The implementation of INetworkSerializable indicates that the PlayerData structure can be serialized and deserialized for transmission over the network.
// The NetworkSerialize function is responsible for serializing and deserializing each of the properties of PlayerData using a BufferSerializer.
//
// This structure is necessary to store and transmit the data of a player in a multiplayer game.
// By implementing IEquatable<PlayerData>, it is easy to compare if two instances of PlayerData are equal.
// By implementing INetworkSerializable, the data of PlayerData can be sent and received over the network efficiently and reliably.
public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public FixedString64Bytes playerName;
    public ulong clientId;
    public int skinIndex;
    public Color color;
    public FixedString64Bytes playerId;
    public bool isPlayerReady;

    // Compares two instances of PlayerData to determine if they are equal.
    public readonly bool Equals(PlayerData other)
    {
        return
            playerName == other.playerName &&
            skinIndex == other.skinIndex &&
            color == other.color &&
            playerId == other.playerId &&
            isPlayerReady == other.isPlayerReady;
    }

    // Serializes and deserializes each of the properties of PlayerData using a BufferSerializer.
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref skinIndex);
        serializer.SerializeValue(ref color);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref isPlayerReady);
    }
}
