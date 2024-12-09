using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using Unity.EditorCoroutines.Editor;


public class SheetToScriptables : MonoBehaviour
{
    // Lien vers le Google Sheet publié
    private static string googleSheetUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRgdnWlKvFW-Eh12BpT4EkbnrwjKeJ291yEusUQwBqE_BiHBNlZq0HXuBW-8ZovCw/pub?gid=1878878299&single=true&output=csv";

    [MenuItem("Tools/Fetch Google Sheet Data")]
    public static void FetchGoogleSheetDataMenu()
    {
        // Create an editor coroutine to handle the fetch
        EditorCoroutineUtility.StartCoroutineOwnerless(FetchGoogleSheetData());
    }

    private static IEnumerator FetchGoogleSheetData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(googleSheetUrl))
        {
            // Envoyer la requête
            yield return webRequest.SendWebRequest();

            // Vérifier si une erreur s'est produite
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur lors de la récupération du Google Sheet : " + webRequest.error);
            }
            else
            {
                // Récupérer le contenu
                string data = webRequest.downloadHandler.text;

                // Debug la première ligne
                string[] rows = data.Split('\n');
                if (rows.Length > 0)
                {
                    Debug.Log("Première ligne : " + rows[2]);
                }
                else
                {
                    Debug.LogWarning("Le fichier récupéré est vide !");
                }
            }
        }
    }
}