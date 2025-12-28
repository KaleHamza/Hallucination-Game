using UnityEngine;

public class HorrorCamera : MonoBehaviour
{
    [Header("Temel Ayarlar")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    [Header("Gerginlik Ayarlarý (Korku Hissi)")]
    [Tooltip("Kameranýn fareye tepki süresi. Yüksek deðer daha aðýr/sarhoþ bir his verir.")]
    public float smoothTime = 10f; // 15-20 arasý daha aðýr hissettirir

    [Tooltip("Yürürken oluþan kafa sallantýsý.")]
    public bool enableHeadBob = true;
    public float bobSpeed = 14f;
    public float bobAmount = 0.05f;

    float xRotation = 0f;
    float mouseX, mouseY;

    // Smooth hareket için geçici deðiþkenler
    float currentXRotation;
    float currentYRotation;
    float xRotationV;
    float yRotationV;

    // Headbob için baþlangýç pozisyonu
    float defaultYPos = 0;
    float timer = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultYPos = transform.localPosition.y; // Kameranýn orijinal yüksekliðini kaydet
    }

    void Update()
    {
        HandleMouseLook();
        if (enableHeadBob) HandleHeadBob();

    }

    void HandleMouseLook()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Hedef rotasyonu hesapla
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // -- KORKU ETKÝSÝ: SMOOTHING --
        // Kamerayý anýnda döndürmek yerine, hedefe doðru "yumuþak" bir geçiþ yapýyoruz.
        // Bu, karakterin kafasýnýn aðýr olduðu veya korkudan yavaþ tepki verdiði hissini yaratýr.

        currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotationV, 1f / smoothTime);
        currentYRotation = Mathf.SmoothDamp(currentYRotation, mouseX, ref yRotationV, 1f / smoothTime);

        // Kamerayý yukarý aþaðý döndür (X ekseni)
        transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);

        // Karakter gövdesini saða sola döndür (Y ekseni) - Fare hareketine göre
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void HandleHeadBob()
    {
        // Karakter hareket ediyor mu? (WASD tuþlarýna basýlýyor mu?)
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
        {
            // Sinüs dalgasý ile ritmik hareket oluþtur
            timer += Time.deltaTime * bobSpeed;
            float newY = defaultYPos + Mathf.Sin(timer) * bobAmount;

            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
        else
        {
            // Durduðunda kamerayý yavaþça eski yüksekliðine getir (Nefes alma etkisi gibi hafif kalabilir)
            timer = 0;
            float newY = Mathf.Lerp(transform.localPosition.y, defaultYPos, Time.deltaTime * bobSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
    }
}