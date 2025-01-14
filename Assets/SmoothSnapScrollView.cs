using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SmoothSnapScrollView : MonoBehaviour
{
    public ScrollRect scrollRect; // Référence au ScrollRect
    public float snapSpeed = 10f; // Vitesse de la transition
    public float threshold = 0.2f; // Seuil pour changer de panneau
    private int currentPanel = 1; // Index du panneau actuel
    private bool isTouching = false; // Si l'utilisateur touche l'écran
    private bool isSnapping = false; // Si une transition est en cours

    private readonly float[] panelPositions = { 0f, 0.5f, 1f }; // Positions des panneaux (0, 1, 2)
    private float startTouchPosition; // Position de départ du swipe
    private float endTouchPosition; // Position de fin du swipe

    // Nouveaux champs ajoutés
    public RectTransform[] panels; // Les panneaux (assignés manuellement)
    private Vector2 initialPanel1Position; // Position verticale initiale du panneau principal
    private bool verticalScrollingEnabled = true; // État du défilement vertical

    private void Awake()
    {
        EnhancedTouchSupport.Enable();

        if (panels != null && panels.Length > 1)
        {
            initialPanel1Position = panels[1].anchoredPosition; // Utiliser la position locale
        }
    }


    private void Update()
    {
        HandleTouchInput();

        if (!isTouching && !isSnapping)
        {
            SnapToClosestPanel();
        }

        if (isSnapping)
        {
            SmoothSnapToTarget();
        }

        // Désactiver ou activer le scrolling vertical en fonction du panneau
        scrollRect.vertical = verticalScrollingEnabled;

        // Appeler AdjustVerticalOffsets pour garantir une mise à jour continue
        AdjustVerticalOffsets();
    }


    private void HandleTouchInput()
    {
        if (Touch.activeTouches.Count > 0)
        {
            isTouching = true;
            isSnapping = false;

            var touch = Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                startTouchPosition = touch.screenPosition.x / Screen.width;
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                endTouchPosition = touch.screenPosition.x / Screen.width;
                EvaluateSwipe();
                isTouching = false;
            }
        }
    }

    private void EvaluateSwipe()
    {
        float swipeDistance = endTouchPosition - startTouchPosition;

        if (Mathf.Abs(swipeDistance) > threshold)
        {
            if (swipeDistance > 0 && currentPanel > 0)
            {
                currentPanel--;
            }
            else if (swipeDistance < 0 && currentPanel < panelPositions.Length - 1)
            {
                currentPanel++;
            }
        }

        // Activer le snapping pour aligner avec le panneau cible
        isSnapping = true;

        // Activer/désactiver le scrolling vertical en fonction du panneau cible
        verticalScrollingEnabled = currentPanel == 1;
    }

    private void SnapToClosestPanel()
    {
        float currentPosition = scrollRect.horizontalNormalizedPosition;
        int closestPanel = GetClosestPanelIndex(currentPosition);

        currentPanel = closestPanel;
        isSnapping = true;

        // Activer/désactiver le scrolling vertical en fonction du panneau cible
        verticalScrollingEnabled = currentPanel == 1;
    }

    private int GetClosestPanelIndex(float currentPosition)
    {
        int closestIndex = 0;
        float minDistance = Mathf.Abs(currentPosition - panelPositions[0]);

        for (int i = 1; i < panelPositions.Length; i++)
        {
            float distance = Mathf.Abs(currentPosition - panelPositions[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private void SmoothSnapToTarget()
    {
        float targetPosition = panelPositions[currentPanel];

        scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
            scrollRect.horizontalNormalizedPosition,
            targetPosition,
            snapSpeed * Time.deltaTime
        );

        if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) < 0.001f)
        {
            scrollRect.horizontalNormalizedPosition = targetPosition;
            isSnapping = false;
        }
    }

    private void AdjustVerticalOffsets()
    {
        if (panels == null || panels.Length <= 1) return;

        // Récupérer la position verticale relative du panneau principal
        float verticalOffset = panels[1].parent.GetComponent<RectTransform>().anchoredPosition.y;

        // Appliquer cet offset aux panneaux latéraux
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == 1) continue; // Ignorer le panneau principal

            // Ajuster la position verticale des panneaux latéraux
            Vector2 currentPosition = panels[i].anchoredPosition;
            panels[i].anchoredPosition = new Vector2(currentPosition.x, - verticalOffset + 633f); // JE SAIS PAS COMMENT ÇA MARCHE MAIS ÇA MARCHE
        }
    }


}
