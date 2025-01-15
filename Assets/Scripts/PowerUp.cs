using UnityEngine;
public enum PowerType
{
    QuadDamage,
    MegaHealth,
    Invis
}
public class PowerUp : MonoBehaviour
{
    public PowerType powerType;
    [SerializeField] int magnitude;
    [SerializeField] public float duration;
    [SerializeField] GameObject artAsset;
}
