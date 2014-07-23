using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JJBJ.Plist
{
    public class PlistDictionary
    {
        private Dictionary<string, PlistValue> values;
    }

    public class PlistValue 
    {
        private XmlNode value;
        
        public PlistValue(XmlNode value)
        {
            switch (GetValueType())
            {
                case "string":
                case "real":
                case "integer":
                case "date":
                case "data":
                case "array":
                case "dict":
                {
                    this.value = value;
                }
                break;
                
                default:
                {
                    throw new InvalidOperationException("Plist value is not found.");
                }
            }
        }
        
        public string GetValueType()
        {
            return this.value.Name;
        }
        
        public object GetValue()
        {
            switch (GetValueType())
            {
                case "string":
                {
                    return this.value.InnerText;
                }
                
                case "real":
                case "integer":
                {
                    return Convert.ToDecimal(this.value.InnerText);
                }
                
                case "date":
                {
                    return DateTime.ParseExact(this.value.InnerText, "yyyyMMdd", null);
                }
                
                case "data":
                {
                    return Convert.FromBase64String(this.value.InnerText);
                }
                
                case "array":
                {
                    return null;
                }
                
                case "dict":
                {
                    return null;
                }
                
                default:
                    throw new NotImplementedException();
            }
        }
    }

    static public class PlistReader
    {
        static public void GetString(XmlNode plistNode)
        {
        }
    }
}
