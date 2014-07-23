using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace UpdateVersionInfo
{
    class Program
    {
        static void MarkAsDirty(String path)
        {
            try
            {
                VersionInfoResource.Structures.VS_VERSIONINFO pVerInfo;
                
                if (VersionInfoResource.LoadFromFile(path, out pVerInfo) == true)
                {
                    String szKey = "FileDescription";
                    
                    // 파일 정보에 추가하려는 항목이 이미 존재하는지 확인한다.
                    if (pVerInfo.StringFileInfo.Children.Children.ContainsKey(szKey) == true)
                    {
                        pVerInfo.StringFileInfo.Children.Children.Remove(szKey);
                    }
                    
                    // 새로운 항목을 추가한다.
                    VersionInfoResource.Structures.StringValue newStringValue = new VersionInfoResource.Structures.StringValue();
                    
                    newStringValue.szKey = szKey;
                    newStringValue.Value = Application.ProductVersion;
                    
                    pVerInfo.StringFileInfo.Children.Children.Add(szKey, newStringValue);
                    
                    // 추가한 항목을 파일에 적용한다.
                    VersionInfoResource.WriteToFile(path, pVerInfo);
                }
            }
            catch
            {
                return;
            }
        }
    
        static void Main(string[] args)
        {
            MarkAsDirty("D:\\UploadPhoto.exe");
        }
    }
}
    