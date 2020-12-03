import sys
import xlrd
import re
import google
import os
sys.path.append("../proto-table")

def INFO(msg):
    print "[INFO] " + str(msg)
def CheckAllProtoFile3(file1,file2):
    INFO(file1+" "+file2)
def CheckAllProtoFile2(file1,path2):
    if os.path.exists(path1) and os.path.exists(path2):
        for root, dirs, files in os.walk(path1):
            for file in files:
                if ".xls" in file and file == file1:
                    CheckAllProtoFile3(file1,file)
def CheckAllProtoFile(path1,path2):
    if os.path.exists(path1) and os.path.exists(path2):
        for root, dirs, files in os.walk(path1):
            for file in files:
                if ".xls" in file:
                    CheckAllProtoFile2(file,path2)

if __name__ == "__main__":
    print "Hello world!"
    argvs = sys.argv[1:]
    alen = len(argvs)
    path1 = None;
    path2 = None;
    if alen == 2:
        path1 = argvs[0]
        path2 = argvs[1]

    INFO(path1+" "+path2);
    CheckAllProtoFile(path1,path2);

    exit(0)