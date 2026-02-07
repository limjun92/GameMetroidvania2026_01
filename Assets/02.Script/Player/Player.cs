using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private MovementController movementController;

    void Start()
    {
        // 같은 오브젝트 안에 있는 MovementController를 찾아서 저장
        movementController = GetComponent<MovementController>();
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
                    movementController.canDoubleJump = true;
                    movementController.amountOfJumps = 2; // 점프 횟수 증가
                }
                else if (artifact.name == "Dash") // 새로운 아이템 이름
                {
                    movementController.isDashUnlocked = true;
                }
            }
            
            Destroy(collision.gameObject);
        }
    }

}
