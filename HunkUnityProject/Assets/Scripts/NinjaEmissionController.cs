using UnityEngine;
using RoR2;

public class NinjaEmissionController : MonoBehaviour
{
    public float maxEmission = 1f;
    public float smoothSpeed = 3f;

    public SkinnedMeshRenderer bodyRenderer;
    public SkinnedMeshRenderer feetRenderer;

    public TrailRenderer[] trails = new TrailRenderer[0];
    public TrailRenderer[] sprintTrails = new TrailRenderer[0];
    public ParticleSystem[] activeEffects = new ParticleSystem[0];
    public ParticleSystem[] sprintEffects = new ParticleSystem[0];

    public Color baseTrailColor = Color.black;
    public Color activeTrailColor = Color.white;

    public string sprintSoundString = "sfx_thruster_loop";

    private float currentPower;
    private float currentSprintPower;
    private float sprintStopwatch;
    private uint sprintSoundPlayID;
}