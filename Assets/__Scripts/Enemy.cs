using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector3[] directions = new Vector3[]
    {
        Vector3.right, Vector3.up, Vector3.left, Vector3.down
    };

    [Header("Set in Inspector: Enemy")]
    public float maxHealth = 1;
    public float knockbackSpeed = 10;
    public float knockbackDuration = 0.25f;
    public float invincibleDuration = 0.5f;
    public GameObject[] randomItemDrops;
    public GameObject guaranteedItemDrop = null;

    [Header("Set Dynamically: Enemy")]
    public float health;
    public bool invincible = false;
    public bool knockback = false;

    private float inviciblekDone = 0;
    private float knockbackDone = 0;
    private Vector3 knockbackVel;

    protected Animator anim;
    protected Rigidbody rigid;
    protected SpriteRenderer sRend;

    protected virtual void Awake()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        sRend = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        // ��������� ��������� ������������ � ������������� ��������� ������
        if (invincible && Time.time > inviciblekDone)
        {
            invincible = false;
        }
        sRend.color = invincible ? Color.red : Color.white;
        if (knockback)
        {
            rigid.velocity = knockbackVel;
            if (Time.time < knockbackDone) return;
        }

        anim.speed = 1;
        knockback = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (invincible)
        {
            return; // �����, ���� ���� ���� ��������
        }
        DamageEffect dEf = other.gameObject.GetComponent<DamageEffect>();
        if (dEf == null)
        {
            return;
        }

        health -= dEf.damage; // ������� �������� ������ �� ������ ��������
        if (health <= 0) 
        {
            Die();
        }
        invincible = true; // ������� ���� ����������
        inviciblekDone = Time.time + invincibleDuration;

        if (dEf.knockback) // ��������� ������������
        {
            // ���������� ����������� �������
            Vector3 delta = transform.position - other.transform.root.position;
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                // ������������ �� �����������
                delta.x = (delta.x > 0) ? 1 : -1;
                delta.y = 0;
            } else
            {
                // ������������ �� ���������
                delta.x = 0;
                delta.y = (delta.y > 0) ? 1 : -1;
            }

            // ��������� �������� ������������ � ���������� Rigidbody
            knockbackVel = delta * knockbackSpeed;
            rigid.velocity = knockbackVel;

            // ���������� ����� knockback � ����� ����������� ������������
            knockback = true;
            knockbackDone = Time.time + knockbackDuration;
            anim.speed = 0;
        }
    }

    void Die()
    {
        GameObject go;
        if (guaranteedItemDrop != null)
        {
            go = Instantiate<GameObject>(guaranteedItemDrop);
            go.transform.position = transform.position;
        }
        else if (randomItemDrops.Length > 0)
        {
            int n = Random.Range(0, randomItemDrops.Length);
            GameObject prefab = randomItemDrops[n];
            if (prefab != null)
            {
                go = Instantiate<GameObject>(prefab);
                go.transform.position = transform.position;
            }
        }
        Destroy(gameObject);
    }
}