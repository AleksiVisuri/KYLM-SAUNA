using UnityEngine;

/// <summary>
/// Projectile used by the Phase 1 gameplay loop. It forwards hit/miss results
/// to the launcher and destroys itself after a terminal collision or timeout.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public sealed class SaunaProjectile : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float lifetime = 8f;

    private SaunaLauncher launcher;
    private bool resolved;

    public void Initialize(SaunaLauncher owner)
    {
        launcher = owner;
        Destroy(gameObject, lifetime);
    }

    public void ReportReward(float reward)
    {
        launcher?.ReceiveReward(reward);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (resolved)
            return;

        resolved = true;
        var target = collision.collider.GetComponentInParent<SaunaTarget>();
        if (target != null)
        {
            target.RegisterHit(this);
        }
        else
        {
            ReportReward(collision.collider.CompareTag("InvalidArea") ? -5f : -1f);
        }

        Destroy(gameObject);
    }
}
