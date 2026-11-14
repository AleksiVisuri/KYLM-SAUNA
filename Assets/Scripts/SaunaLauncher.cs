using UnityEngine;

/// <summary>
/// Minimal launcher API for manual testing and future ML-Agents integration.
/// Actions are normalized: horizontal [-1,1], vertical [0,1], power [0,1].
/// </summary>
public sealed class SaunaLauncher : MonoBehaviour
{
    [SerializeField] private SaunaProjectile projectilePrefab;
    [SerializeField] private Transform launchOrigin;
    [SerializeField, Min(0.1f)] private float minLaunchSpeed = 8f;
    [SerializeField, Min(0.1f)] private float maxLaunchSpeed = 25f;

    public float LastReward { get; private set; }

    public SaunaProjectile Launch(float horizontal, float vertical, float power)
    {
        if (projectilePrefab == null || launchOrigin == null)
        {
            Debug.LogError("SaunaLauncher requires a projectile prefab and launch origin.");
            return null;
        }

        horizontal = Mathf.Clamp(horizontal, -1f, 1f);
        vertical = Mathf.Clamp01(vertical);
        power = Mathf.Clamp01(power);

        var direction = Quaternion.Euler(vertical * 90f, horizontal * 90f, 0f) * launchOrigin.forward;
        var projectile = Instantiate(projectilePrefab, launchOrigin.position, Quaternion.LookRotation(direction));
        projectile.gameObject.SetActive(true);
        projectile.Initialize(this);
        projectile.GetComponent<Rigidbody>().velocity = direction.normalized * Mathf.Lerp(minLaunchSpeed, maxLaunchSpeed, power);
        LastReward = 0f;
        return projectile;
    }

    public void ReceiveReward(float reward)
    {
        LastReward += reward;
        Debug.Log($"Sauna launcher reward: {reward:+0.##;-0.##;0} (episode total: {LastReward:0.##})");
    }
}
