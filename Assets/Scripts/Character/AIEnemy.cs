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

    private Boolean CanWin(ObjectAbstract target)
    {
        if (target.BulletAmount + target.ShieldAmount + target.PowerBulletAmount
            - enemy.BulletAmount - enemy.PowerBulletAmount * 3 <= 0) return true;
        else return false;
    }
    
    //public void HandleSituationAI()
    //{
    //    if (enemy.TargetAI != null)
    //    {
    //        //Xác định target có phải character không
    //        if (enemy.TargetAI.tag == "Object")
    //            //if (linkObject != targetAI)
    //            //{
    //            //Xác định có thể thắng không
    //            if (CanWin(enemy.TargetAI))
    //            {
    //                enemy.linkTo(enemy.TargetAI);
    //            }
    //            else if (IsLink())
    //            {
    //                CancelLink();
    //            }
    //        //}   
    //    }
    //    if (linkObject != null)
    //    {
    //        if (linkObject.tag == "Object")
    //        {
    //            if (CanWin(targetAI))
    //            {
    //                linkTo(targetAI);
    //            }
    //            else if (IsLink())
    //            {
    //                CancelLink();
    //            }
    //        }
    //    }
    //}

    //Thuật toán A* để tìm đường tấn công tốt nhất cho Bot
    public void AFind(ObjectAbstract start, ObjectAbstract target)
    {
        if (target == null) throw new NullReferenceException();

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
                if (i.f < current.f) current = i;
            }

            //Remove current ra khỏi open
            open.Remove(current);
            //Add current vào close
            close.Add(current);

            if (current == target)
            {
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

                    if (!open.Contains(i) || tmpCurrentG < i.g) // lấy g nhỏ nhất 
                    {
                        i.cameFrom = current;
                        i.g = tmpCurrentG;
                        i.h = i.Distance(target);
                        i.f = i.g + i.h;

                        if (!open.Contains(i))
                        {
                            open.Add(i);
                        }
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
