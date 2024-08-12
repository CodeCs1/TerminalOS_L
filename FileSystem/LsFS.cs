using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.HAL;
//This will use something called: Binary Tree

/*
    For thing to be simple, this LsFS uses Binary Tree in saving and loading data
    Structure:
        Folder_t -> binary
        File_t -> Normal

    Tree Structure: (take C as drive letter)
            (C:) <- Root Folder
          /      \
         usr     home (limit: 2 users)
        /   \      \
       bin  local   user1
      /   \          /   \
    alias csh     alias  Loader.cfg

    How about this:
    Still use tree but instead, doing list folder.
    (C): _ abcxyz
            +- abcxyz2

*/

namespace TerminalOS_L {
    public class Folder {
        public struct Time_Creation {
                public int Year;
                public int Month;
                public int Day;
                public int Hours;
                public int Minute;
                public int Second;
            }
        public struct File {
            public string filename;
            public Byte[] content;
            public Time_Creation time;
        }
        public string Folder_Name1;
        public Folder[] folder;
        public File[] file;
        public Time_Creation time;
        public bool is_root_folder;
        public int GetFileCount;
        public Folder(string Folder_Name, bool rootfolder) {
            Folder_Name1 = Folder_Name;
            is_root_folder = rootfolder;
            time.Day = RTC.DayOfTheWeek;
            time.Month = RTC.DayOfTheMonth;
            time.Year = RTC.Year;
            time.Hours = RTC.Hour;
            time.Minute = RTC.Minute;
            time.Second = RTC.Second;
            file = new File[1024]; // TODO: Fix the limitation
            GetFileCount=0;
        }
        public Folder() {}
    }


    public class FileIO {
        Folder f;
        public  FileIO() {
            f = new Folder("0",true);
            _ = f.folder.Append(f);
        }
        public FileIO(string c) {
            f = new Folder(c,true);
            _ = f.folder.Append(f);
        }
        public string ListAll() { /* Path: C:/test.txt */
            foreach(Folder child in f.folder) {
                Console.WriteLine("Got: {0}",child.Folder_Name1);
                foreach (Folder.File file in child.file) {
                    Console.WriteLine("Got: {0}",file.filename);
                }
                Console.ReadLine();
            }
            return "";
        }
        public void CreateFile(string path) {
            string[] pathr = path.Split('/');
            List<string> pathspl = new();
            foreach(string path_child in pathspl) {
                pathspl.Add(path_child);
            }
            List<string> RealPath = pathspl.GetRange(0,pathspl.Count-1);
            foreach(string path_child in RealPath) {
                
            }
        }
        public void WriteFileByString(string path, string Text) {

        }
        public bool Exist(string path){
            return true;
        }
        public string ReadFile(string path) {
            return path;
        }

    }

    public class ListFileSystem {


        public static char[] DriveLabel=new char[1024];
        private readonly char drivepath; // It can be anything, e.g: C, 0, 1,...
        private readonly UInt64 sizeofdrive; // Limition for tree (< 1 TB)
        //sizeofdrive: Use B as size.
        private readonly Int64 totaldrive=0;
        public ListFileSystem(char drivepath,UInt64 sizeofdrive) {
            if (Char.IsLower(drivepath)) drivepath = Char.ToUpper(drivepath);
            Message.Send($"Initializing List File System with {drivepath} Label...");

            if (!DriveLabel.Contains(drivepath)) {
                DriveLabel[totaldrive] = drivepath;
                totaldrive++;
            } else {
                Message.Send_Error("Failed to Initialize File System (Drive Label Exist)");
                return;
            }
            if (sizeofdrive >= 1099511627776) {
                Message.Send_Error("The Init drive can't be equal or greater than 1 TB");
                return;
            } else if (sizeofdrive < 1) {
                Message.Send_Error("The Init drive can't be smaller than 1 B");
                return;
            }
            this.drivepath = drivepath;
            this.sizeofdrive = sizeofdrive;
        }
    }
}