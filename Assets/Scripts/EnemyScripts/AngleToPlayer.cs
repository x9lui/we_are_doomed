using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AngleToPlayer : MonoBehaviour
{

    private Transform player;
    private Vector3 targetPos;
    private Vector3 targetDir;
    
    private float angle;
    public int lastIndex;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        targetDir = targetPos - transform.position;

        //Get angle
        angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

        UnityEngine.Vector3 tempScale = UnityEngine.Vector3.one;
        if(angle >0){
            tempScale.x = -1;
        }

        spriteRenderer.transform.localScale = tempScale;
        lastIndex = GetIndex(angle);

    }

    private int GetIndex(float angle) {
        //front
        if(angle > -22.5f && angle < 22.6f){
            return 0;
        }
        if(angle >= 22.5f && angle < 67.5f){
            return 7;
        }
        if(angle >= 67.5f && angle < 112.5f){
            return 6;
        }
        if(angle >= 112.5f && angle < 157.5f){
            return 5;
        }

        //back
        if(angle <= -157.5 || angle >= 157.5f){
            return 4;
        }
        if(angle >= -157.4f && angle < -112.5f){
            return 3;
        }
        if(angle >= -112.5f && angle < -67.5f){
            return 2;
        }
        if(angle >= -67.5f && angle < -22.5f){
            return 1;
        }
        return lastIndex;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, targetPos);
    }

}
