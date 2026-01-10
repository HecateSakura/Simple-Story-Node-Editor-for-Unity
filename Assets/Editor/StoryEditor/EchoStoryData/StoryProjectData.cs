using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace Rift.Echo.Story
{
    [Serializable]
    public class StoryProjectData
    {
        public string projectId;
        public string projectName;
        public string version = "1.0";
        public string author = "";
        public string description = "";
        public DateTime createTime;
        public DateTime lastModifiedTime;

        // 项目设置
        public ProjectSettings settings = new ProjectSettings();

        // 所有节点
        public Dictionary<string, StoryNodeData> nodes = new Dictionary<string, StoryNodeData>();

        // 节点连接关系
        public List<NodeConnection> connections = new List<NodeConnection>();

        // 入口节点ID
        public string startNodeId = "";

        // 变量定义（用于条件分支）
        public Dictionary<string, VariableDefinition> variables = new Dictionary<string, VariableDefinition>();

        // 标签（用于快速定位）
        public Dictionary<string, List<string>> tags = new Dictionary<string, List<string>>();

        // 分支选项定义
        public Dictionary<string, BranchOption> branchOptions = new Dictionary<string, BranchOption>();

        public StoryProjectData(string id, string name)
        {
            projectId = id;
            projectName = name;
            createTime = DateTime.Now;
            lastModifiedTime = DateTime.Now;
        }
    }

    [Serializable]
    public class ProjectSettings
    {
        public bool enableAutoSave = true;
        public float autoSaveInterval = 300f; // 秒
        public string defaultLanguage = "zh-CN";
        public List<string> supportedLanguages = new List<string> { "zh-CN", "en-US", "ja-JP" };

        // 显示设置
        public bool showNodeGrid = true;
        public float gridSize = 20f;
        public Vector2Data gridOffset = new Vector2Data(0, 0);

        // 导出设置
        public bool compressJson = true;
        public bool includeMetadata = true;
        public string exportFormat = "json"; // json, xml, binary
    }

    [Serializable]
    public class NodeConnection
    {
        public string connectionId;
        public string fromNodeId;
        public string toNodeId;

        // 连接类型：normal, branch, condition, jump
        public string connectionType = "normal";

        // 条件（用于条件连接）
        public Condition condition = new Condition();

        // 分支选项ID（用于手动选择分支）
        public string branchOptionId = "";

        // 连接点位置（用于可视化）
        public Vector2Data fromPortPosition = new Vector2Data(0, 0);
        public Vector2Data toPortPosition = new Vector2Data(0, 0);

        // 连接线样式
        public string lineColor = "#FFFFFF";
        public float lineWidth = 2f;
        public string lineStyle = "solid"; // solid, dashed, dotted

        public NodeConnection(string fromId, string toId)
        {
            connectionId = Guid.NewGuid().ToString();
            fromNodeId = fromId;
            toNodeId = toId;
        }
    }

    [Serializable]
    public class Condition
    {
        // 条件类型：always, variable, flag, item, achievement, time, random
        public string conditionType = "always";

        // 变量条件
        public string variableName = "";
        public string comparison = "equals"; // equals, not_equals, greater, less, greater_or_equals, less_or_equals
        public string value = "";

        // 复合条件
        public List<Condition> subConditions = new List<Condition>();
        public string logicalOperator = "and"; // and, or

        // 随机条件
        public float probability = 1.0f; // 0-1

        // 时间条件
        public string timeCondition = ""; // day, night, specific_time

        public bool Evaluate(Dictionary<string, object> context)
        {
            // 条件求值逻辑
            switch (conditionType)
            {
                case "always":
                    return true;

                case "variable":
                    return EvaluateVariableCondition(context);

                case "random":
                    return UnityEngine.Random.value <= probability;

                case "flag":
                    return context.ContainsKey(variableName) && Convert.ToBoolean(context[variableName]);

                default:
                    return true;
            }
        }

        private bool EvaluateVariableCondition(Dictionary<string, object> context)
        {
            if (!context.ContainsKey(variableName))
                return false;

            var currentValue = context[variableName].ToString();

            switch (comparison)
            {
                case "equals":
                    return currentValue == value;

                case "not_equals":
                    return currentValue != value;

                case "greater":
                    if (float.TryParse(currentValue, out float currentNum) && float.TryParse(value, out float targetNum))
                        return currentNum > targetNum;
                    return false;

                case "less":
                    if (float.TryParse(currentValue, out currentNum) && float.TryParse(value, out targetNum))
                        return currentNum < targetNum;
                    return false;

                default:
                    return false;
            }
        }
    }

    [Serializable]
    public class VariableDefinition
    {
        public string variableId;
        public string displayName;
        public string description = "";

        // 变量类型：int, float, string, bool, enum, list
        public string variableType = "int";

        // 默认值
        public string defaultValue = "";

        // 值范围（用于数值类型）
        public float minValue = 0;
        public float maxValue = 100;

        // 枚举选项
        public List<string> enumOptions = new List<string>();

        // 是否可编辑
        public bool isEditable = true;

        // 是否持久化
        public bool isPersistent = true;

        public VariableDefinition(string id, string name)
        {
            variableId = id;
            displayName = name;
        }
    }

    [Serializable]
    public class BranchOption
    {
        public string optionId;
        public string displayText;
        public string description = "";

        // 显示条件
        public Condition displayCondition = new Condition();

        // 选择后的效果
        public List<VariableEffect> effects = new List<VariableEffect>();

        // 是否隐藏选项
        public bool isHidden = false;

        // 跳转的节点ID
        public string targetNodeId = "";

        // 选项位置（用于UI布局）
        public Vector2Data position = new Vector2Data(0, 0);

        public BranchOption(string id, string text)
        {
            optionId = id;
            displayText = text;
        }
    }

    [Serializable]
    public class VariableEffect
    {
        public string variableId;
        public string operation = "set"; // set, add, subtract, multiply, divide
        public string value = "";

        // 是否立即生效
        public bool immediate = true;
    }
}