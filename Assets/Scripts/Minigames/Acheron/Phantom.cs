using System;
using System.Collections;
using UnityEngine;

public class Phantom : Enemy
{
    private Platform platform;

    private int borderIdx;
    private bool isFollowingPlayer;

    [SerializeField] private float enemyHeight = 0.8f;

    public void Load(Platform givenPlatform)
    {
        platform = givenPlatform;

        if (player == null)
            player = FindFirstObjectByType<Player>();

        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            Vector2 borderLocation = platform.borderLocations[borderIdx].position;
            Vector3 targetPos = new(borderLocation.x, borderLocation.y + enemyHeight, 0);

            while (!Mathf.Approximately(transform.position.x, targetPos.x))
            {
                if (isFollowingPlayer)
                {
                    Vector3 playerPos = new(Mathf.Clamp(player.transform.position.x, platform.borderLocations[1].position.x, platform.borderLocations[0].position.x), targetPos.y, 0);
                    transform.position = Vector3.MoveTowards(transform.position, playerPos, Time.deltaTime * moveSpeed);
                }
                else
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
                yield return null;
            }

            borderIdx++;
            if (borderIdx > platform.borderLocations.Length - 1)
                borderIdx = 0;

            spriteRenderer.flipX = borderIdx != 0;

            yield return null;
        }
    }

    public void FollowPlayer(bool toggle)
    {
        isFollowingPlayer = toggle;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            player.Hit();
    }
}
