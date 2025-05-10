using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OutDoorLightEmitter : MonoBehaviour
{
    [SerializeField] private Light2D emittedlight;
    private Animator animator;
    private float timer = 0f;
    private void Awake()
    {
        //emittedlight = GetComponent<Light2D>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        timer -= Time.deltaTime;    
        if (timer < 0f)
        {
            timer = 0.5f;
            float light = TimeManager.Instance.GetLightInesityForTimeOfDay();
            if (light > 0.1f)
            {
                animator.SetBool("IsLit", true);
                emittedlight.intensity = Random.Range(light - 0.1f, light + 0.1f);
            }
            else 
            {
                animator.SetBool("IsLit", false);
                emittedlight.intensity = 0f;
            }
            
        }
    }
}
