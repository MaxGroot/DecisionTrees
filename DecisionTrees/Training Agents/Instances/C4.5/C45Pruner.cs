﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class C45Pruner
    {
        // Since calculating upper bounds is a computationally expensive process, we store here the variables that we have done that calculation
        // of before. 
        private Dictionary<string, double> calculated_upperBounds = new Dictionary<string, double>();

        // This keeps track of nodes that have been removed, so they can later be snapshotted. [TODO: Make snapshotting work again?)
        private Dictionary<string, Node> pruned_nodes = new Dictionary<string, Node>();

        // Confidence level
        private double confidence;
        
        private Agent agent;
        public DecisionTree prune(DecisionTree tree, string target_attribute, double confidence, Agent agent)
        {
            this.agent = agent;
            this.confidence = confidence;

            // Sort them bottom-up.
            List<Node> queue = tree.sort_nodes_bottom_up();
            
            agent.THINK("prepare-for-pruning").finish();
            // Start post-pruning with this queue.
           
            DecisionTree pruned_tree = pruneIterate(tree, queue, target_attribute);

            // Return the pruned tree
            return pruned_tree;

        }

        private DecisionTree pruneIterate(DecisionTree tree, List<Node> queue, string target_attribute)
        {
            // Manage queue.
            Node node = queue[0];
            queue.RemoveAt(0);

            agent.THINK("consider-node-for-pruning").finish();

            // Lets consider this node.
            List<DataInstance> node_set = new List<DataInstance>();

            // Calculate error estimate of the leafs
            double leaf_estimated_errors = 0;
            int leaf_actual_errors = 0;
            foreach (Leaf child in SetHelper.all_leaf_children(node))
            {
                List<DataInstance> leaf_set = tree.data_locations[child];
                node_set.AddRange(leaf_set);

                // Calculate estimated error.
                int my_errors = SetHelper.subset_errors(leaf_set, target_attribute);
                leaf_actual_errors += my_errors;
                double errorRate = Calculator.confidenceIntervalExact(my_errors, leaf_set.Count, this.confidence);
                double estimatedError = errorRate * leaf_set.Count;
                leaf_estimated_errors += estimatedError;
            }
            
            // Calculate estimated error of node.
            int node_errors = SetHelper.subset_errors(node_set, target_attribute);
            double nodeErrorRate = Calculator.confidenceIntervalExact(node_errors, node_set.Count, this.confidence);
            double nodeEstimatedError = nodeErrorRate * node_set.Count;

            // Compare
            // If a node has a lower estimated error than its leafs, it should be pruned. 
            Dictionary<string, object> state = StateRecording.generateState("estimated_prune_errors", nodeEstimatedError, "estimated_keep_errors", leaf_estimated_errors,
                                                                            "node_attribute", node.label, "node_data_size", node_set.Count, "node_id", node.identifier, "node_value_splitter", (node.value_splitter != null) ? node.value_splitter : "NULL",
                                                                            "node_threshold", (node is ContinuousNode) ? (double?)(node as ContinuousNode).threshold : null,
                                                                            "parent_id", (node.getParent() != null) ? node.getParent().identifier : "NULL", "parent_attribute", (node.getParent() != null) ? node.getParent().label : "NULL", "parent_threshold", (node.getParent() != null && node.getParent() is ContinuousNode) ? (double?)((ContinuousNode) node.getParent()).threshold : null);

            if (nodeEstimatedError < leaf_estimated_errors)
            {
                // We need to prune!
                this.prepareSnapshot(node);
                agent.THINK("prune-node").setState(state).finish();
                
                tree = tree.replaceNodeByNewLeaf(node);
            }else
            {
                agent.THINK("keep-node").setState(state).finish();
            }

            // Iterate further if necessary. 
            if (queue.Count > 0)
            {
                tree = this.pruneIterate(tree, queue, target_attribute);
            }
            return tree;
        }
        
        private void prepareSnapshot(Node node)
        {
            foreach (Node child in node.getNodeChildren())
            {
                if (pruned_nodes.ContainsKey(child.identifier))
                {
                    pruned_nodes.Remove(child.identifier);
                }
            }
            pruned_nodes[node.identifier] = node;
        }
    }
}
