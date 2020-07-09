using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBlock : Block
{
    // variable
    [SerializeField] GameObject bulletPrefab = null;

    private float time;
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        this.blockType = BlockType.STANDARD;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(time > param.blockCoolTime)
        {
            Shoot();
            time = 0;
        }

    }

    void Shoot()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = new Vector3(10, 0, 0);
        Destroy(bullet, 5.0f);
    }
}
