using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnityShaderTool
{
	public class JsonUtils
	{
		private static string	m_sFilePath = "unity_shader_tag.json";
		private JObject m_jobjData = null;
		public JsonUtils ()
		{
			try {
				string m_sJsonOriData = System.IO.File.ReadAllText(m_sFilePath,System.Text.Encoding.UTF8);
				m_jobjData = JObject.Parse(m_sJsonOriData);
				m_sJsonOriData = "";
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}

		public void ParseInerVariSets(List<ShaderInerVariModel> model){
			if (m_jobjData != null) {
				JArray rootArray = m_jobjData ["variner_sets"] as JArray;
				for (int i = 0; i < rootArray.Count; i++) {
					JToken val = rootArray[i];
					{
						ShaderInerVariModel iner = new ShaderInerVariModel ();
						iner.name = (string)val["name"];
						iner.file = (string)val ["file"];
						JToken desc = val ["variables"];
						iner.variables.Add ((string)desc ["desc"]);
						model.Add (iner);
					}
				}
			}
		}

		public void ParseStructSets(List<ShaderStructModel> model){
			if (m_jobjData != null) {
				JArray rootArray = m_jobjData ["struct_sets"] as JArray;
				for (int i = 0; i < rootArray.Count; i++) {
					JToken val = rootArray[i];
					ShaderStructModel iner = new ShaderStructModel ();
					iner.name = (string)val["name"];
					iner.file = (string)val["file"];
					JToken desc = val ["variables"];
					foreach (var item in desc.Children<JToken>()) {
						JProperty pro = item as JProperty;
						ShaderStructModel.StructInfo info = new ShaderStructModel.StructInfo ();
						info.name = pro.Name;
						JArray infoArray = pro.Value as JArray;
						for (int j = 0; j < infoArray.Count; j++) {
							info.info += (string)infoArray [j] + " ";
						}
						iner.variables.Add (info);
					}
					model.Add (iner);
				}
			}
		}

		public void ParseFuncSets(List<ShaderFuncModel> model){
			if (m_jobjData != null) {
				JArray rootArray = m_jobjData ["func_sets"] as JArray;
				for (int i = 0; i < rootArray.Count; i++) {
					JToken val = rootArray[i];
					ShaderFuncModel iner = new ShaderFuncModel ();
					iner.name = (string)val["name"];
					iner.file = (string)val ["file"];
					JToken desc = val ["variables"];
					iner.rettype = (string)desc ["ret"];
					JArray jpar = desc ["par"] as JArray;
					for (int j = 0; j < jpar.Count; j++) {
						JArray tarr = jpar [j] as JArray;
						ShaderFuncModel.FuncTem parInfo = new ShaderFuncModel.FuncTem();
						for (int h = 0; h < tarr.Count; h++) {
							if (h == 0) {
								parInfo.ptype = (string)tarr [h];
							}else{
								parInfo.pinfo += (string)tarr[h] + " ";
							}
						}
						iner.variables.Add (parInfo);
					}
					model.Add (iner);
				}
			}
		}

		public void ParseMacroSets(List<ShaderMacroModel> model){
			if (m_jobjData != null) {
				JArray rootArray = m_jobjData ["macro_sets"] as JArray;
				for (int i = 0; i < rootArray.Count; i++) {
					JToken val = rootArray[i];
					{
						ShaderMacroModel iner = new ShaderMacroModel ();
						iner.name = (string)val["name"];
						iner.file = (string)val ["file"];
						JToken desc = val ["variables"];
						iner.variables.Add ((string)desc ["desc"]);
						model.Add (iner);
					}
				}
			}
		}

		public void ParseHeadSets(ShaderHeadModel model){
			if (m_jobjData != null) {
				JArray rootArray = m_jobjData ["head_sets"] as JArray;
				for (int i = 0; i < rootArray.Count; i++) {
					model.m_lHeadSets.Add ((string)rootArray [i]);
				}
			}
		}

	}
}

