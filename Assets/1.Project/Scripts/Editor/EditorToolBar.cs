using MDF;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditorToolBar : Editor
{
    [MenuItem("Tools/DataExport/Csv To (Json, CS)")]
    public static void CsvToJsonCs()
    {
        var fileList = RWEditorCommon.GetFullFilePathListIn("0.Project/Resources/CSV");
        foreach( var file in fileList )
        {
            if( file.IsCSV )
            {
                //Log.Debug($"Convert start [{file.FullPath}]");

                var csvText = System.IO.File.ReadAllText(file.FullPath);

                var code = new CSDataCodeGenerator();

                //bool usingSchemaType = !string.Equals(file.Name, "TestData_Localization");
                bool usingSchemaType = true;

                if ( code.TryGenerateFrom( file.Name, csvText, usingSchemaType) )
                {
                    RWEditorCommon.WriteFile("0.Project/Resources/Json", file.Name + ".json", code.Json);

                    RWEditorCommon.WriteFile("0.Project/1.Script/Data/TableData", file.Name + ".cs", code.CS);
                }
            }
        }


        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        //MDF.Log.Debug("Work done : CsvToJsonCs");
    }

    [MenuItem("Tools/BuildMode/DebugMode")]
    public static void SetDebugMode()
    {
        var group = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> allDefines = group.Split(';').ToList();
        allDefines.RemoveAll(item => item == "LIVE_MODE");
        allDefines.RemoveAll(item => item == "DEBUG_MODE");
        allDefines.Add("DEBUG_MODE");

        PlayerSettings.SetScriptingDefineSymbolsForGroup(
        EditorUserBuildSettings.selectedBuildTargetGroup,
        string.Join(";", allDefines.ToArray()));
    }

    [MenuItem("Tools/BuildMode/LiveMode")]
    public static void SetLiveMode()
    {
        var group = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> allDefines = group.Split(';').ToList();
        allDefines.RemoveAll(item => item == "DEBUG_MODE");
        allDefines.RemoveAll(item => item == "LIVE_MODE");
        allDefines.Add("LIVE_MODE");

        PlayerSettings.SetScriptingDefineSymbolsForGroup(
        EditorUserBuildSettings.selectedBuildTargetGroup,
        string.Join(";", allDefines.ToArray()));
    }

    [MenuItem("Tools/SelectConfig/Meta Attribute Config")]
    public static void SelectConstFontConfig()
    {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath($"Assets/1.Project/Resources/ScriptableObject/MetaAttributeConfig.asset");
    }

    [MenuItem("Tools/ClearPlayerPrefabs")]
    public static void ClearPlayerPrefabs()
    {
        PlayerPrefs.DeleteAll();
    }

    //[MenuItem("Tools/DataExport/RewardType")]
    //public static void ExportRewardType() 
    //{
    //    var res = Resources.Load<TextAsset>("Json/TSDataRewardItemType");
    //    var dataList = JsonConvert.DeserializeObject<List<TSDataRewardItemType>>(res.text);

    //    string result = "";
    //    result += "public enum ERewardType\n";
    //    result += "{\n";

    //    foreach(var item in dataList)
    //    {
    //        result += $"    {item.typeName} = {item.type},\n";
    //    }
    //    result += "}";


    //    var dir = Path.Combine(Application.dataPath, "1.Project/1.Script/JA/GeneratedEnum");

    //    if (false == Directory.Exists(dir))
    //    {
    //        Directory.CreateDirectory(dir);
    //    }

    //    var resultPath = Path.Combine(dir, "ERewardType.cs");

    //    File.WriteAllText(resultPath, result);

    //    AssetDatabase.Refresh();
    //}
}
