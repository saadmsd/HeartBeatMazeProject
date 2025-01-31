using UnityEngine;

public class VariableChanger : MonoBehaviour
{
    public Agent agent_monstre;  // Référence à l'Agent dont on veut modifier la vitesse

    public Agent agent_monstre2; // Référence à l'Agent dont on veut modifier la vitesse
    private float minSpeed = 0.5f;
    private float maxSpeed = 6.5f;
    private float speedIncrease = 0.1f; // Augmentation de la vitesse à chaque intervalle
    private float switchInterval = 1f; // Temps avant d'augmenter la vitesse

    private float currentSpeed;
    private float timer;

    void Start()
    {
        currentSpeed = minSpeed; // Initialise la vitesse au minimum
        agent_monstre.speed = currentSpeed;
        agent_monstre2.speed = agent_monstre.speed;
        // Debug pour voir la progression de la vitesse
        Debug.Log("Speed: " + agent_monstre.speed);
    }

    void Update()
    {
        if (agent_monstre == null) return; // Vérifie si l'agent est assigné

        // Incrémentation du timer
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            // Augmenter la vitesse progressivement
            currentSpeed += speedIncrease;

            // Limite la vitesse au maxSpeed
            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }

            agent_monstre.speed = currentSpeed; // Applique la nouvelle vitesse

            // Réinitialisation du timer
            timer = 0f;
        }
         agent_monstre2.speed = agent_monstre.speed;
         Debug.Log("Speed: " + agent_monstre.speed);
        

        
    }
}
