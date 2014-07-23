using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace JJBJ.JSON
{
    static internal class JSONReader
    {
        static private Dictionary<string, object> GetObject(int startPosition, string text, ref int nextPosition)
        {
            return null;
        }
    
        static private object[] GetArray(int startPosition, string text, char findToken)
        {
            return null;
        }
        
        static public string GetString(int startPosition, string text, ref int nextPosition)
        {
            int position = startPosition;
            
            StringBuilder value  = new StringBuilder();
            
            if (text[position] == '\"')
            {
                position++;
            }
            else
            {
                nextPosition = startPosition;
                return null;
            }
            
            while (position < text.Length)
            {
                if (text[position] == '\\')
                {
                    position++;
                    
                    if (!(position < text.Length)) 
                    {
                        nextPosition = startPosition;
                        return null;
                    }
                    
                    switch (text[position])
                    {
                        case '\"':
                        case '\\':
                        case '/':
                        {
                            position++;
                            value.Append(text[position]);
                        }
                        break;
                            
                        case 'b':
                        {
                            position++;
                            value.Append('\b');
                        }
                        break;
                        
                        case 'f':
                        {
                            position++;
                            value.Append('\f');
                        }
                        break;
                        
                        case 'n':
                        {
                            position++;
                            value.Append('\n');
                        }
                        break;
                                                
                        case 'r':
                        {
                            position++;
                            value.Append('\r');
                        }
                        break;
                                                
                        case 't':
                        {
                            position++;
                            value.Append('\t');
                        }
                        break;
                        
                        case 'u':
                        {
                            throw new NotSupportedException("\\u is not supported.");
                        }
                    }
                }
                
                if (text[position] == '\"')
                {
                    break;                
                }

                value.Append(text[position]);
                position++;
            }
            
            nextPosition = position + 1;
            return value.ToString();
        }
    }
}