using System.Collections.Generic;

namespace stromlaufplanToolsCLI.StromplanModels
{
    public class PlanData
    {
        public DocumentData documentData;

        public List<TreeNode> treeNodes;

        public Dictionary<string, TreeNodeDataBase> treeNodeDatas;
    }
}