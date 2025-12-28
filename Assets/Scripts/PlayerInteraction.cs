using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Ayarlar")]
    public float interactionDistance = 10f;
    public LayerMask interactableLayers;

    private Camera mainCam;
    private GameManager gameManager;
    public Text PuanText;

    void Start()
    {
        mainCam = Camera.main;
        gameManager = Object.FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AttemptInteract();
        }
    }

    void AttemptInteract()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Anomaly") || hit.collider.CompareTag("AnomalyGoz") || hit.collider.CompareTag("AnomalyKonum"))
            {
                // Puanlama iþlemleri...
                if (PuanText != null)
                {
                    int p = 0;
                    int.TryParse(PuanText.text, out p);
                    PuanText.text = (p + 75).ToString();
                }
                if (hit.collider.TryGetComponent(out AnomaliNesnesi anomaliScript))
                {
                    anomaliScript.NesneBulundu(); // Iþýðý kapat ve listeden düþ
                }
                Debug.Log("Anomali bulundu: " + hit.collider.name);
                FoundAnomaly(hit.collider.gameObject);
            }
            else
            {
                Debug.Log("Yanlýþ obje!");
                gameManager.WrongClickPenalty();
            }
        }
        else
        {
            gameManager.WrongClickPenalty();
        }
    }

    void FoundAnomaly(GameObject anomalyObj)
    {
        
        Destroy(anomalyObj);

        // --- DEÐÝÞEN KISIM BURASI ---
        // GameManager'daki fonksiyonu çaðýrýyoruz, tüm lojiði o hallediyor.
        if (gameManager != null)
        {
            gameManager.AnomaliBulundu();
        }
    }
}// ... FlashEffect, HandleTime, WrongClickPenalty, SetAlpha, UpdateSanityUI, HandleSanityEffects ... 