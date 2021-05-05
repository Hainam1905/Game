using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ObjectAbstract : MonoBehaviour
{
    //AI enemy properties
    private AIEnemy aIEnemy;
    public ObjectAbstract cameFrom = null;
    public float g = 0f;
    public float h, f = 0;
    public int benefitCharacter = 8;
    public float benefitAnonymous;// = 104f;

    //Object
    protected ObjectAbstract objectAbstract, enemyObject, shieldObject,
        enemyShieldObject, fastCharacter, enemyFastCharacter;
    //Bullet
    protected GameObject bulletPrefab, powerBulletPrefab, enemyBulletPrefab, enemyPowerBulletPrefab;
    //LineRenderer
    private List<LineRenderer> LineRenderers = new List<LineRenderer>();
    //LineMaterial
    private Material lineMaterial;

    //Path Object
    private const string pathCharacter = "Prefabs/Character/Character";
    private const string pathFastLoadCharacter = "Prefabs/Character/FastLoadCharacter";
    private const string pathShieldCharacter = "Prefabs/Character/ShieldCharacter";
    private const string pathEnemy = "Prefabs/Enemy/Enemy";
    private const string pathFastLoadEnemy = "Prefabs/Enemy/FastLoadEnemy";
    private const string pathShieldEnemy = "Prefabs/Enemy/ShieldEnemy";
    //Path Bullet
    private const string pathBullet = "Prefabs/Bullet/Bullet";
    private const string pathEnemyBullet = "Prefabs/Bullet/EnemyBullet";
    private const string pathPowerBullet = "Prefabs/Bullet/PowerBullet";
    private const string pathEnemyPowerBullet = "Prefabs/Bullet/EnemyPowerBullet";
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

    //Biến kiểm tra double kick
    protected const float doubleClickTime = .2f; //Nếu click liên tiếp 2 lần dưới thời gian này là double click
    protected float lastClickTime; //Thời điểm của lần click trước

    //Thuộc tích của vật
    [SerializeField] private int shieldAmount, bulletAmount, powerBulletAmount;
    private int maxShield;
    private float timeLoadBullet; //Thời gian nạp đạn
    [SerializeField] private List<ObjectAbstract> link = new List<ObjectAbstract>();
    
    //Raycas để tìm object liên kết (linkTo)
    public ObjectAbstract linkObject = null;
    private Vector3 oldPositionLink;
    public float rayLength = 0f;
    protected bool boolLinked = false; //Kiểm tra xem đã liên kết được chưa

    //Biến để tấn công (attackTo)
    protected float countAttack = 0; //đếm thời gian cho viên đạt kế tiếp
    protected bool boolAttacked = false; //Có đồng ý tấn công không

    //Biến nạp đạp (load)
    protected float countLoadBullet = 0; //đếm thời gian cho lần nạp đạn kế tiếp
    protected float countLoadShield = 0;

    //Animation
    private Animator animPowerUp;

    //Tỏa sáng khi Power
    private ParticleSystem particle;


    //Setter, getter
    //Độ hao phí khi tấn công vào object này
    public float gx
    {
        get { return (BulletAmount / 3 + ShieldAmount); }
    }
    //Hao phí dự đoán khi chiếm được object
    public float hx
    {
        get
        {
            float kq = 0;
            if (this.tag != "Enemy")
            {
                //if (this.tag == "Anonymous") kq = kq - benefitAnonymous;
                //else kq = kq - benefitCharacter;

                foreach (ObjectAbstract objectAbstract in Link)
                {
                    if (objectAbstract.tag == "Anonymous") kq = kq - (benefitAnonymous / 4);
                    if (objectAbstract.tag == "Character") kq = kq - (benefitCharacter / 4);
                }

            }
            else return 0;

            return kq;
        }
    }
    public float fx
    {
        get
        {
            return (gx + hx);
        }
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

    public float a;

    //Hàm tính chi phí của AI
    public int Waste(ObjectAbstract target)
    {
        a = benefitAnonymous;
        foreach (ObjectAbstract objectAbstract in Link)
        {
            if (objectAbstract == target)
            {
                int kq = 0;
                if (target.tag != "Enemy")
                {
                    kq = (target.BulletAmount + target.ShieldAmount);
                    if (target.tag == "Anonymous") kq = kq - (int)benefitAnonymous;
                    else kq = kq - benefitCharacter;
                }
                else kq = 0; 

                return kq;
            }
        }
        throw new ArgumentNullException();
    }
    //Hàm dự đoán chi phí của AI
    public float Distance(ObjectAbstract target)
    {
        if (hx != 0) return hx;
        else return Vector3.Distance(target.transform.position, this.transform.position);
    }

    //Abstract Funtion
    protected virtual void beDefeated(string tagName)
    {
        

        if(tagName == "BulletObject")
        {
            GameController.EarningACharacter();
            if (tag == "Enemy") GameController.DestroyAnEnemy();
        }
        else
        {
            GameController.EarningAnEnemy();
            if (tag == "Character") GameController.DestroyACharacter();
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

        //Load bullet
        bulletPrefab = Resources.Load<GameObject>(pathBullet);
        enemyBulletPrefab = Resources.Load<GameObject>(pathEnemyBullet);
        powerBulletPrefab = Resources.Load<GameObject>(pathPowerBullet);
        enemyPowerBulletPrefab = Resources.Load<GameObject>(pathEnemyPowerBullet);

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
        if(this.tag == "Object") powerBullet = Instantiate(powerBulletPrefab).GetComponent<PowerBullet>();
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

        if(this.tag == "Object") bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
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
            RaycastHit tempRaycastHit = new RaycastHit();

            //Liên kết đến vị trí đấy một lần nữa
            Physics.Raycast(transform.position, oldPositionLink - transform.position, out tempRaycastHit, rayLength);

            if (tempRaycastHit.rigidbody != null)
            {
                linkObject = tempRaycastHit.collider.GetComponent<ObjectAbstract>();
                attackTo(linkObject);
            }
        } else
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
            } else if (ShieldAmount == 0 && PowerBulletAmount > 0)
            {
                PowerBulletAmount = PowerBulletAmount - damage;

                if (PowerBulletAmount <= 0)
                {
                    smallerEffect();
                }
            } else if (ShieldAmount == 0 && BulletAmount > 0)
            {
                BulletAmount = BulletAmount - damage;
            } else if (ShieldAmount == 0 && BulletAmount == 0)
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
                                //this.countAttack = 0; //reset lại thời gian cho viên đạn kế tiếp
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
        //Lấy vị trí target
        Vector3 targetPos = target.transform.position;
        RaycastHit tempLinkOject;
        //Tính khoảng cách từ vật đến target d = căn(bình(x2 - x1) + bình(y2 - y1))
        rayLength = (float)Math.Sqrt(Math.Pow(targetPos.x - transform.position.x, 2)
                                    + Math.Pow(targetPos.y - transform.position.y, 2));
        Debug.DrawRay(transform.position, targetPos - transform.position, Color.red, 10f);
        //tạo raycast để tìm vật liên kết
        if (Physics.Raycast(transform.position, targetPos - transform.position, out tempLinkOject, rayLength))
        {
            Debug.Log("origin: " + this.name + " | target: " + tempLinkOject.collider.name);
            Debug.Log("origin: " + gameObject.tag + " | target: " + tempLinkOject.collider.tag, tempLinkOject.collider);
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
                                //this.countAttack = 0; //reset lại thời gian cho viên đạn kế tiếp
                                if (objectAbstract.linkObject == this.gameObject.GetComponent<ObjectAbstract>())
                                {
                                    /*Nếu object liên kết tới đã liên kết với object này rồi 
                                    thì object đó sẽ ngừng liên kết với object này*/
                                    objectAbstract.boolLinked = false;
                                    objectAbstract.boolAttacked = false;
                                    objectAbstract.linkObject = null;
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
            LineRenderers[i].SetPosition(0, this.transform.position + Vector3.forward);
            LineRenderers[i].SetPosition(1, Link[i].transform.position + Vector3.forward);
        }
    }

    //Create AI Enemy
    private void CreateAIEnemy()
    {
        aIEnemy = new AIEnemy(this);
        aIEnemy.AFind(this, GameObject.Find("Character").GetComponent<ObjectAbstract>());
        ObjectAbstract temp;
        Debug.Log(this.name);
        if (this.name == "ShieldEnemy(Clone)")
        {
            temp = GameObject.Find("ShieldAnonymous").GetComponent<ObjectAbstract>();
            //Debug.Log("1111: " + temp.name);
        }
        else
        {
            temp = aIEnemy.path.Pop(); temp = aIEnemy.path.Pop();
            Debug.Log(temp.name);
        }

        this.linkTo(temp);
    }

    private void OnMouseUp()
    {
        if (this.tag == "Object") linkTo();
    }

    private void OnMouseOver()
    {
        if (this.tag == "Object")
        {
            if (Input.GetMouseButtonDown(0))
            {
                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick <= doubleClickTime)
                {
                    //double click
                    boolLinked = false;
                    boolAttacked = false;
                    countAttack = 0;
                }

                lastClickTime = Time.time;
            }
        }
    }

    protected virtual void Awake()
    {
        //Khởi tạo chỉ số
        //ShieldAmount = constShieldBegin;
        BulletAmount = constBulletBegin;
        MaxShield = constMaxShield;
        TimeLoadBullet = constTimeLoadBullet;

        benefitAnonymous = 104f;
    }

    private void Start()
    {
        //Set Prefab
        loadPrefab();
        //Khởi tạo line
        initLineRenderer();
        //Hiện shield khởi đầu
        appearBeginShield(shieldAmount);

        if (this.tag != "Anonymous")
        {
            //Get animation
            animPowerUp = this.gameObject.GetComponent<Animator>();

            //Get effect
            particle = GetComponentInChildren<ParticleSystem>();
        }

        if (this.tag == "Enemy") CreateAIEnemy();
    }

    private void Update()
    {
        if (!CustomTime.IsPaused)
        {
            if (this.tag != "Anonymous")
            {
                //if (this.tag == "Enemy") aIEnemy.CreateAIBot();

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