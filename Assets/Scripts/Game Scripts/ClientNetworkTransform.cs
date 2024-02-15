using Unity.Netcode.Components;
using UnityEngine;

// This namespace is necessary to provide a clear and organized structure for the classes and components related to client authority functionality in multiplayer games.
// It helps to group together all the related code and makes it easier to understand and maintain.
// The classes and components within this namespace are specifically designed for syncing a transform with client-side changes, including the host.
// It is important to note that this functionality does not support pure server as owner, and it is recommended to use NetworkTransform for transforms that will always be owned by the server.
namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{
    // Used for syncing a transform with client side changes. This includes host. Pure server as owner isn't supported by this. Please use NetworkTransform
    // for transforms that'll always be owned by the server.
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        // Used to determine who can write to this transform. Owner client only.
        // This imposes state to the server. This is putting trust on your clients. Make sure no security-sensitive features use this transform.
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}