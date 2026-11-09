using PurrNet;
using UnityEngine;

public class RoundResettable : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool initialActive;
    private Rigidbody body;
    private bool initialIsKinematic;
    private bool initialUseGravity;
    private Vector3 initialVelocity;
    private Vector3 initialAngularVelocity;
    private bool destroyOnReset;
    private bool capturedInitialState;

    private void Awake()
    {
        CaptureInitialState();
        RoundObjectManager.Register(this);
    }

    private void OnEnable()
    {
        if (capturedInitialState)
            RoundObjectManager.Register(this);
    }

    private void OnDestroy()
    {
        RoundObjectManager.Unregister(this);
    }

    public void DestroyAtRoundReset()
    {
        destroyOnReset = true;
    }

    internal void ResetForRound(bool asServer)
    {
        if (destroyOnReset)
        {
            RoundObjectManager.Unregister(this);

            if (TryGetComponent<NetworkIdentity>(out var identity))
            {
                if (identity.isSpawned)
                {
                    if (asServer)
                        identity.Despawn();
                    return;
                }

                identity.Despawn();
            }
            else
            {
                Destroy(gameObject);
            }

            return;
        }

        if (initialActive && !gameObject.activeSelf)
            gameObject.SetActive(true);

        transform.SetPositionAndRotation(initialPosition, initialRotation);
        transform.localScale = initialScale;

        if (body != null)
        {
            body.isKinematic = initialIsKinematic;
            body.useGravity = initialUseGravity;

            if (!initialIsKinematic)
            {
                body.linearVelocity = initialVelocity;
                body.angularVelocity = initialAngularVelocity;
            }
        }

        if (gameObject.activeSelf != initialActive)
            gameObject.SetActive(initialActive);
    }

    private void CaptureInitialState()
    {
        if (capturedInitialState)
            return;

        transform.GetPositionAndRotation(out initialPosition, out initialRotation);
        initialScale = transform.localScale;
        initialActive = gameObject.activeSelf;
        body = GetComponent<Rigidbody>();

        if (body != null)
        {
            initialIsKinematic = body.isKinematic;
            initialUseGravity = body.useGravity;
            initialVelocity = body.linearVelocity;
            initialAngularVelocity = body.angularVelocity;
        }

        capturedInitialState = true;
    }
}