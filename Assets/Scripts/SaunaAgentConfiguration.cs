using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;

/// <summary>
/// Inspector-visible ML-Agents configuration for the Sauna scene.
/// </summary>
public sealed class SaunaAgentConfiguration : MonoBehaviour
{
    [SerializeField, Min(1)] private int vectorObservationSize = 4;
    [SerializeField, Min(1)] private int continuousActionCount = 3;
    [SerializeField, Range(1, 20)] private int decisionPeriod = 1;
    [SerializeField] private bool heuristicTesting = true;

    public void PrepareBehaviorParameters()
    {
        var behavior = GetComponent<BehaviorParameters>() ?? gameObject.AddComponent<BehaviorParameters>();
        behavior.BrainParameters.VectorObservationSize = vectorObservationSize;
        behavior.BrainParameters.NumStackedVectorObservations = 1;
        behavior.BrainParameters.ActionSpec = ActionSpec.MakeContinuous(continuousActionCount);
        behavior.BehaviorType = heuristicTesting ? BehaviorType.HeuristicOnly : BehaviorType.Default;
    }

    public void ConfigureDecisionRequester()
    {
        var requester = GetComponent<DecisionRequester>() ?? gameObject.AddComponent<DecisionRequester>();
        requester.DecisionPeriod = decisionPeriod;
        requester.TakeActionsBetweenDecisions = false;
    }
}
