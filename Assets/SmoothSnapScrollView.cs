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

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    private void Update()
    {
        HandleTouchInput();

        // Lancer le snapping si l'utilisateur ne touche pas l'écran et qu'une transition est nécessaire
        if (!isTouching && !isSnapping)
        {
            SnapToClosestPanel();
        }

        // Si un snapping est en cours, continue la transition
        if (isSnapping)
        {
            SmoothSnapToTarget();
        }
    }

    private void HandleTouchInput()
    {
        // Vérifier s'il y a des touches actives
        if (Touch.activeTouches.Count > 0)
        {
            isTouching = true;
            isSnapping = false; // Arrêter le snapping pendant que l'utilisateur interagit

            // Détecter le début et la fin du swipe
            var touch = Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                startTouchPosition = touch.screenPosition.x / Screen.width;
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                endTouchPosition = touch.screenPosition.x / Screen.width;
                EvaluateSwipe(); // Évaluer le swipe pour décider si on change de panneau
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
                currentPanel--; // Aller au panneau précédent
            }
            else if (swipeDistance < 0 && currentPanel < panelPositions.Length - 1)
            {
                currentPanel++; // Aller au panneau suivant
            }
        }

        // Activer le snapping pour aligner avec le panneau cible
        isSnapping = true;
    }

    private void SnapToClosestPanel()
    {
        float currentPosition = scrollRect.horizontalNormalizedPosition;
        int closestPanel = GetClosestPanelIndex(currentPosition);

        // Si aucun swipe n'a été détecté, rester sur le panneau actuel
        currentPanel = closestPanel;
        isSnapping = true;
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

        // Effectuer une interpolation linéaire vers la position cible
        scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
            scrollRect.horizontalNormalizedPosition,
            targetPosition,
            snapSpeed * Time.deltaTime
        );

        // Vérifier si la position cible est atteinte (précision arbitraire)
        if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) < 0.001f)
        {
            scrollRect.horizontalNormalizedPosition = targetPosition;
            isSnapping = false;
        }
    }
}
