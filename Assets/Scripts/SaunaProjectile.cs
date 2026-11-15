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
    private SaunaTarget target;
    private float closestDistance = float.MaxValue;
    private bool resolved;

    public void Initialize(SaunaLauncher owner)
    {
        launcher = owner;
        target = FindObjectOfType<SaunaTarget>();
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (target != null)
            closestDistance = Mathf.Min(closestDistance, Vector3.Distance(transform.position, target.transform.position));
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
            var invalidAreaLayer = LayerMask.NameToLayer("InvalidArea");
            ReportReward(invalidAreaLayer >= 0 && collision.collider.gameObject.layer == invalidAreaLayer ? -5f : -1f);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!resolved)
        {
            resolved = true;
            var closeness = target == null ? 0f : target.GetCloseness(transform.position);
            ReportReward(-1f + 0.9f * closeness);
        }
        launcher?.ProjectileFinished(this);
    }
}
