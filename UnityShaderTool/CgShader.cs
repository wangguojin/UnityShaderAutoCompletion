using System;
using System.Collections.Generic;
using TernarySearchTree;
using MonoDevelop.Ide.CodeCompletion;


namespace UnityShaderTool
{
	public enum ShModelType
	{
		ModelNull,
		ModelInerVar,
		ModelMacro,
		ModelFunc,
		ModelStruct,
		ModelHead,
		ModelShaderFlag,
		ModelInput,
		ModelProperties,
		ModelBlendFlag,
		ModelSubTags,
		ModelLighting
	}

	public class CompletionParam{
		public string	m_sdesc = "";
		public string	m_sdistext = "";
		public string	m_scomtext = "";
	}

	public class UsdKeyWordDesc{
		public ShModelType	m_eType;
		public string		m_skey = "";
		public UsdKeyWordDesc(ShModelType mlType,string key){
			m_eType = mlType;
			m_skey = key;
		}

	}

	public class ShaderInerVariModel
	{
		public string name = "";
		public string file = "";
		public List<string>	variables = new List<string>();
	}

	public class ShaderStructModel{
		public class StructInfo{
			public string name = "";
			public string info = "";
		}
		public string name = "";
		public string file = "";
		public List<StructInfo> variables = new List<StructInfo>();
	}

	public class ShaderHeadModel{
		public List<string>	m_lHeadSets = new List<string>();
	}

	public class ShaderFuncModel{
		public class FuncTem{
			public string ptype = "";
			public string pinfo = "";
		}
		public string name = "";
		public string file = "";
		public string rettype = "";
		public List<FuncTem> variables = new List<FuncTem>();
	}

	public class ShaderMacroModel
	{
		public string name = "";
		public string file = "";
		public List<string>	variables = new List<string>();
	}

	public class CgShader
	{
		private JsonUtils m_JsonHelper = null;
		private List<ShaderInerVariModel>	m_lInerVariSets = new List<ShaderInerVariModel> ();
		private List<ShaderStructModel> m_lStructSets = new List<ShaderStructModel> ();
		private List<ShaderFuncModel>	m_lFuncSets = new List<ShaderFuncModel> ();
		private List<ShaderMacroModel>	m_lMacroSets = new List<ShaderMacroModel> ();
		private ShaderHeadModel			m_lHeadSets = new ShaderHeadModel ();
		private TernaryTree<UsdKeyWordDesc> 						m_tSearchUtils = new TernaryTree<UsdKeyWordDesc> ();
		private Dictionary<ShModelType,Action<string,CompletionParam>>	m_dFuncHandlerMap = new Dictionary<ShModelType, Action<string,CompletionParam>>();

		private void buildDefaultType(){
			foreach (var item in ShaderConst.m_dShaderFlags.Keys) {
				UsdKeyWordDesc desc = new UsdKeyWordDesc (ShModelType.ModelShaderFlag,item);
				m_tSearchUtils.Add (item, desc);
			}
			//
			UsdKeyWordDesc descInput = new UsdKeyWordDesc(ShModelType.ModelInput,ShaderConst.m_kInputTag);
			m_tSearchUtils.Add(ShaderConst.m_kInputTag,descInput);
			//
			foreach (var item in ShaderConst.m_dPropertiesInfo.Keys) {
				m_tSearchUtils.Add (item, new UsdKeyWordDesc (ShModelType.ModelProperties,item));
			}
			//
			m_tSearchUtils.Add (ShaderConst.m_kBlendTag, new UsdKeyWordDesc (ShModelType.ModelBlendFlag,ShaderConst.m_kBlendTag));
			//
			foreach (var item in ShaderConst.m_dlSubShaderTagsInfo.Keys) {
				m_tSearchUtils.Add (item, new UsdKeyWordDesc (ShModelType.ModelSubTags,item));
			}
			//
			m_tSearchUtils.Add (ShaderConst.m_kFuncTag, new UsdKeyWordDesc (ShModelType.ModelFunc,ShaderConst.m_kFuncTag));
			//
			foreach (var item in ShaderConst.m_lBaseShaderType) {
				m_tSearchUtils.Add (item, new UsdKeyWordDesc (ShModelType.ModelNull,item));
			}

		}

		private void buildKeyMap(){
			//1 build head tag
			foreach (string item in m_lHeadSets.m_lHeadSets) {
				m_tSearchUtils.Add (item, new UsdKeyWordDesc(ShModelType.ModelHead,item));
			}
			//2 build iner variables 
			foreach (var item in m_lInerVariSets) {
				m_tSearchUtils.Add (item.name, new UsdKeyWordDesc(ShModelType.ModelInerVar,item.name));
			}
			//3 build the macro sets
			foreach (var item in m_lMacroSets) {
				m_tSearchUtils.Add (item.name, new UsdKeyWordDesc(ShModelType.ModelMacro,item.name));
			}
			//4 build the func sets
			foreach (var item in m_lFuncSets) {
				m_tSearchUtils.Add (item.name, new UsdKeyWordDesc(ShModelType.ModelFunc,item.name));
			}
			//5 build the struct sets
			foreach (var item in m_lStructSets) {
				m_tSearchUtils.Add (item.name, new UsdKeyWordDesc(ShModelType.ModelStruct,item.name));
			}
			//6 build default base type
			buildDefaultType ();
		}

		private void registerFuncMap(){
			m_dFuncHandlerMap.Add(ShModelType.ModelInerVar,handleInerVarTags);
			m_dFuncHandlerMap.Add(ShModelType.ModelMacro,handleMacroTags);
			m_dFuncHandlerMap.Add(ShModelType.ModelFunc,handleFuncTags);
			m_dFuncHandlerMap.Add(ShModelType.ModelStruct,handleStructTags);
			m_dFuncHandlerMap.Add(ShModelType.ModelHead,handleHeadTags);
			m_dFuncHandlerMap.Add(ShModelType.ModelShaderFlag,handleShaderFlagTags);
			m_dFuncHandlerMap.Add(ShModelType.ModelInput,handleInputTags);
			m_dFuncHandlerMap.Add(ShModelType.ModelProperties,handlePropertiesTags);
			m_dFuncHandlerMap.Add(ShModelType.ModelBlendFlag,handleBlendTags);
			m_dFuncHandlerMap.Add(ShModelType.ModelSubTags,handleSubTags);
			m_dFuncHandlerMap.Add(ShModelType.ModelLighting,handleLightTags);
		}

		public void Initialize(){
			m_JsonHelper = new JsonUtils ();
			m_JsonHelper.ParseInerVariSets (m_lInerVariSets);
			m_JsonHelper.ParseFuncSets (m_lFuncSets);
			m_JsonHelper.ParseHeadSets (m_lHeadSets);
			m_JsonHelper.ParseMacroSets (m_lMacroSets);
			m_JsonHelper.ParseStructSets (m_lStructSets);
			buildKeyMap ();
			registerFuncMap();
		}

		private void handleInerVarTags(string key,CompletionParam data){
			foreach (var item in m_lInerVariSets) {
				if (item.name.Equals(key)) {
					data.m_sdistext = key;
					data.m_scomtext = item.name;
					data.m_sdesc = item.variables[0] + " " + item.file;
					break;
				}
			}
		}

		private void handleMacroTags(string key,CompletionParam data){
			foreach (var item in m_lMacroSets) {
				if (item.name.Equals(key)) {
					data.m_sdistext = key;
					data.m_scomtext = item.name;
					data.m_sdesc = item.variables[0] + " " + item.file;
					break;
				}
			}
		}

		private void handleFuncTags(string key,CompletionParam data){
			foreach (var item in m_lFuncSets) {
				if (item.name.Equals(key)) {
					data.m_sdistext = key;
					string des = item.rettype + " ",par = "( ";
					for (int i = 0; i < item.variables.Count; i++) {
						ShaderFuncModel.FuncTem fun = item.variables[i];
						string val = fun.ptype + " " + fun.pinfo + ",";
						if (i == item.variables.Count -1) {
							val = fun.ptype + " " + fun.pinfo + ")";
						}
						des += val;
						par += val;
					}
					data.m_scomtext = par;
					data.m_sdesc = des + " " + item.file;
					break;
				}
			}
		}

		private void handleStructTags(string key,CompletionParam data){
			foreach (var item in m_lStructSets) {
				if (item.name.Equals(key)) {
					data.m_sdistext = key;
					data.m_scomtext = item.name;
					data.m_sdesc = item.file;
					break;
				}
			}
		}

		private void handleHeadTags(string key,CompletionParam data){
			foreach (var item in m_lHeadSets.m_lHeadSets) {
				if (item.Equals(key)) {
					data.m_scomtext = key;
					data.m_sdesc = key;
					return;
				}
			}
		}

		private void handleShaderFlagTags(string key,CompletionParam data){
			foreach (var item in ShaderConst.m_dShaderFlags.Keys) {
				if (item.Equals(key)) {
					data.m_sdistext = key;
					data.m_scomtext = key;
					data.m_sdesc	= ShaderConst.getShaderFlagsInfo(key);
					break;
				}
			}
		}

		private void handleInputTags(string key,CompletionParam data){
			foreach (var item in ShaderConst.m_dInerDefaultStruct.Keys) {
				if (item.Equals(key)) {
					data.m_sdistext = key;
					data.m_scomtext = ShaderConst.getInputInfo(key);
					break;
				}
			}
		}

		private void handlePropertiesTags(string key,CompletionParam data){
			foreach (var item in ShaderConst.m_dPropertiesInfo.Keys) {
				if (item.Equals(key)) {
					data.m_sdistext = key;
					data.m_scomtext = key;
					data.m_sdesc = ShaderConst.getPropertiesInfo(key);
					break;
				}
			}
		}

		private void handleBlendTags(string key,CompletionParam data){
			data.m_sdistext = key;
			data.m_scomtext = key;

		}

		private void handleSubTags(string key,CompletionParam data){
			if (ShaderConst.m_dlSubShaderTagsInfo.ContainsKey(key)) {
				data.m_sdistext = key;
				data.m_scomtext = key;
				List<string>	desc = ShaderConst.getSubTagInfo(key);
				for (int i = 0; i < desc.Count; i++) {
					data.m_sdesc += desc[i] + "/";
				}
			}
		}

		private void handleLightTags(string key,CompletionParam data){
			if (ShaderConst.m_dFuncInfo.ContainsKey(key)) {
				data.m_sdistext = ShaderConst.getLightingInfo(key)[0];
				data.m_scomtext = ShaderConst.getLightingInfo(key)[0];
			}
		}

		public IEnumerable<UsdKeyWordDesc> getNoFilterCompletion(string completionStr){
			return m_tSearchUtils.Search(completionStr);
		}

		public void getCompletionInfoData(UsdKeyWordDesc keydesc,ref string desc,ref string comtext,ref string distex){
			CompletionParam param = new CompletionParam ();
			if (m_dFuncHandlerMap.ContainsKey(keydesc.m_eType)) {
				m_dFuncHandlerMap [keydesc.m_eType] (keydesc.m_skey, param);
				desc = param.m_sdesc;
				comtext = param.m_scomtext;
				distex = param.m_sdistext;
			}
			param = null;
		}

	}
}

