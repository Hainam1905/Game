public class Bullet : BulletAbstact
{
    protected void Awake()
    {
        Speed = constSpeed;
        Damage = constDamage;
        Health = constHealth;
    }
}
