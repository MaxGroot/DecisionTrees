﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class DataInstance
    {
        Dictionary<string, string> fields = new Dictionary<string, string> ();

        // Set an attribute to a value of this Data Instance. 
        public void setProperty(string attribute, string value)
        {
            this.fields.Add(attribute, value);
        }

        // Access an attribute of this Data Instance
        public string getProperty(string attribute)
        {
            if (this.fields.ContainsKey(attribute))
            {
                return this.fields[attribute];
            }
            throw new Exception($"Dictionary did not contain {attribute}");
        }
    }
}