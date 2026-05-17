using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RadarManager : MonoBehaviour
{
    public Transform playerTransform;
    public Transform radarMaskTransform;
    public Transform radarVisionCastTransform;
    public GameObject blipPrefab;
    public float radarRange = 20.0f;
    public float radarRadius = 100.0f;

    private Dictionary<RadarEntity, RectTransform> enemyBlips = new Dictionary<RadarEntity, RectTransform>();

    // Start is called first, used to set the player blip on the radar
    void Start()
    {
        GameObject playerBlip = Instantiate(blipPrefab,radarMaskTransform);
        playerBlip.GetComponent<Image>().color = Color.white;

        RectTransform playerBlipRect = playerBlip.GetComponent<RectTransform>();
        playerBlipRect.anchoredPosition = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the vision cone player is facing
        UpdateVisionCone();

        // Step 1: find every object with radarEntity script attached; finds any objects with this radar entity script
        RadarEntity[] radarEntities = FindObjectsByType<RadarEntity>();

        // Step 2: Put things into a dictionary for which enemy matches which blip, so they can be erased at same time
        foreach(RadarEntity enemy in radarEntities)
        {
            if (!enemyBlips.ContainsKey(enemy))
            {
                CreateBlip(enemy);
            }
            // Step 3: Update positions based on enemies
            UpdateBlipPosition(enemy);
        }

        // Step 4: clean destroyed blips
        CleanupBlips();
    }

    void CreateBlip(RadarEntity enemy)
    {
        GameObject newBlip = Instantiate(blipPrefab,radarMaskTransform);
        RectTransform rt = newBlip.GetComponent<RectTransform>();

        newBlip.GetComponent<Image>().color = enemy.blipColor;

        enemyBlips.Add(enemy,rt);
    }

    void UpdateBlipPosition(RadarEntity enemy)
    {
        //Relative distance = enemy position - player position
        Vector3 distance = enemy.transform.position - playerTransform.position;

        float scaleFactor = radarRadius / radarRange;

        Vector2 radarPos = new Vector2(distance.x * scaleFactor,distance.z * scaleFactor);

        RectTransform blipRect = enemyBlips[enemy];

        float clampLimit = radarRadius - 5f;

        // Check if the radar position is out of the radar radius, clamp to edge of radar
        // Square magnitude to avoid square root calculation on normal magnitude (pythagorean theorem)
        if(radarPos.sqrMagnitude >= (clampLimit * clampLimit))
        {
            radarPos = radarPos.normalized * clampLimit;
        }

        blipRect.anchoredPosition = radarPos;
    }

    void CleanupBlips()
    {
        // Create a list of entities that need to be removed
        List<RadarEntity> toRemove = new List<RadarEntity>();
        foreach(var pair in enemyBlips)
        {
            // If the key is null (enemy destroyed), then add it to the to Remove
            if(pair.Key == null)
            {
                toRemove.Add(pair.Key);
            }
        }

        foreach(var enemy in toRemove)
        {
            Destroy(enemyBlips[enemy].gameObject);
            enemyBlips.Remove(enemy);
        }
    }

    // Updates the visual representation of where the player can see
    void UpdateVisionCone()
    {
        // Gets its angles around the y I imagine
        float playerRotation = playerTransform.eulerAngles.y;
        // For some reason, need 45 degree offset for the cone to work correctly (probably because of weird intial positioning)
        Vector3 newRotate = new Vector3(0,0,-playerRotation + 45);
        radarVisionCastTransform.localEulerAngles = newRotate;
    }

    
}
