using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAbstact : MonoBehaviour
{
    //Hằng số chỉ số của đạn
    protected const int constDamage = 1;
    protected const float constSpeed = 2;
    protected const int constHealth = 1;

    //Chỉ số của đạn
    private float speed;
    [SerializeField] private int damage;
    [SerializeField] private int health;
    Vector3 shootDirection;

    //setter, getter
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    void rotateInShootDirection()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, shootDirection);
    }

    public void fireProjectile(Ray shootRay)
    {
        shootDirection = shootRay.direction;
        transform.position = shootRay.origin;
        rotateInShootDirection();
    }

    //Gây dame khi đụng trúng kẻ địch
    private void takeDamege(Collision enemy)
    {
        enemy.gameObject.GetComponent<ObjectAbstract>().beAttacked(damage, this.tag);
    }

    //Truyền đạn khi đụng trúng đồng minh
    private void addBullet(Collision ally)
    {
        ally.gameObject.GetComponent<ObjectAbstract>().receive(damage);
    }

    //Khi bị đụng trúng đạn địch
    private void beHit(Collision bullet)
    {
        Health = Health - bullet.gameObject.GetComponent<BulletAbstact>().Damage;
        if (Health <= 0) Destroy(this.gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!CustomTime.IsPaused)
            transform.Translate(shootDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ObjectAbstract objectAbstract = collision.collider.GetComponent<ObjectAbstract>();
        //Nếu đụng trúng Object
        //if (collision.gameObject.tag == "Object"
        //    || collision.gameObject.tag == "Enemy" 
        //    || collision.gameObject.tag == "Anonymous")
        if (objectAbstract != null)
        {
            Destroy(this.gameObject);

            if (this.gameObject.tag == objectAbstract.TagBulletCharacter) 
            {
                if (collision.gameObject.tag == objectAbstract.TagCharacter) 
                    addBullet(collision);
                else takeDamege(collision);
            } else if(this.gameObject.tag == objectAbstract.TagBulletEnemy)
            {
                if (collision.gameObject.tag == objectAbstract.TagEnemy) addBullet(collision);
                else takeDamege(collision);
            }
            return;
        }

        //Nếu đụng trúng đạn
        BulletAbstact bulletAbstact = collision.collider.GetComponent<BulletAbstact>();
        //if (collision.gameObject.tag == "BulletObject" || collision.gameObject.tag == "BulletEnemy")
        if (bulletAbstact != null)
        {
            if (this.gameObject.tag != collision.gameObject.tag) beHit(collision);
            return;
        }
        
    }
}
