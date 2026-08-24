using PurrNet;
using UnityEngine;
public class Surface : NetworkBehaviour
{
    public enum SurfaceType
    {
        Default,
        Player
    }

    [SerializeField] private SurfaceType surfaceType;
    public SurfaceType Type => surfaceType;
}
