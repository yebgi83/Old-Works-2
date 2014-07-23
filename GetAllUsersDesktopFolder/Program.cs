using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        /// <summary>
        /// 기존 바로 가기를 삭제한다.
        /// </summary>
        /// <returns></returns>
        static String GetAllUsersDesktopFolder()
        {
            IWshRuntimeLibrary.WshShellClass wshShell = new IWshRuntimeLibrary.WshShellClass();
            
            Object speacialFolderIndex = "AllUsersDesktop";
            Object folderPath = wshShell.SpecialFolders.Item(ref speacialFolderIndex);
            
            return folderPath as String;
        }
            
        static void Main(string[] args)
        {
            Console.WriteLine(GetAllUsersDesktopFolder());
            Console.ReadKey();
        }
    }
}
