using UnityEngine;
using System.Collections;

public class UIElementMover : MonoBehaviour
{
    [SerializeField] private float _duration = 0.3f;

    public void Move(GameObject target, Vector3 from, Vector3 to)
    {
        StartCoroutine(MoveCoroutine(target, from, to));
    }

    private IEnumerator MoveCoroutine(GameObject target, Vector3 from, Vector3 to)
    {
        if (target == null) yield break;

        target.transform.localPosition = from;
        target.SetActive(true);

        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.unscaledDeltaTime;
            target.transform.localPosition = Vector3.Lerp(from, to, elapsed / _duration);
            yield return null;
        }

        target.transform.localPosition = to;
    }
}
