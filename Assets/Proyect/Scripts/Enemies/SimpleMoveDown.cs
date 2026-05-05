using UnityEngine;

public class SimpleMoveDown : MonoBehaviour
{
    public float velocidad = 1f;

    void Update()
    {
        // Mueve a todo el barco (y sus hijos) hacia abajo
        transform.Translate(Vector3.down * velocidad * Time.deltaTime);

        // Si sale de la pantalla por abajo, se desactiva (para el ObjectPooler)
        if (transform.position.y < -10f)
        {
            gameObject.SetActive(false);
        }
    }
}