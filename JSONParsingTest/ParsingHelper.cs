using System;
using System.Text;

namespace JJBJ.Helper
{
    static internal class ParsingHelper 
    {
        static internal bool SkipSpace(int startPosition, string text, ref int nextPosition)
        {
            int position = startPosition;
            
            while (position < text.Length)
            {
                if (text[position] == ' ')
                {
                    position++;
                }
                else
                {
                    break;
                }
            }
            
            nextPosition = position;
                           
            if (position < text.Length)
            {
                return true; 
            }
            else
            {
                return false;
            }
        }
        
        static internal decimal? GetDecimal(int startPosition, string text, ref int nextPosition)
        {
            int position = startPosition;
            
            StringBuilder value  = new StringBuilder();

            if (text[position] == '-') 
            {
                value.Append('-');
                position++;
            }
            
            if (Char.IsDigit(text[position]) == true)
            {
                if (text[position] == '0')
                {
                    value.Append('0');
                    position++;
                }
                else
                {
                    value.Append(text[position]);
                    position++;
                    
                    while (position < text.Length)
                    {
                        if (Char.IsDigit(text[position]) == true)
                        {
                            value.Append(text[position]);
                            position++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                
                if (position < text.Length)
                {
                    if (text[position] == '.')
                    {
                        value.Append('.');
                        position++;
                    
                        while (position < text.Length)
                        {
                            if (Char.IsDigit(text[position]) == true)
                            {
                                value.Append(text[position]);
                                position++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        
                        nextPosition = position;
                        return Convert.ToDecimal(value);
                    }
                    else
                    {
                        nextPosition = startPosition;
                        return null;
                    }
                }
                else
                {
                    nextPosition = position;
                    return Convert.ToDecimal(value);
                }
            }            
            
            nextPosition = startPosition;
            return null;
        }
    }
}
