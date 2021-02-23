using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public enum Tab {Music, Travel};
    public int MaxItemsPerRow;

    public GameObject InvItemPrefab;
    public GameObject musicScroll;
    public GameObject travelScroll;
    public GameObject musicContent;
    public GameObject travelContent;
    public Texture musicImage;

    public Tab activeTab;
    public List<MusicItem> musicItems = new List<MusicItem>();
    public List<TravelItem> travelItems = new List<TravelItem>();

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if (!gameObject.activeInHierarchy)
        {
            if (GameManager.instance.MusicOn)
            {
                GameManager.instance.ToggleMusic();
            }
            Clear();
            LayoutMusic();
            //LayoutTravel();
            ShowTab();
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    private void Clear()
    {
        // clear children of musicContent and travelContent
        foreach (Transform child in musicContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in travelContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void LayoutMusic()
    {
        GameObject row = new GameObject();
        row.name = "Row";
        row.AddComponent<RectTransform>();
        row.AddComponent<HorizontalLayoutGroup>();
        row.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset(20, 20, 20, 20);
        row.GetComponent<HorizontalLayoutGroup>().spacing = 100;
        row.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
        int itemsInRow = 0;

        foreach (MusicItem m in musicItems)
        {
            GameObject item = Instantiate(InvItemPrefab);
            item.GetComponent<Button>().onClick.AddListener(delegate { GameManager.instance.MusicSys.SetSong(m.song); });
            item.transform.Find("ItemName").GetComponent<Text>().text = m.song.name;
            item.transform.Find("ItemDesc").GetComponent<Text>().text =
                string.Format("Difficulty: {0}\nSpeed: {1}", m.song.beatDifficulty, (100 * m.song.speedDifficulty).ToString("F0"));
            item.transform.Find("Image").GetComponent<RawImage>().texture = musicImage;
            item.transform.Find("Image").GetComponent<RawImage>().color = m.color;
            item.transform.SetParent(row.transform);
            itemsInRow += 1;
            if (itemsInRow == MaxItemsPerRow)
            {
                row.transform.SetParent(musicContent.transform);
                row = new GameObject();
                row.AddComponent<RectTransform>();
                row.AddComponent<HorizontalLayoutGroup>();
                row.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset(20, 20, 20, 20);
                row.GetComponent<HorizontalLayoutGroup>().spacing = 100;
                row.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
                itemsInRow = 0;
            }
        }

        if (itemsInRow > 0)
        {
            row.transform.SetParent(musicContent.transform);
        }
    }
    

    public void ShowTab()
    {
        musicScroll.SetActive(false);
        travelScroll.SetActive(false);
        if (activeTab == Tab.Music)
        {
            musicScroll.SetActive(true);
        }
        else
        {
            travelScroll.SetActive(true);
        }
    }
}
