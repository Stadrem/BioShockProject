using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBomb : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        Damaged dam = other.gameObject.GetComponent<Damaged>();
        if (dam != null)
        {
            dam.Damage(5, "type");
        }
        Destroy(this); // 충돌 후 컴포넌트를 제거하여 계속해서 데미지를 입히지 않도록 함

        BossDamaged Bodam = other.gameObject.GetComponent<BossDamaged>();
        if(Bodam != null)
        {
            Bodam.Damaged(5, "type");
        }
        Destroy(this); // 충돌 후 컴포넌트를 제거하여 계속해서 데미지를 입히지 않도록 함
    }
}
