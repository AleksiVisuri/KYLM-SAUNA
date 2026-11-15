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
    [SerializeField] private Vector3 launcherPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private bool aimLauncherAtTargetOnStart = false;
    [SerializeField] private Vector3 targetAimOffset = new Vector3(0f, 1f, 0f);

    private void Start()
    {
        if (!Application.isPlaying)
            return;
        if (!createRuntimePlaceholders) return;
        var target = FindOrCreateTarget();
        var launcher = CreateLauncher(GetAimPoint(target) + targetAimOffset);
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

    private static SaunaTarget FindOrCreateTarget()
    {
        var namedTarget = GameObject.Find("Cube.041");
        if (namedTarget != null)
            return EnsureTarget(namedTarget);

        var sauna = GameObject.Find("Sauna");
        if (sauna != null)
        {
            foreach (var child in sauna.GetComponentsInChildren<Transform>(true))
            {
                if (child.name != "Cube.041") continue;
                return EnsureTarget(child.gameObject);
            }
        }

        return CreateFallbackTarget();
    }

    private static SaunaTarget EnsureTarget(GameObject targetObject)
    {
        var target = targetObject.GetComponent<SaunaTarget>() ?? targetObject.AddComponent<SaunaTarget>();
        if (targetObject.GetComponent<Collider>() == null)
        {
            var mesh = targetObject.GetComponent<MeshFilter>();
            if (mesh != null && mesh.sharedMesh != null)
                targetObject.AddComponent<MeshCollider>().sharedMesh = mesh.sharedMesh;
            else
                targetObject.AddComponent<BoxCollider>();
        }
        return target;
    }

    private static Vector3 GetAimPoint(SaunaTarget target)
    {
        var collider = target.GetComponentInChildren<Collider>();
        if (collider != null)
            return collider.bounds.center;

        var renderer = target.GetComponentInChildren<Renderer>();
        return renderer != null ? renderer.bounds.center : target.transform.position;
    }

    private static SaunaTarget CreateFallbackTarget()
    {
        var targetObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        targetObject.name = "Prototype Target (Replace)";
        targetObject.transform.position = new Vector3(0f, 1f, 12f);
        targetObject.transform.localScale = new Vector3(3f, 0.1f, 3f);
        return targetObject.AddComponent<SaunaTarget>();
    }

    private SaunaLauncher CreateLauncher(Vector3 targetPosition)
    {
        var launcherObject = new GameObject("Prototype Launcher (Replace)");
        launcherObject.transform.position = launcherPosition;
        launcherObject.transform.forward = aimLauncherAtTargetOnStart
            ? (targetPosition - launcherObject.transform.position).normalized
            : Vector3.forward;
        var origin = new GameObject("Launch Origin").transform;
        origin.SetParent(launcherObject.transform, false);
        origin.localPosition = Vector3.zero;

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
