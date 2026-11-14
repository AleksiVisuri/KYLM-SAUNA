using UnityEngine;

/// <summary>
/// Scene-independent action/observation boundary for future ML-Agents or
/// Junction integration. Actions are normalized to [-1,1]/[0,1].
/// </summary>
public sealed class SaunaEpisodeController : MonoBehaviour
{
    [SerializeField] private SaunaLauncher launcher;
    [SerializeField] private SaunaTarget target;

    public int ShotCount { get; private set; }
    public float PreviousReward { get; private set; }
    public bool EpisodeComplete => target != null && target.IsHeated;
    public System.Action<float> RewardReceived;

    public Vector4 GetObservations()
    {
        if (launcher == null || target == null)
            return Vector4.zero;

        var offset = target.transform.position - launcher.transform.position;
        return new Vector4(
            Mathf.Clamp(offset.x / 20f, -1f, 1f),
            Mathf.Clamp(offset.y / 20f, -1f, 1f),
            Mathf.Clamp(offset.z / 20f, -1f, 1f),
            target.DesiredTemperature <= 0f ? 0f : target.Temperature / target.DesiredTemperature);
    }

    public void Step(float horizontal, float vertical, float power)
    {
        if (EpisodeComplete || launcher == null || !launcher.CanLaunch)
            return;

        launcher.Launch(horizontal, vertical, power);
        ShotCount++;
        PreviousReward = launcher.LastReward;
    }

    public void ResetEpisode()
    {
        target?.ResetTarget();
        ShotCount = 0;
        PreviousReward = 0f;
    }

    public void Configure(SaunaLauncher configuredLauncher, SaunaTarget configuredTarget)
    {
        if (launcher != null) launcher.RewardReceived -= ForwardReward;
        launcher = configuredLauncher;
        target = configuredTarget;
        if (launcher != null) launcher.RewardReceived += ForwardReward;
    }

    private void ForwardReward(float reward) => RewardReceived?.Invoke(reward);
}
