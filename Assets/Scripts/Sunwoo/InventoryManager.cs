// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using System.IO;

// public class InventoryManager : MonoBehaviour
// {
//     private Dictionary<int, int> ingredientCounts = new Dictionary<int, int>(); // �ε��� ������� ��� ���� ����
//     public List<Ingred> ingreList = new List<Ingred>(); // CSV���� �ҷ��� ��� ����Ʈ
//     private GameData gameData; // DataManager���� �ҷ��� ������
//     private string csvFileName = "ingredient.csv"; // CSV ���ϸ�

//     // ������� �ִ� ��� ��ư ����Ʈ �߰�
//     public List<GameObject> refrigeratorButtons = new List<GameObject>();

//     [System.Serializable]
//     public class Ingred
//     {
//         public int index;
//         public string name;
//         public int type;
//         public int price;
//         public string ename; // ���� �̸� �߰�

//         public Ingred(int index, string name, int type, int price, string ename)
//         {
//             this.index = index;
//             this.name = name;
//             this.type = type;
//             this.price = price;
//             this.ename = ename;
//         }
//     }

//     void Start()
//     {
//         // CSV���� ��� ���� �ҷ�����
//         LoadIngredientsFromCSV();

//         Debug.Log($"ingreList�� �ε�� ��� ����: {ingreList.Count}");

//         // DataManager���� ����� ��� ���� �ҷ�����
//         LoadIngredientsFromGameData();

//         // ����� ��� ��ư ������Ʈ
//         UpdateRefrigeratorButtons();

//         PrintIngredientList();
//     }

//     // ����� ��� ��ư ������Ʈ (index ������� ��ư Ȱ��ȭ)
//     public void UpdateRefrigeratorButtons()
//     {
//         foreach (GameObject buttonObj in refrigeratorButtons)
//         {
//             string buttonName = buttonObj.name.Replace("Button", ""); // ��ư �̸����� "Button" ����
//             int ingredientIndex = GetIngredientIndexFromEname(buttonName); // ename�� index�� ��ȯ

//             bool hasIngredient = HasIngredient(ingredientIndex);

//             buttonObj.SetActive(hasIngredient);
//             Debug.Log($"��� ��ư ������Ʈ: {buttonName} (Index: {ingredientIndex}), ���� ����: {hasIngredient}");
//         }
//     }

//     // GameDataManager���� ��� ���� �ҷ�����
//     private void LoadIngredientsFromGameData()
//     {
//         gameData = DataManager.Instance?.LoadGameData();

//         if (gameData == null)
//         {
//             Debug.LogError("LoadIngredientsFromGameData: GameData�� null�Դϴ�! DataManager���� �����͸� ������ �� �����ϴ�.");
//             return;
//         }

//         if (gameData.ingredientNum == null)
//         {
//             Debug.LogWarning("LoadIngredientsFromGameData: ingredientNum�� null�̹Ƿ� �� ����Ʈ�� �ʱ�ȭ�մϴ�.");
//             gameData.ingredientNum = new List<int>(new int[ingreList.Count]); // �� ����Ʈ �ʱ�ȭ
//         }

//         // CSV���� �ҷ��� ��� ����Ʈ�� ��Ī�Ͽ� ���� ����
//         for (int i = 0; i < ingreList.Count; i++)
//         {
//             int ingredientIndex = ingreList[i].index;
//             int count = (i < gameData.ingredientNum.Count) ? gameData.ingredientNum[i] : 0;
//             ingredientCounts[ingredientIndex] = count;
//         }

//         Debug.Log("��� �����͸� ���������� �ҷ��Խ��ϴ�.");
//         PrintCurrentInventory();
//     }

//     // ��� ���� ����
//     private void SaveIngredients()
//     {
//         if (gameData == null)
//         {
//             Debug.LogError("SaveIngredients: GameData�� null�̹Ƿ� ������ �� �����ϴ�.");
//             return;
//         }

//         // ingredientCounts ��ųʸ� ���� ingredientNum ����Ʈ�� ��ȯ
//         List<int> updatedIngredientNum = new List<int>();

//         foreach (var ingredient in ingreList)
//         {
//             if (ingredientCounts.ContainsKey(ingredient.index))
//             {
//                 updatedIngredientNum.Add(ingredientCounts[ingredient.index]);
//             }
//             else
//             {
//                 updatedIngredientNum.Add(0);
//             }
//         }

//         // ������Ʈ�� ������ ����
//         gameData.ingredientNum = updatedIngredientNum;
//         DataManager.Instance.SaveGameData();
//     }

//     // CSV���� ��� ��� �ҷ�����
//     private void LoadIngredientsFromCSV()
//     {
//         try
//         {
//             TextAsset csvFile = Resources.Load<TextAsset>("ingredient");
//             if (csvFile == null)
//             {
//                 Debug.LogError("CSV ������ ã�� �� �����ϴ�: ingredient.csv");
//                 return;
//             }

//             string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

//             for (int i = 1; i < lines.Length; i++) // ù ��° ��(���) ����
//             {
//                 string[] fields = lines[i].Split(',');
//                 if (fields.Length < 5) continue; // ename���� �ִ��� Ȯ��

//                 int index = int.Parse(fields[0].Trim());
//                 string name = fields[1].Trim();
//                 int type = int.Parse(fields[2].Trim());
//                 int price = int.Parse(fields[3].Trim());
//                 string ename = fields[4].Trim().ToLower(); // ename �߰�

//                 ingreList.Add(new Ingred(index, name, type, price, ename));

//                 Debug.Log($"�ε��: {index}, {name}, {ename}");
//             }

//             Debug.Log($"�� {ingreList.Count}���� ��Ḧ CSV���� �ҷ��Խ��ϴ�.");
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError($"CSV ���� �б� �� ���� �߻�: {ex.Message}");
//         }
//     }

//     // ��� �߰�
//     public void AddIngredient(int ingredientIndex, int count = 1)
//     {
//         if (ingredientCounts.ContainsKey(ingredientIndex))
//         {
//             ingredientCounts[ingredientIndex] += count;
//         }
//         else
//         {
//             ingredientCounts[ingredientIndex] = count;
//         }

//         SaveIngredients();
//         UpdateRefrigeratorButtons(); // ��ᰡ �߰��Ǿ����Ƿ� ��ư ������Ʈ
//     }

//     // ��� ���� ���� (���)
//     public bool UseIngredient(int ingredientIndex)
//     {
//         if (ingredientCounts.ContainsKey(ingredientIndex) && ingredientCounts[ingredientIndex] > 0)
//         {
//             ingredientCounts[ingredientIndex]--;
//             SaveIngredients();
//             UpdateRefrigeratorButtons(); // ��ᰡ ���������Ƿ� ��ư ������Ʈ
//             return true;
//         }
//         return false;
//     }

//     // ��� ���� ����
//     public bool HasIngredient(int ingredientIndex)
//     {
//         return ingredientCounts.ContainsKey(ingredientIndex) && ingredientCounts[ingredientIndex] > 0;
//     }

//     // ���� �̸�(ename)���� �ε��� ã��
//     public int GetIngredientIndexFromEname(string ename)
//     {
//         ename = ename.Trim().ToLower();
//         Debug.Log($"Searching for Ingredient: '{ename}'");

//         foreach (var ingredient in ingreList)
//         {
//             if (ingredient.ename == ename) // ename �������� ��ȸ
//             {
//                 return ingredient.index;
//             }
//         }

//         //Debug.LogError($"GetIngredientIndexFromEname: ��� ���� �̸� '{ename}'�� ã�� �� �����ϴ�. ���� ingreList�� ename ���: ");
//         /*foreach (var ingredient in ingreList)
//         {
//             Debug.Log($"- {ingredient.ename}");
//         }*/

//         return -1;
//     }

//     // �ε����� ��� �̸� ã��
//     public string GetIngredientEname(int index)
//     {
//         foreach (var ingredient in ingreList)
//         {
//             if (ingredient.index == index)
//             {
//                 return ingredient.ename;
//             }
//         }
//         Debug.LogError($"GetIngredientEname: ��� �ε��� '{index}'�� ã�� �� �����ϴ�.");
//         return null;
//     }

//     // ���� ������ ��� ��� ���
//     private void PrintCurrentInventory()
//     {
//         Debug.Log("���� ������ ���:");
//         foreach (var ingredient in ingredientCounts)
//         {
//             string ename = GetIngredientEname(ingredient.Key);
//             Debug.Log($"- {ename} ({ingredient.Key}): {ingredient.Value}��");
//         }
//     }

//     private void PrintIngredientList()
//     {
//         Debug.Log("���� ingreList�� ��� ���:");
//         foreach (var ingredient in ingreList)
//         {
//             Debug.Log($"- Index: {ingredient.index}, Name: {ingredient.name}, eName: {ingredient.ename}");
//         }
//     }
// }
