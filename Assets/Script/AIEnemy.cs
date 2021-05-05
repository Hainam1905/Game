using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AIEnemy
{
    private ObjectAbstract enemy;
    public Stack<ObjectAbstract> path;

    public AIEnemy(ObjectAbstract enemy)
    {
        this.enemy = enemy;
    }

    //Link to Object which have min shiledAmount
    public void CreateAIBot()
    {
        //int minShield = enemy.MaxShield;

        //foreach (ObjectAbstract gameObject in enemy.Link)
        //{
        //    if (gameObject.GetComponent<ObjectAbstract>().ShieldAmount < minShield)
        //        minShield = gameObject.GetComponent<ObjectAbstract>().ShieldAmount;
        //}

        //foreach (ObjectAbstract gameObject in enemy.Link)
        //{
        //    if (gameObject.GetComponent<ObjectAbstract>().ShieldAmount == minShield)
        //        enemy.linkTo(gameObject);
        //}

        float minf = 100;
        foreach (ObjectAbstract objectAbstract in enemy.Link)
        {
            if (objectAbstract.hx + enemy.Waste(objectAbstract) < minf)
                minf = objectAbstract.hx + enemy.Waste(objectAbstract);
        }

        foreach (ObjectAbstract objectAbstract in enemy.Link)
        {
            if (objectAbstract.hx + enemy.Waste(objectAbstract) == minf)
                if (enemy.linkObject != objectAbstract)
                {
                    //Debug.Log(objectAbstract.name + ": " + objectAbstract.hx);
                    if (minf <= 0) enemy.linkTo(objectAbstract);

                }
        }
    }


    //Thuật toán A* để tìm đường tấn công tốt nhất cho Bot
    public void AFind(ObjectAbstract start, ObjectAbstract target)
    {
        List<ObjectAbstract> open = new List<ObjectAbstract>();
        List<ObjectAbstract> close = new List<ObjectAbstract>();

        //Tính h, g, f của start
        start.g = 0;
        start.h = start.Distance(target);
        start.f = start.g + start.h;

        open.Add(start);
        while (open.Count != 0)
        {
            //tìm min fx trong open
            ObjectAbstract current = open.ElementAt(0);
            foreach (ObjectAbstract i in open)
            {
                if (i.fx < current.fx) current = i;
            }
            //if(current.cameFrom == null) Debug.Log(current.f + " " + current.name + " " + current.g);
            //else Debug.Log(current.f + " " + current.name + " comfrom " + current.cameFrom.name + " " + current.g);
            //Remove current ra khỏi open
            open.Remove(current);
            //Add current vào close
            close.Add(current);

            if (current == target)
            {
                //String temp = null;
                //foreach(ObjectAbstract objectAbstract in open)
                //{
                //    temp = temp + " | " + objectAbstract.name; 
                //}

                open.Clear();
                close.Clear();
                //Trả về đường đi
                path = ReconstructPath(start, target);
            }
            else
            {
                foreach (ObjectAbstract i in current.Link)
                {
                    if (close.Contains(i)) continue;
                   
                    //tính lại g
                    float tmpCurrentG = current.g + current.Waste(i);
                    //Debug.Log(current.g + " " + current.Waste(i) + " " + i.name);
                    if (!open.Contains(i) || tmpCurrentG < i.g) // lấy g nhỏ nhất 
                    {
                        //Debug.Log("name: " + i.name);
                        i.cameFrom = current;
                        i.g = tmpCurrentG;
                        i.h = i.Distance(target);
                        i.f = i.g + i.h;
                        if (!open.Contains(i)) open.Add(i);
                    }
                }
            }
        }
        return;
    }

    public Stack<ObjectAbstract> ReconstructPath(ObjectAbstract s, ObjectAbstract t)
    {
        Stack<ObjectAbstract> path = new Stack<ObjectAbstract>();

        ObjectAbstract temp = t;
        while(temp != null)
        {
            path.Push(temp);
            ObjectAbstract cF = temp;
            temp = temp.cameFrom;
            cF.cameFrom = null;
        }

        return path;
    }
}
