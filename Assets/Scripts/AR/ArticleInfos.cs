using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArticleInfos : MonoBehaviour
{
    [SerializeField]
    private Text textToChange = null;

    [SerializeField]
    private GameObject objectToToggle = null;

    private bool loaded = false;

    private static ArticleInfos _instance = null;
    public static ArticleInfos Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<ArticleInfos>();

            return _instance;
        }
    }

    private void Start()
    {
        objectToToggle.SetActive(false);
    }

    public void ClickedToggle()
    {
        objectToToggle.SetActive(!objectToToggle.activeSelf);
        if (objectToToggle.activeSelf && !loaded)
            setText();
    }

    public void setText()
    {
        string text = $"Article title:\n{ LoadedDocument.Instance.ArticleName } (DOI: {LoadedDocument.Instance.DOI})\n\n" +
            $"Currently viewing:\n{LoadedDocument.Instance.LoadedFigure.Title}\n\n" +
            $"Author(s):\n";

        foreach(var author in LoadedDocument.Instance.Authors)
        {
            text += $"- {author.AuthorName}, Affiliation: {author.Affiliation}\n";
        }

        textToChange.text = text;
    }

    public bool IsShown
    {
        get => objectToToggle.activeSelf;
    }
}
