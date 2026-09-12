using UnityEngine;

public class KPlayer : MonoBehaviour
{
    [SerializeField] private KPlayerCharacter playerCharacter;
    [SerializeField] private KPlayerCamera playerCamera;
    void Start()
    {
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
    }

    // Update is called once per framessss
    void Update()
    {
        
    }
}
