using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PlayerMovements : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 5f;
    public float gravity = -19.62f; // Gerçekçi düþüþ için biraz yüksek tutulabilir

    Vector3 velocity;
    bool isGrounded;

    // Yerde olup olmadýðýmýzý kontrol etmek için ayak ucu noktasý
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Update()
    {
        // Yerde miyiz kontrolü
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Yere yapýþýk kalmasý için küçük bir deðer
        }

        // WASD Girdileri
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Hareket yönünü karakterin baktýðý yöne göre hesapla
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        // Yerçekimi uygulamasý
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
