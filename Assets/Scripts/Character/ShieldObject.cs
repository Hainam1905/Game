using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldObject : ObjectAbstract
{
    private ObjectAbstract newObject;

    protected override void beDefeated(string tagName)
    {
        base.beDefeated(tagName);
        
        Destroy(this.gameObject);

        if (this.gameObject.tag == TagEnemy)
        {
            if (tagName == TagBulletCharacter)
            {
                //Xuất hiện vật mới
                newObject = Instantiate(shieldObject);
            }
            else Debug.Log("Lỗi!!!");

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
        else if (this.gameObject.tag == TagCharacter)
        {
            if (tagName == TagBulletEnemy)
            {
                //Xuất hiện vật mới
                newObject = Instantiate(enemyShieldObject);
            }
            else Debug.Log("Lỗi!!!");

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
        else if (this.gameObject.tag == TagAnonymous)
        {
            if (tagName == TagBulletEnemy)
            {
                //Xuất hiện vật mới
                newObject = Instantiate(enemyShieldObject);

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
            else if (tagName == TagBulletCharacter)
            {
                //Xuất hiện vật mới
                newObject = Instantiate(shieldObject);
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

    // Start is called before the first frame update
    protected override void Awake()
    {
        //ShieldAmount = constShieldBegin;
        //BulletAmount = constBulletBegin;
        MaxShield = constMaxShield * 2;
        TimeLoadBullet = constTimeLoadBullet;

        benefitCharacter = constBenefitCharacter + 1;
        benefitAnonymous = constBenefitAnonymous + 1;
    }
}
