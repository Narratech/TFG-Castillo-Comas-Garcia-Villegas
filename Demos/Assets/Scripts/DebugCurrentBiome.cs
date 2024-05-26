using UnityEngine;

public class DebugCurrentBiome : MonoBehaviour
{
    [SerializeField]
    MapGenerator map;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float maxInfl = 0;
        Biome biome = null;

        var infl = map.GetBiomeInfluencesAt(transform.position);

        foreach (var item in infl)
        {
            if (item.Value > maxInfl)
            {
                maxInfl = item.Value;
                biome = item.Key;
            }
        }

        Debug.Log(biome);
    }
}
