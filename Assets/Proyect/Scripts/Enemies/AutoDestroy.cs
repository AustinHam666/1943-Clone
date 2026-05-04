using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // El tiempo que tarda en desaparecer (ajustalo a lo que dura tu animación)
    [SerializeField] private float timeToDestroy = 0.5f;

    private void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }
}