using UnityEngine;

/// <summary>
/// Heating zone for the Phase 1 prototype. A projectile hit raises the target
/// temperature and reports a reward to the launcher that fired it.
/// </summary>
public sealed class SaunaTarget : MonoBehaviour
{
    [SerializeField, Min(1f)] private float desiredTemperature = 100f;
    [SerializeField, Min(0f)] private float temperature = 0f;
    [SerializeField, Min(0f)] private float temperaturePerHit = 10f;

    public float Temperature => temperature;
    public float DesiredTemperature => desiredTemperature;
    public bool IsHeated => temperature >= desiredTemperature;

    public float GetCloseness(Vector3 point)
    {
        var collider = GetComponent<Collider>();
        if (collider == null) return 0f;
        var distance = Vector3.Distance(point, collider.ClosestPoint(point));
        return 1f - Mathf.Clamp01(distance / 10f);
    }

    public float RegisterHit(SaunaProjectile projectile)
    {
        if (projectile == null)
            return -5f;

        temperature = Mathf.Min(desiredTemperature, temperature + temperaturePerHit);
        var reward = temperature >= desiredTemperature ? 20f : 10f;
        projectile.ReportReward(reward);
        return reward;
    }

    public void ResetTarget()
    {
        temperature = 0f;
    }
}
