using UnityEngine;

public class MapScript : MonoBehaviour
{
    public Transform player; // Référence au joueur
    public RectTransform minimapImage; // Référence à l'Image UI de la carte sur la minimap
    public RectTransform playerMarker; // Référence au marqueur de la position du joueur

    private Vector2 worldMin = new(60f, 230f); // Coin inférieur gauche de la carte réelle
    private Vector2 worldMax = new(190f, 330f); // Coin supérieur droit de la carte réelle
    private Vector2 minimapMin = new(-75f, -75f); // Coin inférieur gauche de la carte
    private Vector2 minimapMax = new(75f, 75f); // Coin supérieur droit de la carte
    Vector2 extentMap;
    Vector2 extentWorld;

    private void Start()
    {
        extentMap = new((minimapMax.x - minimapMin.x), (minimapMax.y - minimapMin.y));
        extentWorld = new((worldMax.x - worldMin.x), (worldMax.y - worldMin.y));
        Debug.Log(extentMap);
        Debug.Log(extentWorld);
    }

    void Update()
    {
        Vector2 playerWorldPosition = new(player.position.x, player.position.z);
        UpdateMinimap(playerWorldPosition);
    }

    void UpdateMinimap(Vector2 playerWorldPosition)
    {
        Vector2 minimapPosition = ConvertWorldToMinimap(playerWorldPosition);
        Debug.Log(minimapPosition);

        Vector2 adjustedPosition = new(minimapPosition.x + 19f, minimapPosition.y - 22f);

        Debug.Log(adjustedPosition);

        Vector2 clampedPosition = new(
          Mathf.Clamp(adjustedPosition.x, extentMap.x / 3f + minimapMin.x, (2.0f / 3.0f) * extentMap.x - minimapMax.x),
          Mathf.Clamp(adjustedPosition.y, extentMap.y / 3f + minimapMin.y, (2.0f / 3.0f) * extentMap.y - minimapMax.y)
        );

        minimapImage.anchoredPosition = clampedPosition;

        Vector2 markerPosition = (clampedPosition != adjustedPosition) ?
                                 clampedPosition - adjustedPosition :
                                 Vector2.zero;

        playerMarker.anchoredPosition = markerPosition; 
    }

    Vector2 ConvertWorldToMinimap(Vector2 worldPosition)
    {
        float conversionFactorX = extentMap.x / extentWorld.x;
        float conversionFactorY = extentMap.y / extentWorld.y;

        Vector2 normalizedPosition = new(
            (worldPosition.x - worldMin.x) * conversionFactorX + minimapMin.x,
            (worldPosition.y - worldMin.y) * conversionFactorY + minimapMin.y
        );

        return new(-normalizedPosition.x, -normalizedPosition.y);
    }
}
