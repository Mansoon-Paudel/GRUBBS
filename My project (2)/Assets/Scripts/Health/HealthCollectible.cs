using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthValue;
    [SerializeField] private AudioClip Sound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        SoundManager.instance.PlaySound(Sound);
        collision.GetComponent<Health>().AddHealth(healthValue);
        gameObject.SetActive(false);
    }
}
