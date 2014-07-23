using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace ParsingTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            using(FileStream srcStream = File.Open("test2.pdf", FileMode.Open))
            {
                try
                {
                    Int64 nextPosition = default(Int64);
                    
                    if (PDFParser.FindTrailer(srcStream, ref nextPosition) == false)
                    {
                        return;
                    }
                    
                    if (PDFParser.GetDictionary(srcStream, nextPosition, ref nextPosition) == null)
                    {
                        return;
                    }
                    
                    Int64? startXRef = PDFParser.GetStartXRef(srcStream, nextPosition, ref nextPosition);
                        
                    if (startXRef.HasValue == false)
                    {
                        return;
                    }
                    
                    Dictionary<Int32, PDFParser.XRefObject> xrefObjectTable = PDFParser.GetXRefTable(srcStream, startXRef.Value, ref nextPosition);
                }
                finally
                {
                    Console.ReadKey();
                }
            }
        }
    }
}
