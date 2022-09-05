﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Task")]
    public class ExportProjectTaskDto
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Label")]
        public string Label { get; set; }
    }
}
