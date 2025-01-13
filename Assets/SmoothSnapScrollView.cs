using UnityEngine;
using UnityEngine.UI;

public class SmoothSnapScrollView : MonoBehaviour
{
    public ScrollRect scrollRect; // Référence au ScrollRect
    public float snapSpeed = 10f; // Vitesse de la transition vers la position cible
    public RectTransform content; // Le RectTransform du contenu
    public int numberOfItems = 5; // Nombre d'éléments à afficher
    private float[] snapPoints; // Positions des points de *snap*
    private float targetNormalizedPosition; // Position cible
    private bool isSnapping; // Indique si une transition est en cours

    private void Start()
    {
        // Calculer les positions des points de *snap* en fonction du nombre d'éléments
        snapPoints = new float[numberOfItems];
        float stepSize = 1f / (numberOfItems - 1); // Espace entre les positions normalisées
        for (int i = 0; i < numberOfItems; i++)
        {
            snapPoints[i] = i * stepSize;
        }
    }

    private void Update()
    {
        if (isSnapping)
        {
            // Transition fluide vers la position cible
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targetNormalizedPosition, snapSpeed * Time.deltaTime);

            // Arrêter la transition une fois proche de la cible
            if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetNormalizedPosition) < 0.001f)
            {
                scrollRect.horizontalNormalizedPosition = targetNormalizedPosition;
                isSnapping = false;
            }
        }
    }

    public void OnEndDrag()
    {
        // Trouver le point de *snap* le plus proche
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < snapPoints.Length; i++)
        {
            float distance = Mathf.Abs(scrollRect.horizontalNormalizedPosition - snapPoints[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetNormalizedPosition = snapPoints[i];
            }
        }

        // Lancer la transition vers le point de *snap*
        isSnapping = true;
    }
}
