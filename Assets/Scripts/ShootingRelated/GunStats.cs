using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public GameObject model;
    public int shootDamage;
    public int shootDistance;
    public float shootRate;
    public int ammoCurrent, ammoMax;
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound; //different sound for end of magazine?
    public float shootSoundVolume;
}
