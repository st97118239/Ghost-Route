using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private AcheronManager acheronManager;

    [SerializeField] private bool hasEnemy;
    [SerializeField] private Transform enemySpawnLocation;
    public Transform[] borderLocations;

    private Phantom phantom;

    private void Awake()
    {
        if (acheronManager == null)
            acheronManager = FindFirstObjectByType<AcheronManager>();

        if (hasEnemy)
            SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        phantom = Instantiate(acheronManager.GetPhantomPrefab(), enemySpawnLocation.position, Quaternion.identity, transform).GetComponent<Phantom>();
        phantom.Load(this);
    }

    public void HitPlatform(bool toggle)
    {
        if (hasEnemy)
            phantom.FollowPlayer(toggle);
    }
}
