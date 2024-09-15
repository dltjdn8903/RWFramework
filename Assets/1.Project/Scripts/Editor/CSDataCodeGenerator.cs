using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MDF
{
    public class CSDataCodeGenerator
    {
        public string CS { get { return m_resultCSCode; } }
        public string Json { get { return m_resultJson; } }


        public bool TryGenerateFrom(string className, string csvText, bool usingSchemaTypeInfo)
        {
            csvText = csvText.Replace("\r", "");
            csvText = csvText.Replace("\\a", "_|");
            var lines = csvText.Split("\n", System.StringSplitOptions.RemoveEmptyEntries);
            int lineCount = lines.Length;
            if (lineCount < 1)
            {
                m_resultCSCode = "Invalid line count";
                return false;
            }

            var schemaLine = lines[0];
            var schemas = schemaLine.Split(",");

            //------------------
            //  trim : 휴먼 에러로 인해 컬럼명에 공백이 들어갈수 있다.
            //  " sch ema " => "schema"
            for (int i = 0; i < schemas.Length; ++i)
            {
                var trimSchema = schemas[i].Trim();
                schemas[i] = trimSchema;
            }
            schemas[0] = "tid";

            //------------------


            //------------------
            /*
                데이터 타입 정보.
                e-verse 에서 사용하던 csv양식을 그대로 사용.
                Line 0 : schema1,     schema2,     schema3, 
                Line 1 : schema1Type, schema2Type, schema3Type, 
                
                Line 1은 스키마 타입일수도, 데이터 라인의 시작일수도 있다.
                usingSchemaTypeInfo == true 면 Line 1을 스키마 타입으로 간주하겠다는 뜻.
            */
            string[] schemaTypes = null;
            if (2 <= lineCount && usingSchemaTypeInfo)
            {
                var schemaTypeLine = lines[1];
                schemaTypes = schemaTypeLine.Split(",");
            }

            if (!usingSchemaTypeInfo)
            {
                var types = new List<string>();
                for (int i = 0; i < schemas.Length; i++)
                {
                    types.Add("string");
                }
                schemaTypes = types.ToArray();
            }
            //------------------

            m_resultCSCode = BuildCSCode(className, schemas, schemaTypes);
            m_resultJson = BuildJson(schemas, lines, usingSchemaTypeInfo ? 2 : 1, schemaTypes);
            return true;
        }


        //----------------------------------------------------------------------
        #region private
        static char[] LINESEP = new char[] { '\r', '\n' };
        static char[] COLUMNSEP = new char[] { ',' };
        const string DEFAULT_TYPE = "string";   //  스키마 타입을 판정할수 없는경우 디폴트는 string.

        string m_resultCSCode;
        string m_resultJson;


        string BuildCSCode(string className, string[] schemas, string[] schemaTypes)
        {
            StringBuilder csCode = new StringBuilder();

            csCode.AppendLine("using Newtonsoft.Json;\n");
            csCode.AppendLine("using System.Collections.Generic;");
            csCode.AppendLine("using System.Threading.Tasks;");
            csCode.AppendLine("using UnityEngine;");
            csCode.AppendLine("using UnityEngine.AddressableAssets;");
            csCode.AppendLine();

            csCode.AppendLine("[System.Serializable]");
            csCode.AppendLine($"public class {className} : DataModelBase");
            csCode.AppendLine("{");
            csCode.AppendLine($"    private static Dictionary<string, {className}> dataDictionary = new Dictionary<string, {className}>();");
            csCode.AppendLine($"    private static List<{className}> dataList = new List<{className}>();");
            csCode.AppendLine();

            for (int i = 1; i < schemas.Length; i++)
            {
                csCode.AppendLine($"    public {schemaTypes[i]} {schemas[i]};");
            }
            csCode.AppendLine();

            csCode.AppendLine("    public override string ToString()");
            csCode.AppendLine("    {");
            csCode.AppendLine("        return JsonConvert.SerializeObject(this, Formatting.Indented);");
            csCode.AppendLine("    }");
            csCode.AppendLine();

            csCode.AppendLine("    public static void Load()");
            csCode.AppendLine("    {");
            csCode.AppendLine("        dataList.Clear();");
            csCode.AppendLine("        dataDictionary.Clear();");
            csCode.AppendLine("        var res = Resources.Load<TextAsset>($\"Json/{typeof(" + className + ").Name}\");");
            csCode.AppendLine("        dataList = JsonConvert.DeserializeObject<List<" + className + ">>(res.text);");
            csCode.AppendLine("        foreach(var item in dataList)");
            csCode.AppendLine("        {");
            csCode.AppendLine("            dataDictionary.Add(item.tid, item);");
            csCode.AppendLine("        }");
            csCode.AppendLine("    }");
            csCode.AppendLine();

            csCode.AppendLine("    async public static Task LoadAsync()");
            csCode.AppendLine("    {");
            csCode.AppendLine("        dataList.Clear();");
            csCode.AppendLine("        dataDictionary.Clear();");
            csCode.AppendLine("        var res = Addressables.LoadAssetAsync<TextAsset>($\"Json/{typeof(" + className + ").Name}\");");
            csCode.AppendLine("        await res.Task;");
            csCode.AppendLine("        dataList = JsonConvert.DeserializeObject<List<" + className + ">>(res.Result.text);");
            csCode.AppendLine("        foreach (var item in dataList)");
            csCode.AppendLine("        {");
            csCode.AppendLine("            dataDictionary.Add(item.tid, item);");
            csCode.AppendLine("        }");
            csCode.AppendLine("    }");
            csCode.AppendLine();

            csCode.AppendLine($"    public static {className} Find(string tid)");
            csCode.AppendLine("    {");
            csCode.AppendLine("        if (dataDictionary.ContainsKey(tid))");
            csCode.AppendLine("        {");
            csCode.AppendLine("            return dataDictionary[tid];");
            csCode.AppendLine("        }");
            csCode.AppendLine("        else");
            csCode.AppendLine("        {");
            csCode.AppendLine("            Log.Debug($\"tid를 찾을 수 없습니다. [tid: {tid}]\");");
            csCode.AppendLine("            return null;");
            csCode.AppendLine("        }");
            csCode.AppendLine("    }");
            csCode.AppendLine();

            csCode.AppendLine($"    public static List<{className}> GetValues()");
            csCode.AppendLine("    {");
            csCode.AppendLine("        return dataList;");
            csCode.AppendLine("    }");
            csCode.AppendLine();
            csCode.AppendLine("}");

            return csCode.ToString();
        }

        string BuildJson(string[] schemas, string[] dataLines, int beginLineNumber, string[] schemaTypes)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            result.Clear();

            for (int i = beginLineNumber; i < dataLines.Length; i++)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                var split = dataLines[i].Split(",");

                for (int j = 0; j < schemas.Count(); j++)
                {


                    if (schemaTypes[j] == "bool")
                    {
                        if (split[j] == "0" || split[j] == "1")
                        {
                            split[j] = System.Convert.ToBoolean(System.Convert.ToInt32(split[j])).ToString();
                        }

                        split[j] = split[j].ToLower();
                    }

                    if (split[j] == "NULL")
                    {
                        split[j] = "null";
                    }

                    dictionary.Add(schemas[j], split[j].Replace("_|", ",").Replace("\\n", "\n"));
                }
                result.Add(dictionary);
            }

            var jsonString = JsonConvert.SerializeObject(result, Formatting.Indented);

            return jsonString;
        }
        #endregion
    }
}

