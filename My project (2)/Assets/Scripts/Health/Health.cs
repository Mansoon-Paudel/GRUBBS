using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Header ("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("Boss Death")]
    [SerializeField] private int bossDeathSceneBuildIndex = 2;
    [SerializeField] private string bossDeathSceneName = "End Menu";
    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }
    public void TakeDamage(float _damage)
    {
        if (invulnerable) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
            SoundManager.instance.PlaySound(hurtSound);
        }
        else
        {
            if (!dead)
            {
                if (IsBoss())
                {
                    dead = true;
                    if (!string.IsNullOrEmpty(bossDeathSceneName))
                    {
                        Debug.Log("Health: Boss died — loading scene by name: " + bossDeathSceneName);
                        SceneManager.LoadScene(bossDeathSceneName);
                        return;
                    }

                    int buildCount = SceneManager.sceneCountInBuildSettings;
                    if (bossDeathSceneBuildIndex >= 0 && bossDeathSceneBuildIndex < buildCount)
                    {
                        Debug.Log("Health: Boss died — loading scene by build index: " + bossDeathSceneBuildIndex);
                        SceneManager.LoadScene(bossDeathSceneBuildIndex);
                    }
                    else
                    {
                        Debug.LogWarning($"Health: bossDeathSceneBuildIndex ({bossDeathSceneBuildIndex}) is out of range (0..{buildCount - 1}). Not loading scene.");
                    }

                    return;
                }
                
                foreach (Behaviour component in components)
                    component.enabled = false;
                anim.SetBool("grounded", true);
                anim.SetTrigger("die");
                dead = true;
                SoundManager.instance.PlaySound(deathSound);
                PlayerRespawn playerRespawn = GetComponent<PlayerRespawn>();
                if (playerRespawn != null)
                    playerRespawn.OnPlayerDeath();
            }
        }
    }
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private bool IsBoss()
    {
        return CompareTag("Boss") || (transform.root != null && transform.root.CompareTag("Boss"));
    }
    private bool _allowRespawn = false;
    public void AllowRespawnAndPerform()
    {
        _allowRespawn = true;
        Respawn();
    }

    public void Respawn()
    {
        if (!_allowRespawn)
        {
            return;
        }

        _allowRespawn = false;

        AddHealth(startingHealth);
        anim.ResetTrigger("die");
        anim.Play("Idle");
        StartCoroutine(Invunerability());
        dead = false;

        //Activate all attached component classes
        foreach (Behaviour component in components)
            component.enabled = true;
    }
}