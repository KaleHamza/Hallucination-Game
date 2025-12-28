using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // Listeler için gerekli
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("SonucCanva")]
    public GameObject[] SonucCanvalar;
    [Header("IpucuCanva")]
    public GameObject IpucuPanel;
    public Text timeText;

    [Header("Zaman Ayarlarý")]
    public float elapsedTime = 0f;
    public Text textIpucu;
    private bool gameEnded = false;

    [Header("Akýl Saðlýðý")]
    public float sanity = 100f;
    public Text sanityText;
    private DepthOfField blurEffect;
    public Image flashImage;
    public Image blackFadeImage;
    public AudioSource audioSource;
    public AudioClip heartbeatClip;
    public AudioClip BulunduSes;
    public AudioClip KapiGicirdi;
    public RectTransform customCursorUI;

    [Header("Oyun Mantýðý")]
    public int remainingAnomalies = 2;

    [Header("Oda Sistem")]
    public GameObject[] Odalar;
    public GameObject[] Kapilar;
    public int BulunduguOdaIndeksi = 0;

    // --- YENÝ EKLENEN KISIM BAÞLANGIÇ ---
    [Header("Görsel Ýpucu Sistemi")]
    public Button ipucuButonu; // Unity Editörden 'Ýpucu Kullan' butonunu buraya sürükle
    private bool buBolumdeIpucuKullanildi = false;
    private List<AnomaliNesnesi> aktifOdaAnomalileri = new List<AnomaliNesnesi>();
    // --- YENÝ EKLENEN KISIM BÝTÝÞ ---

    void Start()
    {
        TumOdalarKapali();
        if (Odalar.Length > 0) Odalar[0].SetActive(true);
        foreach (var kapi in Kapilar) if (kapi != null) kapi.SetActive(true);

        SetAlpha(flashImage, 0);
        SetAlpha(blackFadeImage, 0);

        // Buton dinleyicisi
        if (ipucuButonu != null)
        {
            ipucuButonu.onClick.AddListener(GorselIpucuKullan);
        }

        StartCoroutine(AnomaliyeDonusturSayaci(0));
    }

    void Update()
    {
        if (gameEnded) return;

        HandleTime();
        HandleSanityEffects();

        // Eski metin tabanlý ipucu sistemi tuþlarý (Ýstersen kaldýrabilirsin)
        if (Input.GetKeyDown(KeyCode.O)) Ipucu("ipucu");
        if (Input.GetKeyDown(KeyCode.C)) Ipucu("kapat");
        if (Input.GetKeyDown(KeyCode.X)) GorselIpucuKullan();
    }

    // --- YENÝ ÝPUCU SÝSTEMÝ FONKSÝYONLARI ---

    // Anomali objeleri aktif olunca bu listeye kendini ekler
    public void AnomaliKaydet(AnomaliNesnesi anomali)
    {
        if (!aktifOdaAnomalileri.Contains(anomali))
        {
            aktifOdaAnomalileri.Add(anomali);
        }
    }

    // Anomali bulununca veya oda deðiþince listeden çýkar
    public void AnomaliCikar(AnomaliNesnesi anomali)
    {
        if (aktifOdaAnomalileri.Contains(anomali))
        {
            aktifOdaAnomalileri.Remove(anomali);
        }
    }

    // Butona basýldýðýnda çalýþacak fonksiyon
    public void GorselIpucuKullan()
    {
        // 1. Hak kontrolü
        if (buBolumdeIpucuKullanildi)
        {
            Debug.Log("Bu bölüm için ipucu hakkýn bitti!");
            // Ekrana "Hakkýn kalmadý" yazýsý eklenebilir.
            return;
        }

        // 2. Liste kontrolü
        if (aktifOdaAnomalileri.Count == 0)
        {
            Debug.Log("Þu an bulunacak aktif bir anomali yok.");
            return;
        }

        // 3. Rastgele seçim ve çalýþtýrma
        int rastgeleIndex = Random.Range(0, aktifOdaAnomalileri.Count);
        AnomaliNesnesi secilenAnomali = aktifOdaAnomalileri[rastgeleIndex];

        if (secilenAnomali != null)
        {
            secilenAnomali.IpucuGoster();
            buBolumdeIpucuKullanildi = true; // Hakký tüket

            // Butonu pasif hale getirebiliriz (Görsel olarak)
            if (ipucuButonu != null) ipucuButonu.interactable = false;

            Debug.Log("Ýpucu kullanýldý: Bir nesne parlýyor.");
        }
    }

    // --- MEVCUT SÝSTEM ENTEGRASYONU ---

    public void AnomaliBulundu()
    {
        remainingAnomalies--;
        audioSource.PlayOneShot(BulunduSes);
        if (remainingAnomalies <= 0)
        {
            audioSource.PlayOneShot(KapiGicirdi);
            KapiyiAcVeSonrakiOdayiHazirla();
        }
    }

    void KapiyiAcVeSonrakiOdayiHazirla()
    {
        int sonrakiOdaIndex = BulunduguOdaIndeksi + 1;
        if (sonrakiOdaIndex < Odalar.Length)
        {
            Odalar[sonrakiOdaIndex].SetActive(true);
            int kapiIndex = BulunduguOdaIndeksi / 2;
            if (kapiIndex < Kapilar.Length && Kapilar[kapiIndex] != null)
            {
                Kapilar[kapiIndex].SetActive(false);
            }
        }
        else
        {
            SonucCanvalar[0].SetActive(true);
           
        }
    }

    public void YeniOdayaGirisYapildi(int girilenOdaNo)
    {
        Debug.Log("Yeni odaya girildi: " + girilenOdaNo);

        for (int i = 0; i < girilenOdaNo; i++)
        {
            if (Odalar[i] != null) Odalar[i].SetActive(false);
        }

        BulunduguOdaIndeksi = girilenOdaNo;
        remainingAnomalies = 3;

        // --- ÝPUCU HAKKINI YENÝLE ---
        buBolumdeIpucuKullanildi = false;
        aktifOdaAnomalileri.Clear(); // Liste temizlenir, yeni odadaki objeler OnEnable ile tekrar dolar
        if (ipucuButonu != null) ipucuButonu.interactable = true; // Butonu tekrar aktif et

        StartCoroutine(AnomaliyeDonusturSayaci(girilenOdaNo));
    }

    IEnumerator AnomaliyeDonusturSayaci(int normalOdaIndex)
    {
        yield return new WaitForSeconds(5f);
        int anomaliOdaIndex = normalOdaIndex + 1;
        if (anomaliOdaIndex < Odalar.Length)
        {
            StartCoroutine(FlashEffect());
            Odalar[normalOdaIndex].SetActive(false);
            Odalar[anomaliOdaIndex].SetActive(true);
            BulunduguOdaIndeksi = anomaliOdaIndex;
        }
    }

    // --- ESKÝ YARDIMCI FONKSÝYONLAR (Aynen Kalýyor) ---
    void TumOdalarKapali()
    {
        foreach (var oda in Odalar) if (oda != null) oda.SetActive(false);
    }

    // ... FlashEffect, HandleTime, WrongClickPenalty, SetAlpha, UpdateSanityUI, HandleSanityEffects ... 
    // Bu fonksiyonlar senin orijinal kodundaki gibi kalmalý, yer kaplamamasý için tekrar yazmadým.

    IEnumerator FlashEffect()
    {
        SetAlpha(blackFadeImage, 1f);
        float duration = 40.0f;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            SetAlpha(blackFadeImage, Mathf.Lerp(1f, 0f, t / duration));
            yield return null;
        }
        SetAlpha(blackFadeImage, 0f);
    }

    public void WrongClickPenalty()

    {

        sanity -= 15f;

        sanity = Mathf.Clamp(sanity, 0, 100);

        UpdateSanityUI();

        StartCoroutine(RedFlashEffect());

        if (audioSource != null && heartbeatClip != null) audioSource.PlayOneShot(heartbeatClip);

        if (sanity <= 0)

        {

            SonucCanvalar[1].SetActive(true);

            GameOver("Delirdin...");

        }

    }
    void UpdateSanityUI() => sanityText.text = "Sanity: %" + Mathf.RoundToInt(sanity);

    IEnumerator RedFlashEffect()

    {

        float duration = 0.3f;

        float elapsed = 0f;

        while (elapsed < duration)

        {

            elapsed += Time.deltaTime;

            SetAlpha(flashImage, Mathf.Lerp(0.5f, 0, elapsed / duration));

            yield return null;

        }

    }

    void HandleTime()
    {
        elapsedTime += Time.deltaTime;
        int gameMinutes = Mathf.FloorToInt(elapsedTime);
        int hours = gameMinutes / 60;
        int minutes = gameMinutes % 60;
        timeText.text = string.Format("{0:00}:{1:00} AM", hours, minutes);
        if (elapsedTime >= 360f && remainingAnomalies > 0) GameOver("Zaman doldu!");
    }

    void HandleSanityEffects()
    {
        if (sanity < 20f)
        {
            // blurEffect kodu...
            if (customCursorUI != null) customCursorUI.position = (Vector2)Input.mousePosition + (Random.insideUnitCircle * 10f);
        }
        // else blurEffect kodu...
    }

    void SetAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    void GameOver(string message) { gameEnded = true; Debug.Log(message); }

    // Eski metin ipucu fonksiyonunu istersen tutabilirsin
    public void Ipucu(string amac)
    {
        if (amac == "ipucu") IpucuPanel.SetActive(true); // Basitçe paneli açar
        else if (amac == "kapat") IpucuPanel.SetActive(false);
    }
}