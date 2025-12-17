using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class EnemyDecisionData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Resources/MasterData/EnemyDecisionData.xlsx";
	private static readonly string exportPath = "Assets/Resources/MasterData/EnemyDecisionData.asset";
	private static readonly string[] sheetNames = { "EnemyFactorData", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Entity_EnemyDecisionData data = (Entity_EnemyDecisionData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_EnemyDecisionData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_EnemyDecisionData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					Entity_EnemyDecisionData.Sheet s = new Entity_EnemyDecisionData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_EnemyDecisionData.Param p = new Entity_EnemyDecisionData.Param ();
						
					cell = row.GetCell(0); p.EnemyID = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.ClosePlayerDistance = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.FarPlayerDistance = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.MinCoolTime = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.MaxCoolTime = (float)(cell == null ? 0 : cell.NumericCellValue);
					p.PlayerActiveMagic = new int[2];
					cell = row.GetCell(5); p.PlayerActiveMagic[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.PlayerActiveMagic[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
