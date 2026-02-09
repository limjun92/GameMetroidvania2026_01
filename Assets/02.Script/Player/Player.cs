using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 전반적인 상태를 관리하고, 아이템 획득 등의 상호작용을 처리하는 메인 클래스입니다.
/// MovementController 등 컴포넌트를 초기화하고 외부 오브젝트와의 충돌 로직을 담당합니다.
/// </summary>
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
                switch (artifact.type)
                {
                    case ArtifactType.DoubleJump:
                        movementController.canDoubleJump = true;
                        movementController.amountOfJumps = 2; // 점프 횟수 증가
                        break;
                    case ArtifactType.Dash:
                        movementController.isDashUnlocked = true;
                        break;
                    case ArtifactType.WallJump:
                        movementController.isWallJumpUnlocked = true;
                        break;
                }
            }
            
            Destroy(collision.gameObject);
        }
    }

}
