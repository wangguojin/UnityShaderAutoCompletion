using System;
using System.Collections.Generic;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Content;
using Mono.TextEditor;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Core;
using System.Text.RegularExpressions;

namespace UnityShaderTool
{
	#region command
	public class InsertDateHandler : CommandHandler
	{
		protected override void Run ()
		{
			MonoDevelop.Ide.Gui.Document doc = IdeApp.Workbench.ActiveDocument;
			var textEditorData = doc.GetContent<ITextEditorDataProvider>().GetTextEditorData();
			string date = DateTime.Now.ToString();
			textEditorData.InsertAtCaret(date);
		}

		protected override void Update (CommandInfo info)
		{
			MonoDevelop.Ide.Gui.Document doc = MonoDevelop.Ide.IdeApp.Workbench.ActiveDocument;
			info.Enabled = doc != null && doc.GetContent<ITextEditorDataProvider>() != null;
		}
	}

	public enum UnityShaderToolCommands{
		InsertDate,
	}
	#endregion

	public class ShaderComplete : CompletionTextEditorExtension{
		protected delegate ICompletionDataList HandleCharTriggerEventHanlder(CodeCompletionContext completionContext);
		protected Dictionary<string,HandleCharTriggerEventHanlder>	m_lTriggerChar = new Dictionary<string, HandleCharTriggerEventHanlder> ();

		protected CodeCompletionContext m_ctxLastCompletion = new CodeCompletionContext();

		protected CgShader 				m_insShader = new CgShader ();
		public static readonly IconId	iconTest = "md-fun";

		public ShaderComplete(){
		}

		public override void Initialize ()
		{
			base.Initialize ();
			m_insShader.Initialize ();
			//m_ctxLastCompletion = null;
			m_lTriggerChar.Add (".", handleTriggerPoint);
			m_lTriggerChar.Add ("=", handleTriggerEqual);
		}


		public override void CursorPositionChanged ()
		{
			base.CursorPositionChanged ();
		}
		/// <summary>
		/// Handles the code completion.
		/// </summary>
		/// <returns>The code completion.</returns>
		/// <param name="completionContext">Completion context.</param>
		/// <param name="completionChar">Completion char.</param>
		/// <param name="triggerWordLength">该字段表示弹出的提示框中的匹配的字符长度，会标红显示，同时影响tab的填充字符</param>
		public override ICompletionDataList HandleCodeCompletion (CodeCompletionContext completionContext, char completionChar, ref int triggerWordLength)
		{
			if (m_lTriggerChar.ContainsKey(completionChar.ToString()) && completionContext.TriggerLineOffset > 1) {
				return m_lTriggerChar[completionChar.ToString()](completionContext);
			}

			var matches = m_insShader.getNoFilterCompletion(completionChar.ToString());
			CompletionDataList data = new CompletionDataList();
			string desc = "",context = "",distext = "";
			foreach (var item in matches) {
				m_insShader.getCompletionInfoData(item,ref desc,ref context,ref distext);
				data.Add(new CompletionData(distext,Stock.Field,desc,context));
			}
			m_ctxLastCompletion.TriggerLine = completionContext.TriggerLine;
			m_ctxLastCompletion.TriggerLineOffset = completionContext.TriggerLineOffset;
			m_ctxLastCompletion.TriggerOffset = completionContext.TriggerOffset;
			data.AutoCompleteUniqueMatch = false;
			triggerWordLength = 1;
			return data;
		}

		// ParameterDataProvider is for parameter info
		public override ParameterDataProvider HandleParameterCompletion (CodeCompletionContext completionContext, char completionChar)
		{
			return base.HandleParameterCompletion (completionContext, completionChar);
		}

		public override int GetCurrentParameterIndex (int startOffset)
		{
			return base.GetCurrentParameterIndex (startOffset);
		}

		protected ICompletionDataList handleTriggerPoint(CodeCompletionContext completionContext){
			string lineVal = Editor.GetLineText (completionContext.TriggerLine);
			int prefixIdx = lineVal.LastIndexOf ('.');
			string dstVal = lineVal.Substring (0, prefixIdx);
			int idx = dstVal.LastIndexOfAny (new char[]{ ' ', '\t' });
			if (idx >= 0 && idx <= dstVal.Length - 2) {
				string par = lineVal.Substring (idx + 1, dstVal.Length - idx - 1);
				getKeyLineInEditor (par);
			}
			return null;
		}

		protected ICompletionDataList handleTriggerEqual(CodeCompletionContext completionContext){
			return null;
		}

		private List<int> getKeyLineInEditor(string key){
			//todo
			return null;
			foreach (var item in Editor.Lines) {
				string val = Editor.GetLineText (item.LineNumber);
				if (Regex.IsMatch(val,key + "\\s*;")) {
					string[] tmpKeys = Regex.Split (val, "[\\s\\t]+");
				}
			}
		}
	}
}

