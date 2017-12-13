using UnityEngine;

public class KillZoneDestroyer : MonoBehaviour {

    protected GameObject killzoneKiller;
    protected GameObject killzone;

    public void SetKillzone(GameObject _killzoneKiller, GameObject _killzone)
    {
        killzoneKiller = _killzoneKiller;
        killzone = _killzone;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (killzone && killzoneKiller)
        {
          if (collision.gameObject.name == killzoneKiller.name)
          {
              Destroy(killzone);
              Destroy(gameObject);
            }
        }
    }


}
