using System;
using System.Collections.Generic;

namespace UnityShaderTool
{
	public class ShaderConst
	{
		public static Dictionary<string,string>	m_dShaderFlags = new Dictionary<string,string>{
			{"#pragma",""},{"#define",""},{"CGPROGRAM",""},{"ENDCG",""},{"surface",""},{"Properties",""},{"SubShader",""},
			{"vertex","vertex func"},{"fragment","fragmment func"},{"SV_POSITION","semantics: SV_POSITION,surface vertex position"},{"COLOR","semantics:COLOR,per-vertex color"},
			{"POSITION","semantics:POSITION,vertex position"},{"NORMAL","semantics:NORMAL,vertex normal"},{"TEXCOORD0","semantics:TEXCOORD9,first uv coordinate"},
			{"TANGENT","semantics:TANGENT,used for normal mapping,float4"},{"_Time","time since load"},{"_SinTime",""},{"_CosTime",""},{"unity_DeltaTime",""},
			{"Pass","one pass one render"},{"Lambert","light model"},{"BlinnPhong","light model"},{"alpha","surface optionalparam"},
			{"alpha:auto","surface optionalparam"},{"alpha:fade","surface optionalparam"},{"alpha:premul","surface optionalparam"},
			{"alphatest:VariableName","surface optionalparam,cutoff value float with VN"},{"keepalpha","surface optionalparam"},
			{"decal:add","surface optionalparam"},{"decal:blend","surface optionalparam"},{"vertex:VertexFunc","surface optionalparam,custom vertex func"},
			{"finalcolor:ColorFunc","surface optionalparam,custom final color function"},{"Input","surface input struct"},
			{"Cull","Back|Front|Off"},{"ZTest","Less|Greater|LEqual|GEqual|Equal|NotEqual|Always"},{"ZWrite","On|Off"},{"Lighting","On|Off"},
			{"LOD","num"},{"UsePass","UsePass Shader/Name"},{"GrabPass","GrabPass{ } default name _GrabTexture"},
			{"Fallback","last shader"},{"Fog","Fog{Mode Off|Global|Linear|Exp|Exp2 \n Color ColorValue \n Density FloatVal \n Range Ft,Ft}"}
		};

		public static string getShaderFlagsInfo(string key){
			if (m_dShaderFlags.ContainsKey(key)) {
				return m_dShaderFlags [key];
			}
			return "";
		}

		public static string  m_kInputTag = "Input";

		public static Dictionary<string,string>	m_dInerDefaultStruct = new Dictionary<string, string>{
			{m_kInputTag,"struct Input{\n\tfloat3 viewDir;\n\tfloat4 color:COLOR;\n\t" +
				"float4 screenPos;\n\tfloat3 worldPos;\n\tfloat3 worldRefl;\n\tfloat3 worldNormal;\n\t" +
				"float2 uv_?Tex;};"},
		};

		public static string getInputInfo(string key){
			if (m_dInerDefaultStruct.ContainsKey(key)) {
				return m_dInerDefaultStruct [key];
			}
			return "";
		}

		public static Dictionary<string,string>	m_dPropertiesInfo = new Dictionary<string,string> {
			{"Range","Range (min,max) = number"},{"Float","number"},{"Int","number"},{"Color","(num,num,num,num)"},
			{"Vector","(num,num,num,num)"},{"2D","null/white/black/gray/bump{}"},
		};
		public static string getPropertiesInfo(string key){
			if (m_dPropertiesInfo.ContainsKey(key)) {
				return m_dPropertiesInfo [key];
			}
			return "";
		}

		public static string  		m_kBlendTag = "Blend";

		public static List<string>	m_lBlendInfo = new List<string>(){
			"Off/On","Add","Sub","RevSub","Min","Max","Logical?","One","Zero","SrcColor","SrcAlpha",
			"DstColor","DstAlpha","OneMinusSrcColor","OneMinusSrcAlpha","OneMinusDstColor","OneMinusDstAlpha"
		};

		public static Dictionary<string,List<string>>	m_dlSubShaderTagsInfo = new Dictionary<string, List<string>> {
			{"Queue",new List<string>(){"Background","Geometry","AlphaTest","Transparent","Overlay"}},
			{"RenderType",new List<string>(){"Opaque","Transparent","TransparentCutout","Background","Overlay","TreeOpaque",
					"TreeTransparentCutout","TreeBillboard","Grass","GrassBillboard"}},
			{"DisableBatching",new List<string>(){"True","False","LODFading"}},
			{"ForceNoShadowCasting",new List<string>(){"True","False"}},
			{"IgnoreProjector",new List<string>(){"True","False"}},
			{"PreviewType",new List<string>(){"True","False"}},
		};

		public static List<string> getSubTagInfo(string key){
			if (m_dlSubShaderTagsInfo.ContainsKey(key)) {
				return m_dlSubShaderTagsInfo [key];
			}
			return null;
		}

		public static string  m_kFuncTag = "Lighting";
		public static Dictionary<string,List<string>>	m_dFuncInfo	= new Dictionary<string, List<string>> { 
			{m_kFuncTag,new List<string>() {"half4 Lighting<Name> (SurfaceOutput s, half3 lightDir, half atten)",
					"half4 Lighting<Name> (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)"}},
		};

		public static List<string> getLightingInfo(string key){
			if (m_dFuncInfo.ContainsKey(key)) {
				return m_dFuncInfo [key];
			}
			return null;
		}

		public static List<string>	m_lBaseShaderType = new List<string> () {
			"uniform", "void", "int", "bool", "float", "float2", "float3", "float4",
			"float2x2", "float3x3", "float4x4",
			"fixed", "fixed2", "fixed3", "fixed4", "fixed2x2", "fixed3x3", "fixed4x4",
			"half", "half2", "half3", "half4", "half2x2", "half3x3", "half4x4", "in", "out", "inout"
		};
		
	}
}

