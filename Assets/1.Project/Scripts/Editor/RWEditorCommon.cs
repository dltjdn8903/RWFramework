using System.Collections.Generic;
using System.IO;
using UnityEngine;



public struct RWEditorFile
{
    public string FullPath
    {
        get
        {
            var fullPath = Path.Combine(Application.dataPath, m_assetPath) + m_ext;
            fullPath = fullPath.Replace('\\', '/');
            return fullPath;
        }
    }
    public string Name
    {
        get { return m_name; }
    }
    public bool IsMeta { get { return extentionIs(".meta"); } }
    public bool IsCSV { get { return extentionIs(".csv"); } }


    //  ex> assetPath : 1.Project/Resources/CSV/itemData
    //      extention : .csv
    public static RWEditorFile Def(string pathAfter_Assets, string name, string extention)
    {
        var f = new RWEditorFile();
        f.m_assetPath = Path.Combine(pathAfter_Assets, name);
        f.m_name = name;
        f.m_ext = extention;
        return f;
    }


    //----------------------------------------------------------------------
    #region private
    string m_assetPath;
    string m_ext;
    string m_name;


    bool extentionIs(string ext)
    {
        return string.Equals(m_ext.ToLower(), ext);
    }
    #endregion
}


public class RWEditorCommon
{
    public static IList<RWEditorFile> GetFullFilePathListIn(string pathAfter_Assets)
    {
        //  Application.dataPath : ex> C:/Work/Tanuki_svnroot/trunk/quantum_unity/Assets
        var dir = Path.Combine(Application.dataPath, pathAfter_Assets);
        var files = Directory.GetFiles(dir);

        var fileList = new List<RWEditorFile>();

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file);
            var name = Path.GetFileNameWithoutExtension(file);

            var editorFile = RWEditorFile.Def(pathAfter_Assets, name, ext);
            fileList.Add(editorFile);
        }

        return fileList;
    }


    public static void WriteFile(string pathAfter_Assets, string fileName, string text)
    {
        var dir = Path.Combine(Application.dataPath, pathAfter_Assets);

        if (false == Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var filePath = Path.Combine(dir, fileName);
        File.WriteAllText(filePath, text);
    }
}

