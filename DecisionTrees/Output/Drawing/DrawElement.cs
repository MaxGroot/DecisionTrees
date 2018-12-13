﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DrawElement
    {
        public string label;
        public int x, y;
        public Node node;

        public DrawElement(Node node, string label, int x, int y)
        {
            this.label = label;
            this.node = node;
            this.x = x;
            this.y = y;
        }
    }
}