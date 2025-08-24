using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAI : MonoBehaviour
{
    public float timer;//计时器，用于统计动物的闲置时间
    public bool isIdle = true;//是否处于闲置状态
    public float timeKey = 0;//代表当前的时间进度
    private Animator ani;//动画控制器
    private Rigidbody2D rb;//刚体组件
    void Start()
    {
        timer = 3f;
        ani = GetComponent<Animator>();//获取动画组件
        rb = GetComponent<Rigidbody2D>();//获取刚体组件
    }


    void Update()
    {
        timeKey += Time.deltaTime;
        if (timeKey >= timer)
        {
            timeKey = 0;
            timer = Random.Range(2f, 5f);//重置timer的时间 
            if (isIdle)
            {
                isIdle = false;
            }
            else
            {
                isIdle = true;
            }
            //获取朝向随机值
            int ran = Random.Range(0, 2);
            if (ran == 0)
            {
                transform.localScale = new Vector3(1, 1, 1);//向左
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);//向右
            }
        }
        if (isIdle)
        {
            ani.SetBool("isWalk", false);
        }
        else
        {   //正在移动时
            ani.SetBool("isWalk", true);
            if (transform.localScale.x > 0)//朝向左边时
            {
                 rb.velocity = new Vector2(-1, 0)*Time.deltaTime*40;
            }
            else//朝向右边时
            { 
                 rb.velocity = new Vector2(1, 0)*Time.deltaTime*40;
            }
            }
            
        }
}

