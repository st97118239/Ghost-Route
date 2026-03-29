using System.Collections;
using UnityEngine;

public class Ferryman : Enemy
{
    [SerializeField] private Transform[] borderLocations;
    private int borderIdx;

    [SerializeField] private Transform spear;
    [SerializeField] private SpriteRenderer spearRenderer;
    [SerializeField] private BoxCollider2D spearCollider;

    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite stabSprite;

    [SerializeField] private float distanceUnderPlayer;
    [SerializeField] private float distanceAbovePlatform;
    [SerializeField] private float spearSpeed;
    [SerializeField] private float _spearSpeedTimer;
    private WaitForSeconds spearSpeedTimer;
    [SerializeField] private float _timeUntilSpearDown;
    private WaitForSeconds timeUntilSpearDown;
    [SerializeField] private float _spearDelay;
    private WaitForSeconds spearDelay;

    [SerializeField] private float raycastLength;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask platformLayer;

    private Coroutine spearCoroutine;

    private void Start()
    {
        spearSpeedTimer = new WaitForSeconds(_spearSpeedTimer);
        timeUntilSpearDown = new WaitForSeconds(_timeUntilSpearDown);
        spearDelay = new WaitForSeconds(_spearDelay);

        if (player == null)
            player = FindFirstObjectByType<Player>();

        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            Vector3 targetPos = new(borderLocations[borderIdx].position.x, transform.position.y, transform.position.z);

            while (!Mathf.Approximately(transform.position.x, targetPos.x))
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);

                RaycastHit2D ray = Physics2D.Raycast(transform.position, Vector2.up, raycastLength, playerLayer);
                if (ray.collider != null && spearCoroutine == null) 
                    spearCoroutine = StartCoroutine(Spear(ray.collider.transform.position));

                yield return null;
            }

            borderIdx++;
            if (borderIdx > borderLocations.Length - 1)
                borderIdx = 0;

            if (borderIdx != 0 && !spriteRenderer.flipX)
            {
                spriteRenderer.flipX = true;
                spear.transform.localPosition = new Vector3(-0.06f, spear.transform.localPosition.y, spear.transform.localPosition.z);
            }
            else if (spriteRenderer.flipX)
            {
                spriteRenderer.flipX = false;
                spear.transform.localPosition = new Vector3(0.06f, spear.transform.localPosition.y, spear.transform.localPosition.z);
            }

            yield return null;
        }
    }

    private IEnumerator Spear(Vector3 playerPos)
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, Vector2.up, raycastLength, platformLayer);

        if (ray.collider != null)
            playerPos = new Vector3(playerPos.x, ray.collider.transform.position.y + distanceAbovePlatform, playerPos.z);
        else
            playerPos += Vector3.down * distanceUnderPlayer;

        float totalHeight = playerPos.y;
        float currentHeight = spear.position.y;

        spriteRenderer.sprite = stabSprite;

        float speed = spearSpeed;
        while (currentHeight < totalHeight)
        {
            float difference = totalHeight - currentHeight;
            if (speed > difference) speed = difference;
            currentHeight += speed;

            spearRenderer.size = new Vector2(spearRenderer.size.x, spearRenderer.size.y + speed);
            spearCollider.offset = new Vector2(spearCollider.offset.x, spearCollider.offset.y + speed);

            yield return spearSpeedTimer;
        }

        yield return timeUntilSpearDown;

        totalHeight = spear.position.y;
        currentHeight = playerPos.y;

        speed = spearSpeed;
        while (currentHeight > totalHeight)
        {
            float difference = currentHeight - totalHeight;
            if (speed > difference) speed = difference;
            currentHeight -= speed;

            spearRenderer.size = new Vector2(spearRenderer.size.x, spearRenderer.size.y - speed);
            spearCollider.offset = new Vector2(spearCollider.offset.x, spearCollider.offset.y - speed);

            yield return spearSpeedTimer;
        }

        spriteRenderer.sprite = defaultSprite;

        yield return spearDelay;

        spearCoroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            player.Hit();
    }
}
