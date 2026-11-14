using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// ML-Agents adapter for SaunaEpisodeController.
/// </summary>
public sealed class SaunaMlAgent : Agent
{
    [SerializeField] private SaunaEpisodeController episode;

    public override void Initialize()
    {
        BindEpisode(episode);
    }

    public void BindEpisode(SaunaEpisodeController configuredEpisode)
    {
        if (episode != null)
            episode.RewardReceived -= OnRewardReceived;
        episode = configuredEpisode;
        if (episode != null)
            episode.RewardReceived += OnRewardReceived;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (episode == null)
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            return;
        }
        var observations = episode.GetObservations();
        sensor.AddObservation(observations.x);
        sensor.AddObservation(observations.y);
        sensor.AddObservation(observations.z);
        sensor.AddObservation(observations.w);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (episode == null)
            return;
        episode.Step(actions.ContinuousActions[0], actions.ContinuousActions[1], actions.ContinuousActions[2]);
        AddReward(-0.01f);
        if (episode.EpisodeComplete)
            EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = 0f;
        continuous[1] = 0.45f;
        continuous[2] = 0.65f;
    }

    public override void OnEpisodeBegin() => episode?.ResetEpisode();

    private void OnRewardReceived(float reward)
    {
        AddReward(reward);
        if (reward >= 20f)
            EndEpisode();
    }

    private void OnDestroy()
    {
        if (episode != null)
            episode.RewardReceived -= OnRewardReceived;
    }
}
