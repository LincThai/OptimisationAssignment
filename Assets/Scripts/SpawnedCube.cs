using UnityEngine;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections;

public class SpawnedCube : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Vector2 xVelocityRange = new(-1, 1);
    [SerializeField] private float verticalVelocity = 10;
    [SerializeField] private Vector2 zVelocityRange = new(-1, 1);

    [SerializeField] private int lifetime = 100;

    private Coroutine lifetimeCoroutine;
    private WaitForSeconds lifetimeWait;

    private void Awake() 
    {
        lifetimeWait = new WaitForSeconds(lifetime);
    }

    private void OnEnabled()
    {
        lifetimeCoroutine = StartCoroutine(SetLifetime());
    }

    private void OnDisabled()
    {
        if (lifetimeCoroutine != null)
            StopCoroutine(lifetimeCoroutine);
    }

    private IEnumerator SetLifetime()
    {
        yield return lifetimeWait;

        if (gameObject != null && gameObject.activeSelf)
            CubeSpawner.Instance.CubePool.Release(this);
    }

    public void Initialized()
    {
        SetRandomVelocity();
    }

    public void SetRandomVelocity()
    {

		rb.velocity = new Vector3(
            Random.Range(xVelocityRange.x, xVelocityRange.y),
            verticalVelocity, 
            Random.Range(zVelocityRange.x, zVelocityRange.y));

        rb.angularVelocity = new Vector3(
            Random.Range(-10, 10), 
            Random.Range(-10, 10), 
            Random.Range(-10, 10));
    }
}
