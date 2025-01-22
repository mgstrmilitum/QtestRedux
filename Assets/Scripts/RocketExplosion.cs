using UnityEngine;

public class RocketExplosion : MonoBehaviour
{
    private Collider[] hitColliders;
    public float blastRadius;
    public float explosiveForce;
    public LayerMask explosionLayers;
    public ParticleSystem Boom;
    public int blastDamage;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.contacts[0].point.ToString());
    }
  
    void OnExplosion(Vector3 explosionPoint)
    {
        hitColliders = Physics.OverlapSphere(explosionPoint, blastRadius, explosionLayers);
        foreach(Collider hotcol in hitColliders)
        {
            Debug.Log(hotcol.gameObject.name);
            if(hotcol.GetComponent<Rigidbody>() != null)
            {hotcol.GetComponent<Rigidbody>().isKinematic = false;
             hotcol.GetComponent <Rigidbody>().AddExplosionForce(explosiveForce,explosionPoint,blastRadius,1,ForceMode.Impulse);
                IDamage dmg = hotcol.GetComponent<IDamage>();
            if (dmg!=null)
            {

                dmg.TakeDamage(blastDamage);
            }

            }
            
        }
    }

}
