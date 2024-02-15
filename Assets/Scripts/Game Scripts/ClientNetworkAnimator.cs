using Unity.Netcode.Components;

// This class represents a client-side network animator that extends the NetworkAnimator class.
// It is necessary to handle network synchronization of animations on the client side.
public class ClientNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
