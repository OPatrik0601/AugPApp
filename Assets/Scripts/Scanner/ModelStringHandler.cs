using System.Threading.Tasks;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;
using System.IO;
using System.Text;
using Dummiesman;
using UnityEngine.Networking;
using System;

public static class ModelStringHandler
{
    private const int stringFormatLength = 2; //QR code format length
    private const string errorText = "Error";

    /// <summary>
    /// Load the document by the qrValue.
    /// </summary>
    /// <param name="value">[format: URL to the document;modelid]</param>
    /// <returns>The loaded document</returns>
    public static LoadedDocument LoadDocument(string qrValue)
    {
        LoadedDocument.Instance = new LoadedDocument(); //start a new document

        string[] splittedValue = qrValue.Split(';');
        if (splittedValue.Length == stringFormatLength &&
            Uri.IsWellFormedUriString(splittedValue[0], UriKind.Absolute))
        {
            return DocumentLoader(splittedValue[0], splittedValue[1]); //if everything is ok, we can actually load the document
        }

        return LoadedDocument.Instance;
    }

    /// <summary>
    /// Load the XML document
    /// </summary>
    /// <param name="URLToXML">URL of the XML file</param>
    /// <param name="id">The ID of the model to load</param>
    private static LoadedDocument DocumentLoader(string URLToXML, string id)
    {
        XmlTextReader reader = null;
        try
        {
            reader = new XmlTextReader(URLToXML);

            //<MetaData>
            reader.ReadToDescendant("MetaData");
            LoadedDocument.Instance.ArticleName = reader.GetAttribute("ArticleName");
            LoadedDocument.Instance.DOI = reader.GetAttribute("DOI");

            //<Authors>
            if (reader.ReadToDescendant("Author"))
            {
                do
                {
                    LoadedDocument.Instance.AddAuthor(reader.GetAttribute("Name"), reader.GetAttribute("Affiliation")); //<Author />
                } while (reader.ReadToNextSibling("Author"));
            }
            //</Authors>
            //</MetaData>

            //<Figures>
            if (reader.ReadToFollowing("Figure"))
            {
                do
                {
                    string modelId = reader.GetAttribute("Id");
                    Debug.Log($"ID: {id}, modelId: {modelId}");
                    if (modelId == id) //we just need to load one figure
                    {
                        //<Figure>
                        LoadedDocument.Instance.LoadedFigure.ObjFilePath = reader.GetAttribute("ObjFile");
                        LoadedDocument.Instance.LoadedFigure.MtlFilePath = reader.GetAttribute("MtlFile");
                        LoadedDocument.Instance.LoadedFigure.Title = reader.GetAttribute("Title");
                        LoadedDocument.Instance.LoadedFigure.Id = id;

                        LoadedDocument.Instance.ReadyToLoad = true;
                        break;
                        //</Figure>
                    }
                } while (reader.ReadToNextSibling("Figure"));
            }
            //</Figures>
        }
        catch (XmlException exception)
        {
            Debug.Log(exception.Message);
            LoadedDocument.Instance = new LoadedDocument();
        } finally
        {
            if (reader != null)
                reader.Close();
        }

        return LoadedDocument.Instance;
    }

    /// <summary>
    /// Creates a GameObject with the data provided by the LoadedDocument
    /// </summary>
    /// <returns></returns>
    public static GameObject LoadDocumentObject()
    {
        if (!LoadedDocument.Instance.ReadyToLoad)
            return null;

        //obj web request
        var objFileWebRequest = UnityWebRequest.Get(LoadedDocument.Instance.LoadedFigure.ObjFilePath);
        objFileWebRequest.SendWebRequest();
        while (!objFileWebRequest.isDone)
            System.Threading.Thread.Sleep(1);

        //mtl web request
        var mtlFileWebRequest = UnityWebRequest.Get(LoadedDocument.Instance.LoadedFigure.MtlFilePath);
        mtlFileWebRequest.SendWebRequest();
        while (!mtlFileWebRequest.isDone)
            System.Threading.Thread.Sleep(1);

        //streams
        var objFileTextStream = new MemoryStream(Encoding.UTF8.GetBytes(objFileWebRequest.downloadHandler.text));
        var mtlFileTextStream = new MemoryStream(Encoding.UTF8.GetBytes(mtlFileWebRequest.downloadHandler.text));

        //GameObject
        GameObject loadedObj = new OBJLoader().Load(objFileTextStream, mtlFileTextStream);
        loadedObj.tag = "LoadedObject";

        return loadedObj;
    }
}
