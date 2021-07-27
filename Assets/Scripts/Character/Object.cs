using UnityEngine;

public class Object : ObjectAbstract
{
    private ObjectAbstract newObject;

    protected override void beDefeated(string tagName)
    {
        base.beDefeated(tagName);
        
        Destroy(this.gameObject);

        if(this.gameObject.tag == TagEnemy)
        {
            if (tagName == TagBulletCharacter)
            {
                //Xuất hiện vật mới
                newObject = Instantiate(objectAbstract); 
                //Chỉnh vị trí
                newObject.transform.position = this.transform.position;

                //Liên kết với các vật cũ
                newObject.Link = this.Link;

                foreach (ObjectAbstract search in this.Link)
                {
                    //Vật cũ liên kết lại với vật này
                    search.Link.Remove(this);
                    search.Link.Add(newObject);
                }
            }
            else Debug.Log("Lỗi!!!");

            
        } else if (this.gameObject.tag == TagCharacter)
        {
            if (tagName == TagBulletEnemy)
            {
                //Xuất hiện vật mới
                newObject = Instantiate(enemyObject);

                //Chỉnh vị trí
                newObject.transform.position = this.transform.position;

                //Liên kết với các vật cũ
                newObject.Link = this.Link;

                foreach (ObjectAbstract search in this.Link)
                {
                    //Vật cũ liên kết lại với vật này
                    search.Link.Remove(this);
                    search.Link.Add(newObject);
                }
            }
            else Debug.Log("Lỗi!!!");
        }
        else if (this.gameObject.tag == TagAnonymous)
        {
            if (tagName == TagBulletEnemy)
            {
                //Xuất hiện vật mới
                newObject = Instantiate(enemyObject);

                //Chỉnh vị trí
                newObject.transform.position = this.transform.position;

                //Liên kết với các vật cũ
                newObject.Link = this.Link;

                foreach (ObjectAbstract search in this.Link)
                {
                    //Vật cũ liên kết lại với vật này
                    search.Link.Remove(this);
                    search.Link.Add(newObject);
                }
            } else if (tagName == TagBulletCharacter)
            {
                //Xuất hiện vật mới
                newObject = Instantiate(objectAbstract);
                //Chỉnh vị trí
                newObject.transform.position = this.transform.position;

                //Liên kết với các vật cũ
                newObject.Link = this.Link;

                foreach (ObjectAbstract search in this.Link)
                {
                    //Vật cũ liên kết lại với vật này
                    search.Link.Remove(this);
                    search.Link.Add(newObject);
                }
            }
            else Debug.Log("Lỗi!!!");
        }
    }

    //protected override void Awake()
    //{
    //    //ShieldAmount = constShieldBegin;
    //    BulletAmount = constBulletBegin;
    //    MaxShield = constMaxShield;
    //    TimeLoadBullet = constTimeLoadBullet;

    //    //benefitAnonymous = 104f;
    //}
}
