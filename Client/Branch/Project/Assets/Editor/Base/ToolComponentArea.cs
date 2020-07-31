using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ToolComponentArea
{
    public int columns;//每行个个数
    public float itemMarginH;
    public float itemMarginV;
    public float ItemWidth;
    public int column;
    public float LineHeight;

    bool IsLast = true;

    public int CurPage = 1;

    public int index = 0;
    public int line = 0;
    private Vector2 ScrollPos = new Vector2(); 

    public ToolComponentArea(float ItemWidth, float itemMarginV, float itemMarginH)
    {
        this.ItemWidth = ItemWidth;
        this.itemMarginV = itemMarginV;
        this.itemMarginH = itemMarginH;
    }

    public void UpdateCount(float ItemWidth, float itemMargin, float lineHeight)
    {
        this.ItemWidth = ItemWidth;
        this.itemMarginV = itemMargin;
        this.itemMarginH = lineHeight;
    }

    public int PageCount = 1;
    public int PageItemCount = 100;
    public int ScreenWidth;
    bool isScrollView = false;
    public bool Begin(int ScreenWidth, int ScreenHeight = 0, int LineHeight = 0, int PageItemCount = 0, int PageCount = 1, bool isScrollView = true)
    {
        this.isScrollView = isScrollView;
        column = 0;
        columns = Mathf.FloorToInt(ScreenWidth / (ItemWidth + itemMarginV * 2));
        index = 0;
        line = 0;
        IsLast = true;
        this.PageCount = PageCount;
        this.ScreenWidth = ScreenWidth;
        this.PageItemCount = PageItemCount;
        this.LineHeight = LineHeight;
        if (ScreenWidth == 0) return false;
        ShowPageIndexHeader();
        if(this.isScrollView)
            ScrollPos = GUILayout.BeginScrollView(ScrollPos);
        if (columns == 0) return false;

        return true;
    }

    void ShowPageIndexHeader()
    {
        int BtnPageColums = Mathf.FloorToInt(ScreenWidth / (GUIWindow.BtnShortSelectedStyle.fixedWidth + 3 * 2));
        try
        {
            if (PageCount > 1 && GUIWindow.DrawHeader("目录"))
            {
                for (int i = 0; i < PageCount; i++)
                {
                    int column = i % BtnPageColums;
                    if (column == 0)
                    {
                        GUILayout.BeginHorizontal();
                    }

                    if (CurPage == i + 1)
                    {
                        if (GUILayout.Button((i + 1).ToString(), GUIWindow.BtnShortSelectedStyle))
                            CurPage = i + 1;
                    }
                    else
                    {
                        if (GUILayout.Button((i + 1).ToString(), GUIWindow.BtnShortNormalStyle))
                            CurPage = i + 1;
                    }


                    if (column == BtnPageColums - 1 || i == PageCount - 1)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.Space(itemMarginH);
                    }
                }
            }
            GUIWindow.DrawSeparator();
        }
        catch { }
       
    }
    public void Show(System.Action ShowAction)
    {
        column = index % columns;

        if (column == 0)
        {
            if (LineHeight != 0)
                GUILayout.BeginHorizontal(GUILayout.Height(LineHeight));
            else
                GUILayout.BeginHorizontal();
            GUILayout.Space(itemMarginV);
        }

        IsLast = false;

        if (ShowAction != null)
            ShowAction();

        GUILayout.Space(itemMarginV);

        if (column == columns - 1)
        {
            IsLast = true;
            GUILayout.EndHorizontal();
            GUILayout.Space(itemMarginH);
            line++;
        }
        index++;
    }

    public void DrawNewLine()
    {
        if(!IsLast)
        {
            GUILayout.EndHorizontal();
            GUILayout.Space(itemMarginH);
        }
        else
        {
            GUILayout.Space(itemMarginH);
        }
        index = (Mathf.CeilToInt(index * 1f / columns))* columns;
    }

    public void End()
    {
        if (!IsLast)
        {
            GUILayout.EndHorizontal();
            GUILayout.Space(itemMarginH);
        }

        if (this.isScrollView)
            GUILayout.EndScrollView();
    }
}