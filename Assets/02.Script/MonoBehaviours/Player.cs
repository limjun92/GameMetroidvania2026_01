using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerJump playerJump;

    void Start()
    {
        // 같은 오브젝트 안에 있는 MovementController를 찾아서 저장
        playerJump = GetComponent<PlayerJump>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CanGet"))
        {
            Artifact artifact = collision.gameObject.GetComponent<Collectible>().artifact;

            if (artifact != null)
            {
                if (artifact.name == "DoubleJump")
                {
                    playerJump.canDoubleJump = true;
                }
            }
            
            Destroy(collision.gameObject);
        }
    }

}
