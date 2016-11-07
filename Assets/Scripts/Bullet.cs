using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    //If bullet hit a player, send int 10 to TakeDamage function
    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(10);
        }

        Destroy(gameObject);
    }
}
