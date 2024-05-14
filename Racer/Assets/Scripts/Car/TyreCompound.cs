using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TyreCompound", menuName = "Tyre/TyreCompound", order = 1)]
public class TyreCompound : ScriptableObject
{
    [Header("General info")]
    [Tooltip("The amount of grip this tyre makes when the optimum conditions are met"), Range(0, 10)]
    public float grip = 1f;
    [Tooltip("The amount of grip from the tyre on different tyre temperatures"), Curve(0, 0, 1f, 1f, true)]
    public AnimationCurve gripCurve;
    [Tooltip("The optimum temperature window for the tyres to operate in")]
    public Vector2 optimumTyreTemps;

    [Header("Forward grip")]
    [Tooltip("The maximum amount of slip that can be toleraded by the tire. if it exeeds this level the tire begins to slip")]
    public float forwardExtremumSlip = 0.4f;
    [Tooltip("The amount of force that the tyre gives forward when on the extremumSlip point")]
    public float forwardExtremumValue = 1f;
    [Tooltip("The amount of slip needed for the tyre to keep giving a constant (low) force forward (spinning or drifting)")]
    public float forwardAsymptoteSlip = 0.8f;
    [Tooltip("The amount of force that the tyre gives forward when on the asymptoteSlip point or beyond")]
    public float forwardAsymptoteValue = 0.5f;

    [Header("Sideways grip")]
    [Tooltip("The maximum amount of slip that can be toleraded by the tire. if it exeeds this level the tire begins to slip")]
    public float sidewayExtremumSlip = 0.2f;
    [Tooltip("The amount of force that the tyre gives sideways when on the extremumSlip point")]
    public float sidewayExtremumValue = 1f;
    [Tooltip("The amount of slip needed for the tyre to keep giving a constant (low) force forward (spinning or drifting)")]
    public float sidewayAsymptoteSlip = 0.5f;
    [Tooltip("The amount of force that the tyre gives sideways when on the asymptoteSlip point or beyond")]
    public float sidewayAsymptoteValue = 0.75f;
}
