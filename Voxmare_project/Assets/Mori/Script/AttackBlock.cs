using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBlock : Block
{
    // variable
    [SerializeField] GameObject bulletPrefab = null;

    private float time;
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        blockType = BlockType.ATTACK;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > param.blockCoolTime)
        {
            Shoot();
            time = 0;
        }

    }

    void Shoot()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 10);
        Destroy(bullet, 5.0f);
    }
}
