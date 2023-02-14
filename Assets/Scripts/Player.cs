using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    
    public bool invincibilityActive = false;

    private void OnTriggerEnter(Collider collision)
    {
        print("collsion with enemy");
        if (invincibilityActive && collision.gameObject.CompareTag("Enemy"))
        {
            // Kill the enemy when the player collides with it while invincibility is active
            print("dead enemy");
            Destroy(collision.gameObject);
        }
    }

    public void ActivateInvincibility()
    {
        print("Invincibility activated!");
        invincibilityActive = true;
        // Start a coroutine to deactivate invincibility after a set amount of time
        StartCoroutine(DeactivateInvincibility());
    }

    private IEnumerator DeactivateInvincibility()
    {
        yield return new WaitForSeconds(10f);
        invincibilityActive = false;
        print("Invincibility deactivated!");
    }
}
