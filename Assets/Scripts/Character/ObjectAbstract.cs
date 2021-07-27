using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ObjectAbstract : MonoBehaviour
{
    private SoundManager soundManager;
    
    //AI enemy properties
    private AIEnemy aIEnemy;
    public ObjectAbstract cameFrom = null;
    public float g = 0f;
    public float h, f = 0f;
    protected float benefitCharacter;
    protected float benefitAnonymous;
    [SerializeField] private ObjectAbstract targetAI;

    //Object
    protected ObjectAbstract objectAbstract, enemyObject, shieldObject,
        enemyShieldObject, fastCharacter, enemyFastCharacter;
    private string tagCharacter, tagEnemy;
    private string tagAnonymous = "Anonymous";
    //Bullet
    protected GameObject bulletPrefab, powerBulletPrefab, enemyBulletPrefab, enemyPowerBulletPrefab;
    private string tagBulletCharacter, tagBulletEnemy;
    //LineRenderer
    private List<LineRenderer> LineRenderers = new List<LineRenderer>();
    //LineMaterial
    private Material lineMaterial;
    
    //Path Object
    private const string pathCharacter = "Prefabs/Objects/Character/Character";
    private const string pathFastLoadCharacter = "Prefabs/Objects/Character/FastLoadCharacter";
    private const string pathShieldCharacter = "Prefabs/Objects/Character/ShieldCharacter";
    private const string pathEnemy = "Prefabs/Objects/Enemy/Enemy";
    private const string pathFastLoadEnemy = "Prefabs/Objects/Enemy/FastLoadEnemy";
    private const string pathShieldEnemy = "Prefabs/Objects/Enemy/ShieldEnemy";
    //Path Bullet
    private const string pathBullet = "Prefabs/Objects/Bullet/Bullet";
    private const string pathEnemyBullet = "Prefabs/Objects/Bullet/EnemyBullet";
    private const string pathPowerBullet = "Prefabs/Objects/Bullet/PowerBullet";
    private const string pathEnemyPowerBullet = "Prefabs/Objects/Bullet/EnemyPowerBullet";
    //Path Material
    private const string pathLineMaterial = "Materials/LineLink/LineLinkNormalColor";

    //Hằng số của vật
    private const int maxBullet = 20; //Số lượng đạn tối đa
//    private const int increasePowerBullet = 3; //Sức mạnh của đạn được tăng lên sau khi full đạn
//    private const float speedBullet = 3.0f; //Tốc độ bay của đạn
    private const float timeAttack = .25f; //Khoảng thời gian bắn giữa 2 viên đạn
    private const float timeLoadShield = 2f; //Thời gian hồi phục 1 shield

    //Hằng số chỉ số của vật
    protected const int constBulletBegin = 0;
    //protected const int constShieldBegin = 1;
    protected const int constMaxShield = 8;
    protected const float constTimeLoadBullet = 1.5f;
    protected const float constBenefitAnonymous = 6f;
    protected const float constBenefitCharacter = 8f;

    //Biến kiểm tra double kick
    protected const float doubleClickTime = .2f; //Nếu click liên tiếp 2 lần dưới thời gian này là double click
    protected float lastClickTime; //Thời điểm của lần click trước

    //Thuộc tích của vật
    [SerializeField] private int shieldAmount, bulletAmount, powerBulletAmount;
    private int maxShield;
    private float timeLoadBullet; //Thời gian nạp đạn
    [SerializeField] private List<ObjectAbstract> link = new List<ObjectAbstract>();
    
    //Raycas để tìm object liên kết (linkTo)
    private ObjectAbstract linkObject = null;
    private Vector3 oldPositionLink;
    private float rayLength = 0f;
    private bool boolLinked = false; //Kiểm tra xem đã liên kết được chưa

    //Biến để tấn công (attackTo)
    private float countAttack = 0; //đếm thời gian cho viên đạt kế tiếp
    private bool boolAttacked = false; //Có đồng ý tấn công không

    //Biến nạp đạp (load)
    private float countLoadBullet = 0; //đếm thời gian cho lần nạp đạn kế tiếp
    private float countLoadShield = 0;

    //Animation
    private Animator animPowerUp;

    //Tỏa sáng khi Power
    private ParticleSystem particle;
    private LineRenderer chooseEffect;
    public ParticleSystem FXGenerate;

    //Setter, getter
    public string TagBulletCharacter
    {
        get { return tagBulletCharacter; }
    }
    public string TagBulletEnemy
    {
        get { return tagBulletEnemy; }
    }
    public string TagCharacter
    {
        get { return tagCharacter; }
    }
    public string TagEnemy
    {
        get { return tagEnemy; }
    }
    public string TagAnonymous
    {
        get { return tagAnonymous; }
    }
    public Boolean BoolLinked
    {
        get { return boolLinked; }
    }
    public Boolean BoolAttacked
    {
        get { return boolAttacked; }
    }    
    public ObjectAbstract TargetAI
    {
        get { return targetAI; }
    }
    public int MaxShield
    {
        get { return maxShield; }
        set { maxShield = value; }
    }
    public float TimeLoadBullet
    {
        get { return timeLoadBullet; }
        set { timeLoadBullet = value; }
    }
    public int ShieldAmount
    {
        get { return shieldAmount; }
        set
        {
            if (value < 0) shieldAmount = 0;
            else if (value > MaxShield) shieldAmount = MaxShield;
            else shieldAmount = value;
        }
    }
    public int BulletAmount
    {
        get { return bulletAmount; }
        set
        {
            if (value < 0) bulletAmount = 0;
            else if (value > maxBullet) bulletAmount = maxBullet;
            else bulletAmount = value;
        }
    }
    public int PowerBulletAmount
    {
        get { return powerBulletAmount; }
        set
        {
            if (value < 0) powerBulletAmount = 0;
            else if (value > maxBullet) powerBulletAmount = maxBullet;
            else powerBulletAmount = value;
        }
    }
    public List<ObjectAbstract> Link
    {
        get { return link; }
        set { link = value; }
    }

    //Hàm tính chi phí của AI
    public int Waste(ObjectAbstract target)
    {
        foreach (ObjectAbstract objectAbstract in Link)
        {
            if (objectAbstract == target)
            {
                int kq = 0;
                if (target.tag != TagEnemy)
                {
                    kq = target.BulletAmount + target.ShieldAmount;
                    //Debug.Log(target + " " + target.ShieldAmount + " " + target.benefitAnonymous);
                    if (target.tag == TagAnonymous) kq = kq - (int)target.benefitAnonymous;
                    else kq = kq - (int)target.benefitCharacter;
                }
                else kq = 0;
                //Debug.Log(kq);
                return kq;
            }
        }
        throw new ArgumentNullException();
    }

    //Hàm dự đoán chi phí của AI
    public float Distance(ObjectAbstract target)
    {
        float kq = 0;
        if (this.tag != TagEnemy)
        {
            foreach (ObjectAbstract objectAbstract in Link)
            {
                if (objectAbstract.tag == TagAnonymous) kq = kq + 1;
                if (objectAbstract.tag == tagCharacter) kq = kq + 2;
            }
        }
        else return 0;

        return kq;
    }

    //Abstract Funtion
    protected virtual void beDefeated(string tagName)
    {
        if(tagName == tagBulletCharacter)
        {
            GameController.EarningACharacter();
            if (tag == TagEnemy) GameController.DestroyAnEnemy();

            soundManager.PlaySound(soundManager.SoundGenerateObject, 1);
        }
        else if(tagName == tagBulletEnemy)
        {
            GameController.EarningAnEnemy();
            if (tag == tagCharacter) GameController.DestroyACharacter();
        }
    }

    //Load Prefab
    private void loadPrefab()
    {
        //Load Object
        objectAbstract = Resources.Load<ObjectAbstract>(pathCharacter);
        shieldObject = Resources.Load<ObjectAbstract>(pathShieldCharacter);
        fastCharacter = Resources.Load<ObjectAbstract>(pathFastLoadCharacter);
        enemyObject = Resources.Load<ObjectAbstract>(pathEnemy);
        enemyShieldObject = Resources.Load<ObjectAbstract>(pathShieldEnemy);
        enemyFastCharacter = Resources.Load<ObjectAbstract>(pathFastLoadEnemy);

        //Load tag object
        tagCharacter = objectAbstract.tag;
        tagEnemy = enemyObject.tag;

        //Load bullet
        bulletPrefab = Resources.Load<GameObject>(pathBullet);
        enemyBulletPrefab = Resources.Load<GameObject>(pathEnemyBullet);
        powerBulletPrefab = Resources.Load<GameObject>(pathPowerBullet);
        enemyPowerBulletPrefab = Resources.Load<GameObject>(pathEnemyPowerBullet);

        //Load tag bullet
        tagBulletCharacter = bulletPrefab.tag;
        tagBulletEnemy = enemyBulletPrefab.tag;

        //Load material
        lineMaterial = Resources.Load<Material>(pathLineMaterial);
    }

    //Thay đổi lượng đạn hiện ra
    private void changeBulletAppear()
    {
        Image[] shield = GetComponentsInChildren<Image>();

        foreach (Image search in shield)
        {
            if (search.gameObject.name == "BulletAmount")
            {
                search.fillAmount = (float)BulletAmount / maxBullet;
            }
        }
    }
    
    //Hiệu ứng Power
    private void turnOnPowerEffect()
    {
        particle.Play();
    }

    private void turnOffPowerEffect()
    {
        particle.Stop();
    }

    private void biggerEffect()
    {
        animPowerUp.SetBool("BoolBigger", true);
        animPowerUp.SetBool("BoolSmaller", false);

        turnOnPowerEffect();
    }

    private void smallerEffect()
    {
        animPowerUp.SetBool("BoolBigger", false);
        animPowerUp.SetBool("BoolSmaller", true);

        turnOffPowerEffect();
    }

    //Nạp đạn
    public void loadBullet()
    {
        if (countLoadBullet > 0)
        {
            countLoadBullet -= Time.deltaTime;
        } else if (countLoadBullet <= 0 && BulletAmount < maxBullet)
        {
            BulletAmount++;

            changeBulletAppear();

            countLoadBullet = timeLoadBullet;
        } else if (countLoadBullet <= 0 && BulletAmount == maxBullet)
        {
            PowerBulletAmount++;

            countLoadBullet = timeLoadBullet;

            biggerEffect();
        }
        
        if (BulletAmount > maxBullet || BulletAmount < 0)
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    //Hiện 1 Shield
    private void appearShield()
    {
        Image[] shield = GetComponentsInChildren<Image>();

        foreach (Image search in shield)
        {
            for (int i = 1; i <= MaxShield; i++)
            {
                if (ShieldAmount == i) if (search.gameObject.name == "Shield" + i) search.fillAmount = 1;
            }
        }
    }
    private void appearBeginShield(int shieldBegin)
    {
        Image[] shield = GetComponentsInChildren<Image>();

        for (int i = 0; i < shieldBegin; i++)
        {
            shield[i].fillAmount = 1;
        }
    }

    private void appearBeginBullet()
    {
        changeBulletAppear();
        if (bulletAmount == maxBullet) biggerEffect();
    }

    //Nạp shield
    public void loadShield()
    {
        if (countLoadShield > 0)
        {
            countLoadShield -= Time.deltaTime;
        } else if (countLoadShield <= 0 && ShieldAmount < maxShield && ShieldAmount >= 0)
        {
            ShieldAmount++;

            appearShield();

            //Reset thời gian đếm
            countLoadShield = timeLoadShield;
        }
        
        if (ShieldAmount > maxShield || ShieldAmount < 0)
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    private void shootPowerBullet(ObjectAbstract target)
    {
        BulletAmount--;
        PowerBulletAmount--;
        countAttack = timeAttack;

        if(PowerBulletAmount == 0)
        {
            smallerEffect();
        }

        //hiện đạn
        PowerBullet powerBullet;
        if(this.tag == tagCharacter) powerBullet = Instantiate(powerBulletPrefab).GetComponent<PowerBullet>();
        else powerBullet = Instantiate(enemyPowerBulletPrefab).GetComponent<PowerBullet>();

        //chỉnh hướng đạn
        Vector3 direction = target.transform.position - transform.position;

        //đạn bay theo hướng trên
        var shootRay = new Ray(transform.position, direction);

        Physics.IgnoreCollision(this.GetComponent<Collider>(), powerBullet.GetComponent<Collider>());

        powerBullet.fireProjectile(shootRay);
    }

    private void shootBullet(ObjectAbstract target)
    {
        BulletAmount--;
        countAttack = timeAttack;

        Bullet bullet;

        if(this.tag == tagCharacter) bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
        else bullet = Instantiate(enemyBulletPrefab).GetComponent<Bullet>();

        Vector3 direction = target.transform.position - transform.position;
        var shootRay = new Ray(transform.position, direction);

        Physics.IgnoreCollision(this.GetComponent<Collider>(), bullet.GetComponent<Collider>());

        bullet.fireProjectile(shootRay);

        changeBulletAppear();
    }

    //Bắn đạn
    private void attackTo(ObjectAbstract target)
    {
        if(target == null)
        {
            linkTo(oldPositionLink);
        } 
        else
        {
            if (countAttack > 0)//qua mỗi countAttack giây thì bắn một viên
            {
                countAttack -= Time.deltaTime;
            }
            else if (countAttack <= 0 && BulletAmount > 0)//Khi hết đạn thì không bắn được
            {
                if (PowerBulletAmount == 0)
                {
                    shootBullet(target);
                }
                else if (PowerBulletAmount > 0)
                {
                    shootPowerBullet(target);   
                }
            }
        }
    }

    //Làm 1 shield biến mất
    private void disappearShield()
    {
        Image[] shield = GetComponentsInChildren<Image>();

        foreach (Image search in shield)
        {
            for (int i = 0; i < MaxShield; i++)
            {
                if (ShieldAmount == i)
                {
                    for(int j = i; j < MaxShield; j++) 
                        if (search.gameObject.name == "Shield" + (j + 1)) search.fillAmount = 0;
                }
            }
        }
    }

    //Bị bắn bởi địch
    public void beAttacked(int damage, string tagEnemy)
    {
        if(damage < 0 || tagEnemy == string.Empty)
        {
            throw new ArgumentNullException();
        } else
        {
            if (ShieldAmount > 0)
            {
                ShieldAmount = ShieldAmount - damage;

                disappearShield();
            } else if (ShieldAmount <= 0 && PowerBulletAmount > 0)
            {
                PowerBulletAmount = PowerBulletAmount - damage;

                if (PowerBulletAmount <= 0)
                {
                    smallerEffect();
                }
            } else if (ShieldAmount <= 0 && BulletAmount > 0)
            {
                BulletAmount = BulletAmount - damage;
            } else if (ShieldAmount <= 0 && BulletAmount <= 0)
            {
                beDefeated(tagEnemy);
            }
            
            if(ShieldAmount < 0 || BulletAmount < 0 || ShieldAmount > MaxShield || BulletAmount > maxBullet)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    //Được truyền đạn bởi đồng minh
    public void receive(int damage)
    {
        if (damage < 0)
        {
            throw new ArgumentNullException();
        } else
        {
            if (BulletAmount < maxBullet)
            {
                BulletAmount = BulletAmount + damage;

                changeBulletAppear();
            } else if (BulletAmount == maxBullet && PowerBulletAmount < maxBullet)
            {
                PowerBulletAmount = PowerBulletAmount + damage;

            } else if (PowerBulletAmount == maxBullet)
            {
                //Hiệu ứng power
            }

            if (PowerBulletAmount < 0 || BulletAmount < 0 || PowerBulletAmount > maxBullet || BulletAmount > maxBullet)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    //Liên kết với object khac để tấn công
    public void linkTo() //Vector3 direction
    {
        //Lấy vị trí chuột
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);

        RaycastHit tempLinkOject;
        //Tính khoảng cách từ vật đến nơi thả chuột d = căn(bình(x2 - x1) + bình(y2 - y1))
        rayLength = (float)Math.Sqrt(Math.Pow(mousePos.x - transform.position.x, 2) 
                                    + Math.Pow(mousePos.y - transform.position.y, 2));

        //tạo raycast để tìm vật liên kết
        if (Physics.Raycast(transform.position, mousePos - transform.position, out tempLinkOject, rayLength))
        {
            if (tempLinkOject.collider.tag == this.gameObject.tag) //có phải đồng minh hay kẻ địch không?
            {
                ObjectAbstract objectAbstract = tempLinkOject.collider.GetComponent<ObjectAbstract>();
                if (objectAbstract != null)
                {
                    foreach (ObjectAbstract search in link)
                    {
                        if (objectAbstract == search.GetComponent<ObjectAbstract>())
                        {
                            boolLinked = true; //đã liên kết
                            boolAttacked = true; //đã tấn công
                            linkObject = objectAbstract; //xác định object liên kết tới
                            //Lưu lại vị trí vật đã liên kết
                            oldPositionLink = tempLinkOject.collider.GetComponent<ObjectAbstract>().transform.position;

                            if (objectAbstract.linkObject != null)
                            {
                                if (objectAbstract.linkObject == this.gameObject.GetComponent<ObjectAbstract>())
                                {
                                    /*Nếu object liên kết tới đã liên kết với object này rồi 
                                    thì object đó sẽ ngừng liên kết với object này*/
                                    objectAbstract.boolLinked = false;
                                    objectAbstract.boolAttacked = false;
                                    objectAbstract.linkObject = linkObject;
                                }
                            }
                            return;
                        }
                    }
                }
            }
            else if (tempLinkOject.collider.tag != this.gameObject.tag) //Có phải kẻ định không?
            {
                ObjectAbstract objectAbstract = tempLinkOject.collider.GetComponent<ObjectAbstract>();
                if (objectAbstract != null)
                {
                    foreach (ObjectAbstract search in link)
                    {
                        if (objectAbstract == search.GetComponent<ObjectAbstract>())
                        {
                            boolLinked = true; //đã liên kết
                            boolAttacked = true; //đã tấn công
                            linkObject = objectAbstract; //xác định object liên kết tới
                            //Lưu lại vị trí vật đã liên kết
                            oldPositionLink = objectAbstract.transform.position;

                            return;
                        }
                    }
                }
            }
        }

        return;
    }
    public void linkTo(ObjectAbstract target) //Vector3 direction
    {
        try
        {
            if (target.tag == this.gameObject.tag) //có phải đồng minh hay kẻ địch không?
            {
                //ObjectAbstract objectAbstract = tempLinkOject.collider.GetComponent<ObjectAbstract>();
                if (target != null)
                {
                    foreach (ObjectAbstract search in link)
                    {
                        if (target == search.GetComponent<ObjectAbstract>())
                        {
                            boolLinked = true; //đã liên kết
                            boolAttacked = true; //đã tấn công
                            linkObject = target; //xác định object liên kết tới
                                                 //Lưu lại vị trí vật đã liên kết
                            oldPositionLink = target.transform.position;

                            if (target.linkObject != null)
                            {
                                if (target.linkObject == this.gameObject.GetComponent<ObjectAbstract>())
                                {
                                    /*Nếu object liên kết tới đã liên kết với object này rồi 
                                    thì object đó sẽ ngừng liên kết với object này*/
                                    target.boolLinked = false;
                                    target.boolAttacked = false;
                                    target.linkObject = null;
                                }
                            }
                            return;
                        }
                    }
                }
            }
            else if (target.tag != this.gameObject.tag) //Có phải kẻ định không?
            {
                //ObjectAbstract objectAbstract = tempLinkOject.collider.GetComponent<ObjectAbstract>();
                if (target != null)
                {
                    foreach (ObjectAbstract search in link)
                    {
                        if (target == search.GetComponent<ObjectAbstract>())
                        {
                            boolLinked = true; //đã liên kết
                            boolAttacked = true; //đã tấn công
                            linkObject = target; //xác định object liên kết tới
                                                 //Lưu lại vị trí vật đã liên kết
                            oldPositionLink = target.transform.position;

                            return;
                        }
                    }
                }
            }
        }
        catch (MissingReferenceException)
        {
            linkTo(oldPositionLink);
        }

        return;
    }
    public void linkTo(Vector3 targetPosition) //Vector3 direction
    {
        //Lấy vị trí chuột
        Vector3 mousePos = targetPosition;
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);

        RaycastHit tempLinkOject;
        //Tính khoảng cách từ vật đến nơi thả chuột d = căn(bình(x2 - x1) + bình(y2 - y1))
        rayLength = (float)Math.Sqrt(Math.Pow(mousePos.x - transform.position.x, 2)
                                    + Math.Pow(mousePos.y - transform.position.y, 2));

        //Debug.DrawRay(transform.position, mousePos - transform.position, Color.red, 5f);
        //tạo raycast để tìm vật liên kết
        if (Physics.Raycast(transform.position, mousePos - transform.position, out tempLinkOject, rayLength))
        {
            if (tempLinkOject.collider.tag == this.gameObject.tag) //có phải đồng minh hay kẻ địch không?
            {
                ObjectAbstract objectAbstract = tempLinkOject.collider.GetComponent<ObjectAbstract>();
                if (objectAbstract != null)
                {
                    foreach (ObjectAbstract search in link)
                    {
                        if (objectAbstract == search.GetComponent<ObjectAbstract>())
                        {
                            boolLinked = true; //đã liên kết
                            boolAttacked = true; //đã tấn công
                            linkObject = objectAbstract; //xác định object liên kết tới
                            //Lưu lại vị trí vật đã liên kết
                            oldPositionLink = tempLinkOject.collider.GetComponent<ObjectAbstract>().transform.position;

                            if (objectAbstract.linkObject != null)
                            {
                                if (objectAbstract.linkObject == this.gameObject.GetComponent<ObjectAbstract>())
                                {
                                    /*Nếu object liên kết tới đã liên kết với object này rồi 
                                    thì object đó sẽ ngừng liên kết với object này*/
                                    objectAbstract.boolLinked = false;
                                    objectAbstract.boolAttacked = false;
                                    objectAbstract.linkObject = linkObject;
                                }
                            }
                            return;
                        }
                    }
                }
            }
            else if (tempLinkOject.collider.tag != this.gameObject.tag) //Có phải kẻ định không?
            {
                ObjectAbstract objectAbstract = tempLinkOject.collider.GetComponent<ObjectAbstract>();
                if (objectAbstract != null)
                {
                    foreach (ObjectAbstract search in link)
                    {
                        if (objectAbstract == search.GetComponent<ObjectAbstract>())
                        {
                            boolLinked = true; //đã liên kết
                            boolAttacked = true; //đã tấn công
                            linkObject = objectAbstract; //xác định object liên kết tới
                            //Lưu lại vị trí vật đã liên kết
                            oldPositionLink = objectAbstract.transform.position;

                            return;
                        }
                    }
                }
            }
        }

        return;
    }

    //initialization LineRenderer
    private void initLineRenderer()
    {
        for(int i = 0; i < Link.Count; i ++)
        {
            GameObject emptyObject = new GameObject("Link Line " + (i + 1));
            emptyObject.transform.parent = this.gameObject.transform;
            
            //Khởi tạo line
            LineRenderers.Add(emptyObject.AddComponent<LineRenderer>());
            LineRenderers[i].material = lineMaterial;
            //LineRenderers[i].startColor = new Color(119, 91, 147);
            //LineRenderers[i].endColor = new Color(119, 91, 147);
            LineRenderers[i].startWidth = .05f;
            LineRenderers[i].endWidth = .05f;
            LineRenderers[i].positionCount = 2;
            LineRenderers[i].SetPosition(0, this.transform.position + new Vector3(0, 0, 0.5f));
            LineRenderers[i].SetPosition(1, Link[i].transform.position + new Vector3(0, 0, 0.5f));// + Vector3.forward);
        }
    }

    IEnumerator CreateAIEnemyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        try
        {
            aIEnemy = new AIEnemy(this);
            aIEnemy.AFind(this, GameObject.FindGameObjectWithTag(tagCharacter).GetComponent<ObjectAbstract>());

            aIEnemy.path.Pop();
            targetAI = aIEnemy.path.Pop();

            if (targetAI.tag != tagCharacter) this.linkTo(targetAI);
            if (targetAI.tag == TagEnemy) targetAI.CreateAIEnemy();
        }
        catch (NullReferenceException)
        {
            Debug.Log("Đã chiếm hết Object");
        }
    }

    //Create AI Enemy
    private void CreateAIEnemy()
    {
        try
        {
            aIEnemy = new AIEnemy(this);
            aIEnemy.AFind(this, GameObject.FindGameObjectWithTag(tagCharacter).GetComponent<ObjectAbstract>());
            aIEnemy.path.Pop();
            targetAI = aIEnemy.path.Pop();

            if (targetAI.tag != tagCharacter) this.linkTo(targetAI);
            if (targetAI.tag == TagEnemy) targetAI.CreateAIEnemy();
        }
        catch (NullReferenceException)
        {
            StartCoroutine(CreateAIEnemyAfter(1));
        }
    }

    //check can this enemy destroy target object
    private Boolean CanWin(ObjectAbstract target)
    {
        if (target.BulletAmount + target.ShieldAmount + target.PowerBulletAmount
            - this.BulletAmount - this.PowerBulletAmount * 3 <= 0) return true;
        else return false;
    }

    private void CancelLink()
    {
        boolLinked = false;
        boolAttacked = false;
        linkObject = null;
        //countAttack = 0;
    }

    private Boolean IsLink()
    {
        if (boolLinked == false && boolAttacked == false && linkObject == null) return false;
        else return true;
    }

    protected void turnOnGenerateFX()
    {
        if (FXGenerate)
        {
            FXGenerate.Play();
        }
    }

    private void OnMouseUp()
    {
        if (this.tag == tagCharacter)
        {
            linkTo();
            if (chooseEffect.gameObject.activeSelf == true) chooseEffect.gameObject.SetActive(false);
        }
    }

    private void OnMouseOver()
    {
        if (this.tag == tagCharacter)
        {
            if (Input.GetMouseButtonDown(0))
            {
                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick <= doubleClickTime)
                {
                    CancelLink();
                }

                lastClickTime = Time.time;
            }
        }
    }

    private void OnMouseDown()
    {
        if (this.tag == TagCharacter)
        {
            chooseEffect.gameObject.SetActive(true);
        }
    }

    protected virtual void Awake()
    {
        //Khởi tạo chỉ số
        //ShieldAmount = constShieldBegin;
        //BulletAmount = constBulletBegin;
        MaxShield = constMaxShield;
        TimeLoadBullet = constTimeLoadBullet;

        benefitAnonymous = constBenefitAnonymous;
        benefitCharacter = constBenefitCharacter;
    }

    private void Start()
    {
        //Set Prefab
        loadPrefab();
        //Khởi tạo line
        initLineRenderer();
        //Hiện shield khởi đầu
        appearBeginShield(shieldAmount);

        soundManager = FindObjectOfType<SoundManager>();

        if (this.tag != TagAnonymous)
        {
            //Get animation
            animPowerUp = this.gameObject.GetComponent<Animator>();

            //Get effect
            particle = GetComponentInChildren<ParticleSystem>();
        }

        //get choose effect
        if (this.tag == TagCharacter)
        {
            chooseEffect = GetComponentInChildren<LineRenderer>();
            chooseEffect.gameObject.SetActive(false);

            FXGenerate = GetComponentsInChildren<ParticleSystem>()[1];
            turnOnGenerateFX();
        }

        appearBeginBullet();

        if (this.tag == TagEnemy) CreateAIEnemy();
    }

    private void Update()
    {
        if (!CustomTime.IsPaused)
        {
            if (this.tag != TagAnonymous)
            {
                if (this.tag == TagEnemy)
                {
                    if (targetAI != null)
                    {
                        //Xác định target có phải character không
                        if (targetAI.tag == tagCharacter)
                            if (CanWin(targetAI))
                            {
                                linkTo(targetAI);
                            }
                            else if (!CanWin(targetAI) && IsLink())
                            {
                                CancelLink();
                            }
                    }
                    if(linkObject != null)
                    {
                        if (linkObject.tag == tagCharacter)
                        {
                            if (CanWin(targetAI))
                            {
                                linkTo(targetAI);
                            }
                            else if (!CanWin(targetAI) && IsLink())
                            {
                                CancelLink();
                            }
                        }
                    }
                }

                loadBullet(); //nạp đạn
                loadShield(); //nạp shield

                if (boolLinked && boolAttacked)
                {
                    attackTo(linkObject);
                }
            }
        }
    }
}