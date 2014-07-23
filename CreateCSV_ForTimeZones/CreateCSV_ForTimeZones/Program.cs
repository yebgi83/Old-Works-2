using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;

namespace CreateCSV_ForTimeZones
{
    class Program
    {
        static void Main(string[] args)
        {
            string outputPath = Path.Combine(Application.StartupPath, "TimeZones.csv");
            
            // 한글이 제대로 표시되도록 하기 위해서 euc-kr 인코딩으로 처리한다.
            StreamWriter streamWriter = new StreamWriter(outputPath, false, Encoding.GetEncoding("euc-kr"));
            
            try
            {
                // Write Headers.
                WriteValues(streamWriter, "Name", "Display", "Dlt", "Std");

                // Write Data
                WriteTimeZoneValues(streamWriter);

                // Output success message.                
                Console.WriteLine("Created.");
                Console.ReadKey();
            }
            finally
            {
                streamWriter.Close();
            }
        }

        static void WriteValues(StreamWriter streamWriter, params string[] values)
        {
            StringBuilder csvLineBuilder = new StringBuilder();

            for (int index = 0; index < values.Length; index++)
            {
                csvLineBuilder.Append('\"');
                csvLineBuilder.Append(values[index]);
                csvLineBuilder.Append('\"');

                if (index + 1 < values.Length)
                {
                    csvLineBuilder.Append(',');
                }
            }

            // 스트림에 생성한 줄을 추가한다.
            streamWriter.WriteLine(csvLineBuilder.ToString());
            
            // 추가한 줄이 완전히 처리될 때까지 기다린다.
            streamWriter.Flush();
            
            // 디버깅 (처리한 줄 표시)
            Console.WriteLine(csvLineBuilder.ToString());
        }

        static void WriteTimeZoneValues(StreamWriter streamWriter)
        {
            RegistryKey timeZoneRoot = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones");

            try
            {
                foreach (string subKeyName in timeZoneRoot.GetSubKeyNames())
                {
                    RegistryKey timeZoneSub = timeZoneRoot.OpenSubKey(subKeyName);

                    try
                    {
                        WriteValues
                        (
                            streamWriter,
                            subKeyName,
                            timeZoneSub.GetValue("Display") as string,
                            timeZoneSub.GetValue("Dlt") as string,
                            timeZoneSub.GetValue("Std") as string
                        );
                    }
                    finally
                    {
                        timeZoneSub.Close();
                    }
                }
            }
            finally
            {
                timeZoneRoot.Close();
            }
        }
    }
}
