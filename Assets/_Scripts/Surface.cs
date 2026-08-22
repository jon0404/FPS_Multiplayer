using PurrNet;
using UnityEngine;
public class Surface : NetworkBehaviour
{
    public enum SurfaceType
    {
        Default,
        Player
    }

    public SurfaceType surfaceType;
}
