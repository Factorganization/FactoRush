using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using GameContent.Entities.UnmanagedEntities;

public class EnemySpawner : MonoBehaviour
{
    #region Fields

    public static EnemySpawner Instance;

    [Header("Enemy Data Settings")]
    public string enemyDataId; // ID pour charger le fichier txt

    [Header("Spawn Timing Settings")]
    public float timeBetweenLines = 10f; // Temps entre chaque ligne
    public float delayBetweenUnits = 0.5f; // Délai entre chaque unité sur la même ligne

    private ComponentAtlas componentAtlas; // Référence à l'atlas des composants
    private Queue<string> linesQueue; // File d'attente pour stocker les lignes du fichier
    private System.Random random; // Générateur de nombres aléatoires

    #endregion

    #region Unity Methods

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        linesQueue = new Queue<string>();
        componentAtlas = ComponentAtlas.Instance;
        random = new System.Random(); // Initialiser le générateur de nombres aléatoires
        ReadFile(); // Charger les données du fichier
        StartCoroutine(ProcessFile()); // Commencer à traiter les lignes
    }

    #endregion

    #region Methods

    // Lecture du fichier dans Resources/EnemyData/
    private void ReadFile()
    {
        string filePath = Path.Combine(Application.dataPath, "Resources/EnemyWave", $"{enemyDataId}.txt");
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                // Ignorer les lignes vides ou commençant par un commentaire
                string trimmedLine = line.Split('#')[0].Trim(); // Ignore tout après un #
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    linesQueue.Enqueue(trimmedLine);
                }
            }
        }
        else
        {
            Debug.LogError($"Fichier introuvable : {filePath}");
        }
    }

    // Traitement des lignes avec un intervalle configurable
    private IEnumerator ProcessFile()
    {
        while (linesQueue.Count > 0)
        {
            string line = linesQueue.Dequeue();

            if (line.Trim() != "-") // Ignorer les lignes contenant seulement "-"
            {
                if (line.StartsWith("/"))
                {
                    StartCoroutine(SpawnRandomEnemies(line));
                }
                else
                {
                    StartCoroutine(SpawnEnemies(line));
                }
            }

            yield return new WaitForSeconds(timeBetweenLines); // Attendre un délai configurable avant de traiter la prochaine ligne
        }
    }

    // Spawn d'un nombre aléatoire d'ennemis définis sur une ligne commençant par "/"
    private IEnumerator SpawnRandomEnemies(string line)
    {
        // Extraire le nombre d'unités à choisir aléatoirement
        string[] parts = line.Split(' ');
        if (parts.Length < 2 || !int.TryParse(parts[0].Substring(1), out int count))
        {
            Debug.LogError($"Format de ligne aléatoire invalide : {line}");
            yield break;
        }

        string[] enemyDataArray = parts[1].Split(',');

        // Si le nombre d'unités à choisir est supérieur au nombre disponible, prendre tout
        count = Mathf.Min(count, enemyDataArray.Length);

        // Mélanger les données disponibles et prendre les `count` premières
        List<string> shuffledEnemies = new List<string>(enemyDataArray);
        for (int i = 0; i < shuffledEnemies.Count; i++)
        {
            int randomIndex = random.Next(i, shuffledEnemies.Count);
            (shuffledEnemies[i], shuffledEnemies[randomIndex]) = (shuffledEnemies[randomIndex], shuffledEnemies[i]);
        }

        for (int i = 0; i < count; i++)
        {
            string enemyData = shuffledEnemies[i];
            if (enemyData.Length == 4) // Vérifie si l'entrée est valide (4 caractères)
            {
                // Parse les données pour le transport et l'arme
                int transportId = int.Parse(enemyData.Substring(0, 2)); // Premiers 2 chiffres
                int weaponId = int.Parse(enemyData.Substring(2, 2));   // Derniers 2 chiffres

                // Trouve les composants correspondants dans l'Atlas
                TransportComponent transportComponent = GetTransportComponent(transportId);
                WeaponComponent weaponComponent = GetWeaponComponent(weaponId);

                // Vérifie que les composants existent
                if (transportComponent != null && weaponComponent != null)
                {
                    UnitsManager.Instance.SpawnUnit(false, transportComponent, weaponComponent, null, i * delayBetweenUnits);
                }
                else
                {
                    Debug.LogError($"Composants non trouvés pour Transport ID: {transportId}, Weapon ID: {weaponId}");
                }
            }
            else
            {
                Debug.LogError($"Format de données invalide : {enemyData}");
            }

            yield return new WaitForSeconds(delayBetweenUnits); // Attendre avant de spawner la prochaine unité
        }
    }

    // Spawn des ennemis définis sur une ligne
    private IEnumerator SpawnEnemies(string line)
    {
        string[] enemyDataArray = line.Split(',');

        for (int i = 0; i < enemyDataArray.Length; i++)
        {
            string enemyData = enemyDataArray[i];
            if (enemyData.Length == 4) // Vérifie si l'entrée est valide (4 caractères)
            {
                // Parse les données pour le transport et l'arme
                int transportId = int.Parse(enemyData.Substring(0, 2)); // Premiers 2 chiffres
                int weaponId = int.Parse(enemyData.Substring(2, 2));   // Derniers 2 chiffres

                // Trouve les composants correspondants dans l'Atlas
                TransportComponent transportComponent = GetTransportComponent(transportId);
                WeaponComponent weaponComponent = GetWeaponComponent(weaponId);

                // Vérifie que les composants existent
                if (transportComponent != null && weaponComponent != null)
                {
                    UnitsManager.Instance.SpawnUnit(false, transportComponent, weaponComponent, null, i * delayBetweenUnits);
                }
                else
                {
                    Debug.LogError($"Composants non trouvés pour Transport ID: {transportId}, Weapon ID: {weaponId}");
                }
            }
            else
            {
                Debug.LogError($"Format de données invalide : {enemyData}");
            }

            yield return new WaitForSeconds(delayBetweenUnits); // Attendre avant de spawner la prochaine unité
        }
    }

    // Récupère le TransportComponent depuis l'Atlas
    private TransportComponent GetTransportComponent(int id)
    {
        switch (id)
        {
            case 1: return componentAtlas.transportBase;
            case 2: return componentAtlas.transportPropeller;
            case 3: return componentAtlas.transportThornmail;
            case 4: return componentAtlas.transportAccumulator;
            case 5: return componentAtlas.transportDrill;
            case 6: return componentAtlas.transportInsulatingWheels;
            case 7: return componentAtlas.transportSlider;
            case 8: return componentAtlas.transportTwinBoots;
            default: return null;
        }
    }

    // Récupère le WeaponComponent depuis l'Atlas
    private WeaponComponent GetWeaponComponent(int id)
    {
        switch (id)
        {
            case 1: return componentAtlas.weaponBase;
            case 2: return componentAtlas.weaponArtillery;
            case 3: return componentAtlas.weaponC4;
            case 4: return componentAtlas.weaponCanon;
            case 5: return componentAtlas.weaponMinigun;
            case 6: return componentAtlas.weaponRailgun;
            case 7: return componentAtlas.weaponShield;
            case 8: return componentAtlas.weaponSpear;
            case 9: return componentAtlas.weaponSpinningBlade;
            case 10: return componentAtlas.weaponSymbioticRifle;
            case 11: return componentAtlas.weaponWarhammer;
            default: return null;
        }
    }

    #endregion
}
