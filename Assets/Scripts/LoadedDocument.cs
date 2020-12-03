using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedDocument
{
    private static LoadedDocument _instance = null;
    public static LoadedDocument Instance
    {
        get
        {
            if(_instance == null)
                _instance = new LoadedDocument();

            return _instance;
        }

        set => _instance = value;
    }
    public string ArticleName { get; set; }
    public string DOI { get; set; }
    public List<Author> Authors { get; private set; } = new List<Author>();

    public Figure LoadedFigure = new Figure();

    public bool ReadyToLoad { get; set; }

    public void AddAuthor(string authorName, string affiliation)
    {
        Authors.Add(new Author(authorName, affiliation));
    }

    public class Author
    {
        public string AuthorName { get; set; }
        public string Affiliation { get; set; }

        public Author(string aName, string affiliation)
        {
            this.AuthorName = aName;
            this.Affiliation = affiliation;
        }
    }

    public class Figure
    {
        public string Id { get; set; }
        public string ObjFilePath { get; set; }
        public string MtlFilePath { get; set; }
    }
}
