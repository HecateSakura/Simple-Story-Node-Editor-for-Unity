using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public static class ScriptableObjectUtility
{
    public static T CreateSO<T>(string parentPath, string subFolder = "", string categoryFolder = "", string fileName = null) where T : ScriptableObject
    {
        // 生成默认文件名
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = $"New{typeof(T).Name}";
        }

        // 构建完整路径
        string finalPath = BuildFolderPath(parentPath, subFolder, categoryFolder);
        string assetPath = $"{finalPath}/{fileName}.asset";

        // 确保路径存在
        ValidateAndCreatePath(parentPath, subFolder, categoryFolder);

        // 创建或加载已有资产
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // 选中新创建的对象
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }
    public static void DeleteSO<T>(string parentPath, string subFolder = "", string categoryFolder = "", string fileName = null) where T : ScriptableObject
    {
        string finalPath = BuildFolderPath(parentPath, subFolder, categoryFolder);
        string assetPath = $"{finalPath}/{fileName}.asset";

        if(File.Exists(assetPath))
        {
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogWarning(assetPath + " 不存在！");
        }
    }
    private static void ValidateAndCreatePath(string parentPath, string subFolder, string categoryFolder)
    {
        // 验证父路径格式
        if (!parentPath.StartsWith("Assets/"))
        {
            parentPath = $"Assets/{parentPath}";
        }

        // 创建子文件夹
        string currentPath = parentPath;
        if (!string.IsNullOrEmpty(subFolder))
        {
            currentPath = $"{parentPath}/{subFolder}";
            if (!AssetDatabase.IsValidFolder(currentPath))
            {
                AssetDatabase.CreateFolder(parentPath, subFolder);
            }
        }

        // 创建分类文件夹
        if (!string.IsNullOrEmpty(categoryFolder))
        {
            string categoryPath = $"{currentPath}/{categoryFolder}";
            if (!AssetDatabase.IsValidFolder(categoryPath))
            {
                AssetDatabase.CreateFolder(currentPath, categoryFolder);
            }
        }
    }

    private static string BuildFolderPath(string parentPath, string subFolder, string categoryFolder)
    {
        // 自动修正路径格式
        parentPath = parentPath.Trim('/');
        subFolder = subFolder?.Trim('/');
        categoryFolder = categoryFolder?.Trim('/');

        // 拼接完整路径
        string path = parentPath;
        if (!string.IsNullOrEmpty(subFolder))
        {
            path += $"/{subFolder}";
        }
        if (!string.IsNullOrEmpty(categoryFolder))
        {
            path += $"/{categoryFolder}";
        }
        return path;
    }
}