using UnityEngine;

public class mt_Destructible : MonoBehaviour
{
    public GameObject fracturedGO;
    public float fracturesLife = 30;

    public float minMass = 50;
    public float minSpeed = 6;

    public GameObject explosionEffect;
    public AudioClip explodingSound;
    public float explosionRange = 50;
    public ForceMode forceMode = ForceMode.Impulse;
    public float explosionForce = 500;
    public int explosionDamage = 10;

    public Transform[] lowHealthEffect;
    public float degradeTime = 1;
    public float healthLoss = 1;

    public float Health = 100;

    [HideInInspector]
    public float currentHealth;

    Rigidbody m_Rigid = null;
    private Rigidbody myRigidbody
    {
        get
        {
            if (m_Rigid == null) m_Rigid = GetComponent<Rigidbody>();
            return m_Rigid;
        }
    }

    public virtual void Start()
    {
        currentHealth = Health;
    }

    public virtual void ActivateShards()
    {
        if (fracturedGO != null)
        {
            GameObject go = (GameObject)Instantiate(fracturedGO, transform.position, transform.rotation);
            go.transform.localScale = transform.lossyScale;

            Rigidbody[] rigid = go.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody t in rigid)
            {
                t.GetComponent<Rigidbody>().AddExplosionForce(myRigidbody.mass, transform.position, explosionRange, 0, forceMode);
            }

            if (fracturesLife > 0) Destroy(go, fracturesLife);
        }
    }

    public virtual void ExplosionSphere()
    {
        transform.parent = null;
        GetComponent<Collider>().enabled = false;

        RaycastHit hit = new RaycastHit();
        Collider[] struckColliders = Physics.OverlapSphere(transform.position, explosionRange);

        foreach (Collider col in struckColliders)
        {
            float distance = Vector3.Distance(transform.position, col.transform.position);
            float damageToApply = (int)Mathf.Abs((1 - (distance / explosionRange)) * explosionDamage);

            if (Physics.Linecast(transform.position, col.transform.position, out hit))
            {
                if (hit.transform == col.transform || col.transform.root.CompareTag("Player"))
                {
                    col.transform.root.SendMessage("ProcessDamage", damageToApply, SendMessageOptions.DontRequireReceiver);
                    col.transform.SendMessage("ProcessDamage", damageToApply, SendMessageOptions.DontRequireReceiver);
                }

                if (col.GetComponent<Rigidbody>() != null) col.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange, 1, forceMode);
            }
        }

        Destroy(gameObject, 0.05f);
    }

    [HideInInspector]
    public bool isExploding = false;
    public virtual void DeductHealth(float healthToDeduct)
    {
        currentHealth -= healthToDeduct;

        if (currentHealth <= (Health / 2) && !isBurning) LowHealth();

        if (currentHealth <= 0 && !isExploding)
        {
            isExploding = true;
            ActivateShards();
            ExplosionSphere();
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation);
            }
            if (explodingSound != null) AudioSource.PlayClipAtPoint(explodingSound, transform.position);
        }
    }

    [HideInInspector]
    public bool isBurning = false;
    public virtual void LowHealth()
    {
        if (lowHealthEffect.Length < 1) return;
        foreach (Transform t in lowHealthEffect) t.gameObject.SetActive(true);
        isBurning = true;
        InvokeRepeating("DegradeHealth", 0, degradeTime);
    }

    void DegradeHealth()
    {
        currentHealth -= healthLoss;
        if (currentHealth <= 0)
        {
            ProcessDamage(1);
        }
    }

    public void ProcessDamage(float damage)
    {
        DeductHealth(damage);
    }

    void CollisionCheck(Rigidbody otherRigidbody)
    {
        if (otherRigidbody.mass > minMass && otherRigidbody.velocity.sqrMagnitude > (minSpeed * minSpeed))
        {
            int damage = (int)otherRigidbody.mass;
            DeductHealth(damage);
        }
        else SelfSpeedCheck();
    }

    void SelfSpeedCheck()
    {
        if (myRigidbody.velocity.sqrMagnitude > (minSpeed * minSpeed))
        {
            int damage = (int)myRigidbody.mass;
            DeductHealth(damage);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.contacts.Length > 0)
        {
            if (col.contacts[0].otherCollider.GetComponent<Rigidbody>() != null) CollisionCheck(col.contacts[0].otherCollider.GetComponent<Rigidbody>());
            else SelfSpeedCheck();
        }
    }

}