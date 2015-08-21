﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class EditorLabelAttribute : Attribute
    {
        public string Label { get; private set; }

        public EditorLabelAttribute(string label)
        {
            Label = label;
        }
    }
}
