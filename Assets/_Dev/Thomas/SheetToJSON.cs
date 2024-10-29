using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GoogleSheetFetcher : MonoBehaviour
{
    private string baseCsvUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRgdnWlKvFW-Eh12BpT4EkbnrwjKeJ291yEusUQwBqE_BiHBNlZq0HXuBW-8ZovCw/pub?output=csv&gid=";
    private List<string> gids = new List<string> { "206496070", "409085572", "919971693" }; // Les GIDs spécifiés

    private void Awake()
    {
        StartCoroutine(GetDataFromSheets());
    }

    private IEnumerator GetDataFromSheets()
    {
        List<string> sheetData = new List<string>();

        // Récupère les données pour chaque GID
        foreach (string gid in gids)
        {
            string csvURL = baseCsvUrl + gid;
            yield return StartCoroutine(GetCSVData(csvURL, sheetData));
        }

        // Imprime toutes les lignes récupérées en un seul log, séparées par des virgules
        Debug.Log(string.Join(", ", sheetData));
    }

    private IEnumerator GetCSVData(string csvURL, List<string> results)
    {
        using (UnityWebRequest csvRequest = UnityWebRequest.Get(csvURL))
        {
            yield return csvRequest.SendWebRequest();

            if (csvRequest.result == UnityWebRequest.Result.ConnectionError || 
                csvRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(csvRequest.error);
            }
            else
            {
                string csvData = csvRequest.downloadHandler.text;
                string line2 = ProcessData(csvData);
                if (!string.IsNullOrEmpty(line2))
                {
                    results.Add(line2);
                }
            }
        }
    }

    private string ProcessData(string data)
    {
        // Sépare les lignes par retour à la ligne
        string[] rows = data.Split('\n');

        // Vérifie si la ligne 2 existe
        if (rows.Length > 1)
        {
            // Sépare la ligne 2 par les virgules
            string[] columns = rows[1].Split(',');

            // Concatène les valeurs de la ligne 2 jusqu'à la colonne H (colonne 8)
            List<string> selectedColumns = new List<string>();
            for (int i = 0; i < Mathf.Min(columns.Length, 8); i++)
            {
                selectedColumns.Add(columns[i]);
            }
            return string.Join(", ", selectedColumns); // Retourne la ligne sous forme de chaîne
        }
        else
        {
            Debug.LogWarning("La ligne 2 n'existe pas dans les données CSV.");
            return string.Empty;
        }
    }
}
