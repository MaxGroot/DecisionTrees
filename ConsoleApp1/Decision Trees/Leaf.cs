﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    public class Leaf
    {
        public string value_splitter;
        public string classifier;
        private Node parent;

        public Leaf(string value_splitter, string classifier, Node parent)
        {
            this.value_splitter = value_splitter;
            this.classifier = classifier;
            this.parent = parent;
        }

        public string myRule(string targetAttribute)
        {
            string rule = this.parent.triggerRule(value_splitter);
            
            rule += " THEN " + targetAttribute + " = " + this.classifier;
            return rule;
        }
    }
}