
using UnityEngine;

public class KillZoneDestroyer : MonoBehaviour {

    public string killZoneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Engineer")
        {
            GameObject killZone = GameObject.Find(killZoneName);
            Destroy(killZone);
        }
    }


}
