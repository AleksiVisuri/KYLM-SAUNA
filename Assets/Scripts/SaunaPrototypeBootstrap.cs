using UnityEngine;

/// <summary>
/// Temporary Phase 1 harness for the otherwise empty Sauna scene. Remove this
/// component when the Blender environment is ready.
/// </summary>
public sealed class SaunaPrototypeBootstrap : MonoBehaviour
{
    [SerializeField] private bool createRuntimePlaceholders = true;
    [SerializeField] private bool addMlAgent = true;
    [SerializeField, Range(0.1f, 10f)] private float secondsBetweenThrows = 1f;

    private void Start()
    {
        if (!Application.isPlaying)
            return;
        if (!createRuntimePlaceholders) return;
        var target = CreateTarget();
        var launcher = CreateLauncher(target.transform.position);
        SetPrivateField(launcher, "secondsBetweenThrows", secondsBetweenThrows);
        var episode = gameObject.AddComponent<SaunaEpisodeController>();
        episode.Configure(launcher, target);
        var configuration = GetComponent<SaunaAgentConfiguration>();
        configuration?.PrepareBehaviorParameters();
        if (addMlAgent)
        {
            var agent = gameObject.AddComponent<SaunaMlAgent>();
            agent.BindEpisode(episode);
            configuration?.ConfigureDecisionRequester();
        }
        episode.Step(0f, 0.45f, 0.65f);
    }

    private static SaunaTarget CreateTarget()
    {
        var targetObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        targetObject.name = "Prototype Target (Replace)";
        targetObject.transform.position = new Vector3(0f, 1f, 12f);
        targetObject.transform.localScale = new Vector3(3f, 0.1f, 3f);
        return targetObject.AddComponent<SaunaTarget>();
    }

    private static SaunaLauncher CreateLauncher(Vector3 targetPosition)
    {
        var launcherObject = new GameObject("Prototype Launcher (Replace)");
        launcherObject.transform.position = new Vector3(0f, 1f, 0f);
        launcherObject.transform.forward = (targetPosition - launcherObject.transform.position).normalized;
        var origin = new GameObject("Launch Origin").transform;
        origin.SetParent(launcherObject.transform, false);
        origin.localPosition = Vector3.forward;

        var projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectileObject.name = "Prototype Projectile (Runtime)";
        projectileObject.transform.localScale = Vector3.one * 0.35f;
        projectileObject.SetActive(false);
        projectileObject.AddComponent<Rigidbody>();
        projectileObject.AddComponent<SaunaProjectile>();
        var launcher = launcherObject.AddComponent<SaunaLauncher>();
        SetPrivateField(launcher, "projectilePrefab", projectileObject.GetComponent<SaunaProjectile>());
        SetPrivateField(launcher, "launchOrigin", origin);
        return launcher;
    }

    private static void SetPrivateField(Object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field?.SetValue(target, value);
    }
}
