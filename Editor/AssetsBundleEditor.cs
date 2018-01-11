/** 
 *Copyright(C) 201x by #John.W# 
 *All rights reserved. 
 *FileName:     #SCRIPTFULLNAME# 
 *Author:       #AUTHOR# 
 *Version:      #VERSION# 
 *UnityVersion：#UNITYVERSION# 
 *Date:         #DATE# 
 *Description:  A Simple Unity Muti-Game FrameWork  
 *History: 
*/  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetsBundleEditor:Editor{
    #region 打包指令
    [MenuItem("igeeknerd.com/Build Asset Bundle Demo")]
    public static void DoAssetBundle()
    {
        string p_path = Application.dataPath;
        //FileStream tmpstream = new FileStream(p_path + "/AssetBundle",FileMode.OpenOrCreate);
        Directory.CreateDirectory(p_path + "/AssetBundle");
        //Debug.Log("" + tmpstream.Name);
        //tmpstream.Close();
        BuildPipeline.BuildAssetBundles(p_path + "/AssetBundle", 0, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }

    #endregion
    [MenuItem("igeeknerd.com/Search Art Files")]
    public static void SearchArtFiles()
    {
        //删除无用的名字
        AssetDatabase.RemoveUnusedAssetBundleNames();
        //搜索文件
        string path = Application.dataPath + "/Art";
        //搜索文件
        DirectoryInfo dinfo = new DirectoryInfo(path);
        FileSystemInfo[] finfos = dinfo.GetFileSystemInfos();
        for (int i = 0; i < finfos.Length; i++) 
        {
            FileSystemInfo tmpfinfo = finfos[i];
            //过滤meta文件
            if (tmpfinfo.Name.IndexOf(".meta") > 0) continue;
            if (finfos[i].Name.IndexOf(".txt") > 0) continue;
            if (tmpfinfo is DirectoryInfo)
            {
                //Debug.Log(tmpfinfo.Name + " 是个文件夹" + "全名是 " + tmpfinfo.FullName);
                RecordPathTxt(tmpfinfo.Name);
                //处理第一级的文件夹
            }
            else
            {
                //Debug.Log(tmpfinfo.Name + "是文件");
            }
        }
    }

    public static void RecordPathTxt(string p_path)
    {
        string tmpstr = Application.dataPath+"/Art" +"/"+ p_path+"/"+  "Record.txt";
        FileStream tmpstream = new FileStream(tmpstr, FileMode.OpenOrCreate);
        StreamWriter tmpwriter = new StreamWriter(tmpstream);
        //tmpwriter.WriteLine(tmpstr);
        Dictionary<string, string> tmpDic = new Dictionary<string, string>();
        SearchSecondLayer(p_path, tmpDic);

        foreach (string ele in  tmpDic.Keys)
        {
            tmpwriter.WriteLine(ele + " " + tmpDic[ele]);
        }
        
        tmpwriter.Close();
        tmpstream.Close();
    }

    //搜索第二级文件夹，单独特殊处理
    public static void SearchSecondLayer(string p_path, Dictionary<string,string> p_dic)
    {
        string tmpstr = Application.dataPath + "/Art" + "/" + p_path;
        //Debug.Log("搜索的第二层是 " + tmpstr);
        //第一层是 p_path
        DirectoryInfo info1 = new DirectoryInfo(tmpstr);

        FileSystemInfo[] finfos = info1.GetFileSystemInfos();
        for (int i = 0; i < finfos.Length; i++)
        {
            if (finfos[i].Name.IndexOf(".meta") > 0) continue;
            if (finfos[i].Name.IndexOf(".txt") > 0) continue;
            if (finfos[i] is DirectoryInfo)
            {
                //Debug.Log("这里是第二层 " + p_path + "/" + finfos[i].Name + "模块包");
                ListAllFiles(finfos[i] as DirectoryInfo,p_path,finfos[i].Name);
                //记录第二层的模块名和文件路径（即是ab的名字）
                p_dic.Add(finfos[i].Name,p_path + "/" + finfos[i].Name);

            }
            else
            {
                //Debug.Log("第二层文件的名字是 " + finfos[i].Name);
            }
            //改变import
            
            ChangeFileName(finfos[i], p_path, finfos[i].Name);
        }

    }



    //如果第二层是文件夹递归列出所有文件
    public static void ListAllFiles(DirectoryInfo p_dinfo,string p_1stname,string p_2ndname)
    {
        FileSystemInfo[] finfos = p_dinfo.GetFileSystemInfos();
        for (int i=0;i < finfos.Length;i++)
        {
            if (finfos[i].Name.IndexOf(".meta") > 0) continue;
            if (finfos[i].Name.IndexOf(".txt") > 0) continue;
            if (finfos[i] is DirectoryInfo)
            {
                ListAllFiles(finfos[i] as DirectoryInfo,p_1stname,p_2ndname);
            }
            else
            {
                //Debug.Log(p_dinfo.Name + "遍历的文件的名字是 " + finfos[i].Name);
            }
            ChangeFileName(finfos[i], p_1stname, p_2ndname);
        }
    }

    public static void ChangeFileName(FileSystemInfo p_info,string p_1stname,string p_2ndname)
    {
        //import 必须从相对路径截取
        int curIndex = p_info.FullName.IndexOf("Asset");
        string tmp_path = p_info.FullName.Substring(curIndex);
        
        AssetImporter imp = AssetImporter.GetAtPath(tmp_path);
        //Debug.Log("文件全名" + p_info.FullName + "imp "+ imp);
        //return;
        imp.assetBundleName = p_1stname + "/" + p_2ndname;
        if (p_info.Extension == ".unity")
        {
            imp.assetBundleVariant = "u3d";
        }
        else
        {
            imp.assetBundleVariant = "ld";
        }
    }
}
