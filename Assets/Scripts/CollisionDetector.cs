using System;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public String ignore;
    public List<GameObject> collidedWith = new();
    public List<Collider> collidedWithC = new();
    public List<String> collidedWithStr = new();

    private void OnCollisionEnter(Collision other)
    {
        if (ignore.Length > 0 && other.gameObject.name.Contains(ignore)) return;
        collidedWith.Add(other.gameObject);
        if (other.collider != null) collidedWithC.Add(other.collider);
        if (other.collider != null) collidedWithStr.Add(other.collider.name);
    }

    private void OnCollisionExit(Collision other)
    {
        collidedWith.Remove(other.gameObject);
        if (other.collider != null) collidedWithC.Remove(other.collider);
        if (other.collider != null) collidedWithStr.Remove(other.collider.name);
    }

    public void Clear()
    {
        collidedWith.Clear();
        collidedWithC.Clear();
        collidedWithStr.Clear();
    }
}