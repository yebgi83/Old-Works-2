using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ParsingTest2
{
    static public class PDFParser
    {
        #region Tokens
        public enum PDFTokenType
        {
            Header = 1,
            EOF = 2,
            Trailer = 3,
            StartXRef = 4,
            XRef = 5,
            DictionaryHeader = 10,
            DictionaryTail = 11,
            ArrayHeader = 12,
            ArrayTail = 13,
            NameHeader = 14,
            Obj = 20,
            EndObj = 21,
            Stream = 30,
            EndStream = 31
        };
        
        static public Dictionary<PDFTokenType, String> tokens = new Dictionary<PDFTokenType, String> ()
        {
            {PDFTokenType.Header, "%PDF-"},
            {PDFTokenType.EOF, "%%EOF"},
            {PDFTokenType.Trailer, "trailer"},
            {PDFTokenType.StartXRef, "startxref"},
            {PDFTokenType.XRef, "xref"},
            {PDFTokenType.DictionaryHeader, "<<"},
            {PDFTokenType.DictionaryTail, ">>"},
            {PDFTokenType.ArrayHeader, "["},
            {PDFTokenType.ArrayTail, "]"},
            {PDFTokenType.NameHeader, "/"},
            {PDFTokenType.Obj, "obj"},
            {PDFTokenType.EndObj, "endobj"},
            {PDFTokenType.Stream, "stream"},
            {PDFTokenType.EndStream, "endstream"},
        };
        
        static public Boolean FindToken(Stream srcStream, PDFTokenType tokenType, Int64 startPosition, ref Int64 nextPosition)
        {
            Int32 positionIncrement = 0;
            
            if (srcStream.Position != startPosition)
            {
                srcStream.Seek
                (
                    startPosition - srcStream.Position, 
                    SeekOrigin.Current
                );
            }
        
            while (true)
            {
                Int32 srcByte = srcStream.ReadByte();
                
                if (srcByte == -1)
                {
                    nextPosition = startPosition;
                    return false;
                }
                
                Byte tokenByte = Convert.ToByte(tokens[tokenType][positionIncrement]);
                    
                if (srcByte == tokenByte)
                {
                    if (positionIncrement + 1 == tokens[tokenType].Length)
                    {
                        nextPosition = srcStream.Position;
                        return true;
                    }
                    else
                    {
                        positionIncrement++;
                    }
                }  
                else
                {
                    nextPosition = startPosition;
                    return false;
                }
            }
        }
        
        static public PDFTokenType? GetToken(Stream srcStream, Int64 startPosition, ref Int64 nextPosition)
        {
            if(srcStream.Position != startPosition)
            {
                srcStream.Seek
                (
                    startPosition - srcStream.Position, 
                    SeekOrigin.Current
                );
            }
        
            Int32 positionIncrement = 0;
            
            Dictionary<PDFTokenType, Boolean> sameToken = new Dictionary<PDFTokenType, Boolean> ();
            
            foreach(PDFTokenType tokenKey in tokens.Keys)
            {
                sameToken.Add(tokenKey, true);
            }
            
            while(true)
            {
                Int32 srcByte = srcStream.ReadByte();
                
                if (srcByte == -1)
                {
                    nextPosition = startPosition;
                    return null;
                }
                
                Boolean isByteMatched = false;
                
                foreach(KeyValuePair<PDFTokenType, String> token in tokens)
                {
                    if (sameToken[token.Key] == false)
                    {
                        continue;
                    }
                
                    if (positionIncrement >= token.Value.Length)
                    {
                        continue;
                    }
                    
                    Byte tokenByte = Convert.ToByte(token.Value[positionIncrement]);
                    
                    if (srcByte == tokenByte)
                    {
                        if (positionIncrement + 1 == token.Value.Length)
                        {
                            nextPosition = srcStream.Position;
                            return token.Key;
                        }
                        else
                        {
                            isByteMatched = true;
                            break;
                        }
                    }
                    else
                    {
                        sameToken[token.Key] = false;
                    }
                }  
                
                if (isByteMatched == true)
                {
                    positionIncrement++;
                }
                else
                {
                    nextPosition = startPosition;
                    return null;
                }
            }
        }
        
        static public Boolean SkipSpaceAndCrLf(Stream srcStream, Int64 startPosition, ref Int64 nextPosition)
        {
            if(srcStream.Position != startPosition)
            {
                srcStream.Seek
                (
                    startPosition - srcStream.Position, 
                    SeekOrigin.Current
                );
            }
            
            while (true)
            {
                Int32 srcByte = srcStream.ReadByte();
                
                if (srcByte == -1)
                {
                    nextPosition = srcStream.Position;
                    return false;
                }

                if (srcByte == ' ')
                {
                    continue;
                }
                
                if (srcByte == '\n')
                {
                    continue;
                }

                if (srcByte == '\r')
                {
                    continue;
                }
                
                srcStream.Seek(-1, SeekOrigin.Current);
                nextPosition = srcStream.Position;

                return true;
            }
        }
        
        static public Array GetArray(Stream srcStream, Int64 startPosition, ref Int64 nextPosition)
        {
            if (FindToken(srcStream, PDFTokenType.ArrayHeader, startPosition, ref nextPosition) == false)
            {
                nextPosition = startPosition;
                return null;
            }
            
            ArrayList     arrayValue = new ArrayList();
            StringBuilder itemValue  = new StringBuilder(); 
            
            while (true)
            {
                if (SkipSpaceAndCrLf(srcStream, nextPosition, ref nextPosition) == false)
                {
                    nextPosition = startPosition;
                    return null;
                }
                
                if (FindToken(srcStream, PDFTokenType.ArrayTail, nextPosition, ref nextPosition) == true)
                {
                    arrayValue.Add(itemValue.ToString());
                    return arrayValue.ToArray(); 
                }
               
                if (nextPosition != srcStream.Position)
                {
                    srcStream.Seek
                    (
                        nextPosition - srcStream.Position,
                        SeekOrigin.Current
                    );
                }                
                
                Int32 srcByte = srcStream.ReadByte();

                nextPosition = srcStream.Position;
                
                if (srcByte == -1)
                {
                    nextPosition = startPosition;
                    return null;
                }
                
                if (srcByte == '\n')
                {
                    nextPosition = startPosition;
                    return null;
                }
                
                if (srcByte == '\r')
                {
                    nextPosition = startPosition;
                    return null;
                }
                
                if (srcByte == ' ')
                {
                    arrayValue.Add(itemValue.ToString());
                    itemValue = new StringBuilder();
                    continue;
                }
                
                itemValue.Append(Convert.ToChar(srcByte));
            }
        }

        static public Byte[] GetBinary(Stream srcStream, Int64 startPosition, ref Int64 nextPosition)
        {
            if (FindToken(srcStream, PDFTokenType.Stream, startPosition, ref nextPosition) == false)
            {
                nextPosition = startPosition;
                return null;
            }
            
            MemoryStream value = new MemoryStream();
            
            while (true)
            {
                if (FindToken(srcStream, PDFTokenType.EndStream, nextPosition, ref nextPosition) == true)
                {
                    return value.ToArray(); 
                }
               
                if (nextPosition != srcStream.Position)
                {
                    srcStream.Seek
                    (
                        nextPosition - srcStream.Position,
                        SeekOrigin.Current
                    );
                }                
                
                Int32 srcByte = srcStream.ReadByte();

                nextPosition = srcStream.Position;
                
                if (srcByte == -1)
                {
                    nextPosition = startPosition;
                    return null;
                }
                
                value.WriteByte((Byte)srcByte);
            }
        }
        
        static public Dictionary<String, Object> GetDictionary(Stream srcStream, Int64 startPosition, ref Int64 nextPosition)
        {
            if (SkipSpaceAndCrLf(srcStream, startPosition, ref startPosition) == false)
            {
                nextPosition = startPosition;
                return null;
            }
        
            if (FindToken(srcStream, PDFTokenType.DictionaryHeader, startPosition, ref nextPosition) == false)
            {
                nextPosition = startPosition;
                return null;
            }
            
            Dictionary<String, Object> dicValue = new Dictionary<String, Object>();
        
            while (true)
            {
                String itemName = GetName(srcStream, nextPosition, ref nextPosition);
                
                if (itemName == null)
                {
                    if (FindToken(srcStream, PDFTokenType.DictionaryTail, nextPosition, ref nextPosition) == true)
                    {
                        return dicValue;
                    }
                    else
                    {
                        nextPosition = startPosition;
                        return null;
                    }
                }
                
                Object itemValue = GetValue(srcStream, nextPosition, ref nextPosition);
                
                if (itemValue == null)
                {
                    nextPosition = startPosition;
                    return null;
                }
                
                dicValue.Add (itemName, itemValue);
            }
        }

        static public String GetName(Stream srcStream, Int64 startPosition, ref Int64 nextPosition)
        {
            if (SkipSpaceAndCrLf(srcStream, startPosition, ref startPosition) == false)
            {
                nextPosition = startPosition;
                return null;
            }
        
            if (FindToken(srcStream, PDFTokenType.NameHeader, startPosition, ref nextPosition) == false)
            {
                nextPosition = startPosition;
                return null;
            }
            
            StringBuilder name = new StringBuilder();
            
            while (true)
            {
                PDFTokenType? tokenType = GetToken(srcStream, nextPosition, ref nextPosition);
            
                if (tokenType.HasValue == true)
                {
                    srcStream.Seek
                    (
                        tokens[tokenType.Value].Length * -1,
                        SeekOrigin.Current
                    );
                
                    nextPosition = srcStream.Position;
                    return name.ToString();
                }
                
                if (nextPosition != srcStream.Position)
                {
                    srcStream.Seek
                    (
                        nextPosition - srcStream.Position,
                        SeekOrigin.Current
                    );
                }
                
                Int32 srcByte = srcStream.ReadByte();
                
                nextPosition = srcStream.Position;
                
                if (srcByte == -1)
                {
                    return name.ToString();
                }
                
                if (srcByte == '\n')
                {
                    return name.ToString();
                }
                
                if (srcByte == '\r')
                {
                    return name.ToString();
                }
                
                if (srcByte == ' ')
                {
                    return name.ToString();
                }
                
                name.Append(Convert.ToChar(srcByte));
            }
        } 
         
        static public Object GetValue(Stream srcStream, Int64 startPosition, ref Int64 nextPosition)
        {
            if (SkipSpaceAndCrLf(srcStream, startPosition, ref startPosition) == false)
            {
                nextPosition = startPosition;
                return null;
            }
        
            Object value = value = GetDictionary(srcStream, startPosition, ref nextPosition);
            
            if (value != null)
            {
                return value;
            }
            
            value = GetArray(srcStream, startPosition, ref nextPosition);
            
            if (value != null)
            {
                return value;
            }
            
            value = new StringBuilder();
            
            while (true)
            {
                PDFTokenType? tokenType = GetToken(srcStream, nextPosition, ref nextPosition);
            
                if (tokenType.HasValue == true)
                {
                    srcStream.Seek
                    (
                        tokens[tokenType.Value].Length * -1,
                        SeekOrigin.Current
                    );
                
                    nextPosition = srcStream.Position;
                    return value.ToString();
                }
                
                if (nextPosition != srcStream.Position)
                {
                    srcStream.Seek
                    (
                        nextPosition - srcStream.Position,
                        SeekOrigin.Current
                    );
                }                
                
                Int32 srcByte = srcStream.ReadByte();
                
                nextPosition = srcStream.Position;
                
                if (srcByte == -1)
                {
                    return value.ToString();
                }
                
                if (srcByte == '\n')
                {
                    return value.ToString();
                }
                
                if (srcByte == '\r')
                {
                    return value.ToString();
                }
                
                (value as StringBuilder).Append(Convert.ToChar(srcByte));
            }
        }
        #endregion
        
        #region Trailer
        static public Boolean FindTrailer(Stream srcStream, ref Int64 nextPosition)
        {
            for (Int32 startPosition = (Int32)srcStream.Length; startPosition > srcStream.Length - 256; startPosition--)
            {
                if (FindToken(srcStream, PDFTokenType.Trailer, startPosition, ref nextPosition) == true)
                {
                    return true;
                }
            }

            return false;
        }        
        
        static public Boolean FindTrailer(Stream srcStream, Int64 startPosition, ref Int64 nextPosition)
        {
            if (PDFParser.SkipSpaceAndCrLf(srcStream, startPosition, ref startPosition) == false)
            {
                return false;
            }        
        
            return FindToken(srcStream, PDFTokenType.Trailer, startPosition, ref nextPosition);
        }
        #endregion
        
        #region XRef
        public struct XRefData
        {
            public Object obj;
            public Byte[] binary;
            
            public XRefData(Object obj, Byte[] stream)
            {
                this.obj    = obj;
                this.binary = stream;
            }
        }
        
        public struct XRefObject
        {
            public Int64     offset;
            public Int32     generation;
            public Char      flag;
            public XRefData? data;
            
            public XRefObject(Int64 offset, Int32 generation, Char flag)
            {
                this.offset     = offset;
                this.generation = generation;
                this.flag       = flag; 
                this.data       = null;
            }
        }
        
        static public Int64? GetStartXRef(Stream srcStream, Int64 startPosition, ref Int64 nextPosition)
        {
            if (SkipSpaceAndCrLf(srcStream, startPosition, ref startPosition) == false)
            {
                return null;
            }
        
            if (FindToken(srcStream, PDFTokenType.StartXRef, startPosition, ref nextPosition) == false)
            {
                nextPosition = startPosition;
                return null;
            }
            
            return Convert.ToInt64(GetValue(srcStream, nextPosition, ref nextPosition));
        }
        
        static public Dictionary<Int32, XRefObject> GetXRefTable(Stream srcStream, Int64 startPosition, ref Int64 nextPosition)
        {
            if (SkipSpaceAndCrLf(srcStream, startPosition, ref startPosition) == false)
            {
                return null;
            }
        
            if (FindToken(srcStream, PDFTokenType.XRef, startPosition, ref nextPosition) == false)
            {
                nextPosition = startPosition;
                return null;
            }
            
            Char[] splitter = new Char[] {' '};
            
            String   xrefHeader = GetValue(srcStream, nextPosition, ref nextPosition) as String;
            String[] xrefHeaderArgs = xrefHeader.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            
            if (xrefHeaderArgs.Length != 2)
            {
                nextPosition = startPosition;
                return null;
            }
             
            Dictionary<Int32, XRefObject> xrefTable = new Dictionary<Int32, XRefObject>();
            
            Int32 startIndex = Int32.Parse(xrefHeaderArgs[0]);
            Int32 endIndex = startIndex + Int32.Parse(xrefHeaderArgs[1]) - 1;
            
            for (Int32 index = startIndex; index <= endIndex; index++)
            {
                String   xrefInfo = GetValue(srcStream, nextPosition, ref nextPosition) as String;
                String[] xrefInfoArgs = xrefInfo.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                
                if (xrefInfoArgs.Length != 3)
                {
                    nextPosition = startPosition;
                    return null;
                }               
                
                XRefObject xrefObject = new XRefObject
                (
                    Convert.ToInt64(xrefInfoArgs[0]),
                    Convert.ToInt32(xrefInfoArgs[1]),
                    xrefInfoArgs[2][0]
                );

                xrefObject.data = GetXRefData (srcStream, xrefObject);
                xrefTable.Add(index, xrefObject);
            }
            
            return xrefTable;
        }
        #endregion
        
        #region Object 
        static public XRefData? GetXRefData (Stream srcStream, XRefObject xrefObject)
        {
            Int64 startPosition = xrefObject.offset;
            Int64 nextPosition = default(Int64);

            Char[] splitter = new Char[] {' '};

            String   objectHeader = GetValue(srcStream, startPosition, ref nextPosition) as String;
            String[] objectHeaderArgs = objectHeader.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

            if (objectHeaderArgs.Length != 2)
            {
                return null;
            }
            
            if (FindToken(srcStream, PDFTokenType.Obj, nextPosition, ref nextPosition) == false)
            {
                return null;
            }
            
            Object obj = GetDictionary (srcStream, nextPosition, ref nextPosition);
            
            if (obj == null)
            {
                obj = GetValue (srcStream, nextPosition, ref nextPosition);
            
                if(obj == null)
                {
                    return null;
                }
            }
            
            if (SkipSpaceAndCrLf(srcStream, nextPosition, ref nextPosition) == false)
            {
                return null;
            }
            
            Byte[] stream = GetBinary (srcStream, nextPosition, ref nextPosition);
            
            if (FindToken(srcStream, PDFTokenType.EndObj, nextPosition, ref nextPosition) == false)
            {
                return null;
            }
            
            return new XRefData(obj, stream);
        }
        #endregion
    }
}