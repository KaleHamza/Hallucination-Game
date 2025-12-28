using UnityEngine;
using System.Collections;

public class AnomaliNesnesi : MonoBehaviour
{
    [Header("Efekt Ayarlarý")]
    public GameObject isikEfekti; // Objenin içine koyacaðýn Point Light veya Parlayan bir Sprite
    public float yanipSonmeSuresi = 2.0f;

    private GameManager gameManager;
    private bool bulundu = false;

    void OnEnable()
    {
        // Sahne açýldýðýnda veya obje aktif olduðunda GameManager'a kendini kaydettir
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AnomaliKaydet(this);
        }

        // Baþlangýçta ýþýk kapalý olsun
        if (isikEfekti != null) isikEfekti.SetActive(false);
    }

    void OnDisable()
    {
        // Obje kapandýðýnda (veya bulunduðunda) listeden çýkar
        if (gameManager != null)
        {
            gameManager.AnomaliCikar(this);
        }
    }

    // Bu fonksiyonu PlayerInteraction scripti çaðýrmalý (Eþyayý bulunca)
    public void NesneBulundu()
    {
        bulundu = true;
        if (gameManager != null) gameManager.AnomaliCikar(this);
        // Buraya ekstra yok olma veya ses efektleri eklenebilir
    }

    // GameManager burayý tetikleyecek
    public void IpucuGoster()
    {
        if (!bulundu)
        {
            StartCoroutine(ParlatRoutine());
        }
    }

    IEnumerator ParlatRoutine()
    {
        if (isikEfekti == null) yield break;

        isikEfekti.SetActive(true); // Iþýðý aç

        float timer = 0f;
        // Basit bir ýþýk titremesi (Opsiyonel: Light bileþeninin intensity'si ile oynanabilir)
        // Burada sadece 2 saniye açýk tutup kapatýyoruz, dilersen yanýp sönme ekleyebiliriz.

        // Yavaþça yanýp sönme efekti (Pulse)
        Light lightComp = isikEfekti.GetComponent<Light>();
        float defaultIntensity = lightComp != null ? lightComp.intensity : 1f;

        while (timer < yanipSonmeSuresi)
        {
            timer += Time.deltaTime;
            // Sinüs dalgasý ile ýþýk þiddetini artýrýp azaltma
            if (lightComp != null)
            {
                lightComp.intensity = Mathf.PingPong(Time.time * 5, defaultIntensity * 2);
            }
            yield return null;
        }

        if (lightComp != null) lightComp.intensity = defaultIntensity;
        isikEfekti.SetActive(false); // Iþýðý kapat
    }
}