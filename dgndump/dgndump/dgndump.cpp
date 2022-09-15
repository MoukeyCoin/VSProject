// dgndump.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//
#pragma warning(disable : 4996)
#include <iostream>
#include "dgnlibp.h"
#include <string>
#include <dgnlib.h>
#include <direct.h>
#include <windows.h>

static void myDGNDumpElement(DGNHandle hDGN, DGNElemCore* psElement, FILE* fp)

{
    if (psElement->stype == DGNST_CELL_HEADER || psElement->stype == DGNST_COMPLEX_HEADER || psElement->stype == DGNST_BSPLINE_SURFACE_HEADER ||
        psElement->stype == DGNST_BSPLINE_CURVE_HEADER || psElement->stype == DGNST_MULTIPOINT)
    {
    }
    else
        return;
   
    DGNInfo* psInfo = (DGNInfo*)hDGN;

    fprintf(fp, "\n");
    fprintf(fp, "Element:%-12s Level:%2d id:%-6d ",
        DGNTypeToName(psElement->type),
        psElement->level,
        psElement->element_id);

    if (psElement->complex)
        fprintf(fp, "(Complex) ");

    if (psElement->deleted)
        fprintf(fp, "(DELETED) ");

    fprintf(fp, "\n");

    fprintf(fp, "  offset=%d  size=%d bytes\n",
        psElement->offset, psElement->size);

    fprintf(fp,
        "  graphic_group:%-3d color:%d weight:%d style:%d\n",
        psElement->graphic_group,
        psElement->color,
        psElement->weight,
        psElement->style);

    if (psElement->properties != 0)
    {
        int     nClass;

        fprintf(fp, "  properties=%d", psElement->properties);
        if (psElement->properties & DGNPF_HOLE)
            fprintf(fp, ",HOLE");
        if (psElement->properties & DGNPF_SNAPPABLE)
            fprintf(fp, ",SNAPPABLE");
        if (psElement->properties & DGNPF_PLANAR)
            fprintf(fp, ",PLANAR");
        if (psElement->properties & DGNPF_ORIENTATION)
            fprintf(fp, ",ORIENTATION");
        if (psElement->properties & DGNPF_ATTRIBUTES)
            fprintf(fp, ",ATTRIBUTES");
        if (psElement->properties & DGNPF_MODIFIED)
            fprintf(fp, ",MODIFIED");
        if (psElement->properties & DGNPF_NEW)
            fprintf(fp, ",NEW");
        if (psElement->properties & DGNPF_LOCKED)
            fprintf(fp, ",LOCKED");

        nClass = psElement->properties & DGNPF_CLASS;
        if (nClass == DGNC_PATTERN_COMPONENT)
            fprintf(fp, ",PATTERN_COMPONENT");
        else if (nClass == DGNC_CONSTRUCTION_ELEMENT)
            fprintf(fp, ",CONSTRUCTION ELEMENT");
        else if (nClass == DGNC_DIMENSION_ELEMENT)
            fprintf(fp, ",DIMENSION ELEMENT");
        else if (nClass == DGNC_PRIMARY_RULE_ELEMENT)
            fprintf(fp, ",PRIMARY RULE ELEMENT");
        else if (nClass == DGNC_LINEAR_PATTERNED_ELEMENT)
            fprintf(fp, ",LINEAR PATTERNED ELEMENT");
        else if (nClass == DGNC_CONSTRUCTION_RULE_ELEMENT)
            fprintf(fp, ",CONSTRUCTION_RULE_ELEMENT");

        fprintf(fp, "\n");
    }

    switch (psElement->stype)
    {
    case DGNST_MULTIPOINT:
    {
        DGNElemMultiPoint* psLine = (DGNElemMultiPoint*)psElement;
        int                   i;

        for (i = 0; i < psLine->num_vertices; i++)
            fprintf(fp, "  (%.6f,%.6f,%.6f)\n",
                psLine->vertices[i].x,
                psLine->vertices[i].y,
                psLine->vertices[i].z);
    }
    break;


    case DGNST_CELL_HEADER:
    {
        DGNElemCellHeader* psCell = (DGNElemCellHeader*)psElement;
        fprintf(fp, "  totlength=%d, name=%s, class=%x, levels=%02x%02x%02x%02x\n",
            psCell->totlength, psCell->name, psCell->cclass,
            psCell->levels[0], psCell->levels[1], psCell->levels[2],
            psCell->levels[3]);
        fprintf(fp, "  rnglow=(%.5f,%.5f,%.5f)\n"
            "  rnghigh=(%.5f,%.5f,%.5f)\n",
            psCell->rnglow.x, psCell->rnglow.y, psCell->rnglow.z,
            psCell->rnghigh.x, psCell->rnghigh.y, psCell->rnghigh.z);
        fprintf(fp, "  origin=(%.5f,%.5f,%.5f)\n",
            psCell->origin.x, psCell->origin.y, psCell->origin.z);

        if (psInfo->dimension == 2)
            fprintf(fp, "  xscale=%g, yscale=%g, rotation=%g\n",
                psCell->xscale, psCell->yscale, psCell->rotation);
        else
            fprintf(fp, "  trans=%g,%g,%g,%g,%g,%g,%g,%g,%g\n",
                psCell->trans[0],
                psCell->trans[1],
                psCell->trans[2],
                psCell->trans[3],
                psCell->trans[4],
                psCell->trans[5],
                psCell->trans[6],
                psCell->trans[7],
                psCell->trans[8]);

    }
    break;


    case DGNST_SHARED_CELL_DEFN:
    {
        DGNElemSharedCellDefn* psShared = (DGNElemSharedCellDefn*)psElement;

        fprintf(fp, "  totlength=%d\n", psShared->totlength);
    }
    break;

    case DGNST_COMPLEX_HEADER:
    {
        DGNElemComplexHeader* psHdr = (DGNElemComplexHeader*)psElement;

        fprintf(fp,
            "  totlength=%d, numelems=%d\n",
            psHdr->totlength,
            psHdr->numelems);
        if (psElement->type == DGNT_3DSOLID_HEADER ||
            psElement->type == DGNT_3DSURFACE_HEADER) {
            fprintf(fp,
                "  surftype=%d, boundelms=%d\n",
                psHdr->surftype, psHdr->boundelms);
        }
    }
    break;


    case DGNST_BSPLINE_CURVE_HEADER:
    {
        DGNElemBSplineCurveHeader* psSpline =
            (DGNElemBSplineCurveHeader*)psElement;

        fprintf(fp,
            "  desc_words=%ld, curve type=%d\n"
            "  properties=%02x",
            psSpline->desc_words, psSpline->curve_type,
            psSpline->properties);
        if (psSpline->properties != 0) {
            if (psSpline->properties & DGNBSC_CURVE_DISPLAY) {
                fprintf(fp, ",CURVE_DISPLAY");
            }
            if (psSpline->properties & DGNBSC_POLY_DISPLAY) {
                fprintf(fp, ",POLY_DISPLAY");
            }
            if (psSpline->properties & DGNBSC_RATIONAL) {
                fprintf(fp, ",RATIONAL");
            }
            if (psSpline->properties & DGNBSC_CLOSED) {
                fprintf(fp, ",CLOSED");
            }
        }
        fprintf(fp, "\n");
        fprintf(fp, "  order=%d\n  %d poles, %d knots\n",
            psSpline->order, psSpline->num_poles, psSpline->num_knots);
    }
    break;

    

    default:
        break;
    }

    if (psElement->attr_bytes > 0)
    {
        int iLink;

        fprintf(fp, "Attributes (%d bytes):\n", psElement->attr_bytes);

        for (iLink = 0; TRUE; iLink++)

        {
            int nLinkType, nEntityNum = 0, nMSLink = 0, nLinkSize, i;
            unsigned char* pabyData;

            pabyData = DGNGetLinkage(hDGN, psElement, iLink, &nLinkType,
                &nEntityNum, &nMSLink, &nLinkSize);
            if (pabyData == NULL)
                break;

            fprintf(fp, "Type=0x%04x", nLinkType);
            if (nMSLink != 0 || nEntityNum != 0)
                fprintf(fp, ", EntityNum=%d, MSLink=%d",
                    nEntityNum, nMSLink);
            fprintf(fp, "\n  0x");

            for (i = 0; i < nLinkSize; i++)
                fprintf(fp, "%02x", pabyData[i]);
            fprintf(fp, "\n");

        }
    }
}
static void Usage()

{
    printf("Usage: dgndump [-e xmin ymin xmax ymax] [-s] [-r n] filename.dgn\n");
    printf("\n");
    printf("  -e xmin ymin xmax ymax: only get elements within extents.\n");
    printf("  -s: produce summary report of element types and levels.\n");
    printf("  -r n: report raw binary contents of elements of type n.\n");

    exit(1);
}


static void DGNDumpRawElement(DGNHandle hDGN, DGNElemCore* psCore,
    FILE* fpOut)

{
    int         i, iChar = 0;
    char        szLine[80];

    fprintf(fpOut, "  Raw Data (%d bytes):\n", psCore->raw_bytes);
    for (i = 0; i < psCore->raw_bytes; i++)
    {
        char    szHex[3];

        if ((i % 16) == 0)
        {
            sprintf_s(szLine, "%6d: %71s", i, " ");
            iChar = 0;
        }

        sprintf_s(szHex, "%02x", psCore->raw_data[i]);
        //strncpy_s(szLine + 8 + iChar * 2, szHex, 2);

        if (psCore->raw_data[i] < 32 || psCore->raw_data[i] > 127)
            szLine[42 + iChar] = '.';
        else
            szLine[42 + iChar] = psCore->raw_data[i];

        if (i == psCore->raw_bytes - 1 || (i + 1) % 16 == 0)
        {
            fprintf(fpOut, "%s\n", szLine);
        }

        iChar++;
    }
}

int main(int argc, char** argv)
{
    //std::cout << "Hello World!\n";
    DGNHandle   hDGN;
    DGNElemCore* psElement;
    const char* pszFilename = NULL;
    //输出文件的路径
    FILE* outputPath = NULL;
    FILE* PointsPath = NULL;
    int         bSummary = FALSE, iArg, bRaw = FALSE, bReportExtents = FALSE,output = FALSE;
    char        achRaw[128];
    double      dfSFXMin = 0.0, dfSFXMax = 0.0, dfSFYMin = 0.0, dfSFYMax = 0.0;

    memset(achRaw, 0, 128);
    //pszFilename = "d:\\R43-R540-008.dgn";

    std::string path = "";

    for (iArg = 1; iArg < argc; iArg++)
    {
        if (strcmp(argv[iArg], "-s") == 0)
        {
            bSummary = TRUE;
        }
        else if (strcmp(argv[iArg], "-e") == 0 && iArg < argc - 4)
        {
            dfSFXMin = atof(argv[iArg + 1]);
            dfSFYMin = atof(argv[iArg + 2]);
            dfSFXMax = atof(argv[iArg + 3]);
            dfSFYMax = atof(argv[iArg + 4]);
            iArg += 4;
        }
        else if (strcmp(argv[iArg], "-r") == 0 && iArg < argc - 1)
        {
            achRaw[MAX(0, MIN(127, atoi(argv[iArg + 1])))] = 1;
            bRaw = TRUE;
            iArg++;
        }
        else if (strcmp(argv[iArg], "-extents") == 0)
        {
            bReportExtents = TRUE;
        }
        else if (strcmp(argv[iArg], "-o") == 0 && iArg < argc - 1)
        {
            char* pt = argv[iArg + 1];
            output = TRUE;
            path = pt;

            iArg += 1;
        }
        else if (argv[iArg][0] == '-' || pszFilename != NULL)
            Usage();

       
        else
            pszFilename = argv[iArg];
    }
 
    if (pszFilename == NULL)
        Usage();
    if (output)
    {
        time_t now = time(NULL);
        tm* time = localtime(&now);
        // time->tm_year = time->tm_year + 1900;
         //time->tm_mon = time->tm_mon + 1;
        char timestr[25];
        strftime(timestr, 25, "%Y-%m-%d-%H-%M-%S", time);
        std::string filename = pszFilename;
        std::string suppname = filename.replace(filename.find(".dgn"), 4, "").substr(filename.find_last_of("\\") + 1);
        std::string tm = timestr;
        path = path + "\\" + suppname + tm;
        mkdir(path.c_str());
        std::string reportpath = path + "\\DGNReport_" + suppname  +".txt";
        outputPath = fopen(reportpath.c_str(), "w");
        std::string ppath = path + "\\DGNPoints.txt";
        PointsPath = fopen(ppath.c_str(), "w");
                                  
    }
    hDGN = DGNOpen(pszFilename, FALSE);
    if (hDGN == NULL)
        exit(1);

    if (bRaw)
        DGNSetOptions(hDGN, DGNO_CAPTURE_RAW_DATA);

    DGNSetSpatialFilter(hDGN, dfSFXMin, dfSFYMin, dfSFXMax, dfSFYMax);
    int         firstCell = 0;
    if (!bSummary)
    {
        int shape = 0;
        while ((psElement = DGNReadElement(hDGN)) != NULL)
        {
            
            if (output)
            {
                if (psElement->stype == DGNST_CELL_HEADER)
                {
                    firstCell ++;
                    fprintf(outputPath, "********************************\n");
                    fprintf(outputPath, " *** Cell ID:%i\n", firstCell);
                    fprintf(outputPath, "********************************\n");
                    shape = 0;
                   
                        
                    
                }
                //提取shape
                if (psElement->type == DGNT_SHAPE && firstCell > 0)
                {
                    shape++;

                    DGNElemMultiPoint* psLine = (DGNElemMultiPoint*)psElement;
                    int                   i;

                    for (i = 0; i < psLine->num_vertices; i++)
                    {
                        fprintf(PointsPath, "1-%i-%i-shape%i:", firstCell, firstCell, shape);
                        fprintf(PointsPath, "  (%.6f,%.6f,%.6f)\n",
                            psLine->vertices[i].x,
                            psLine->vertices[i].y,
                            psLine->vertices[i].z);
                    }
                        
                }
                if(firstCell > 0)
                myDGNDumpElement(hDGN, psElement, outputPath);
            }
            else
            {
                
                if (psElement->stype == DGNST_CELL_HEADER)
                {
                    firstCell++;
                    fprintf(stdout, "********************************\n");
                    fprintf(stdout, " *** Cell ID:%i\n", firstCell);
                    fprintf(stdout, "********************************\n");                  
                    shape = 0;
                }
                
                //提取shape
                if (psElement->type == DGNT_SHAPE && firstCell > 0)
                {
                    shape++;

                    DGNElemMultiPoint* psLine = (DGNElemMultiPoint*)psElement;
                    int                   i;

                    for (i = 0; i < psLine->num_vertices; i++)
                    {
                        fprintf(stdout, "1-%i-%i-shape%i:", firstCell, firstCell, shape);
                        fprintf(stdout, "  (%.6f,%.6f,%.6f)\n",
                            psLine->vertices[i].x,
                            psLine->vertices[i].y,
                            psLine->vertices[i].z);
                    }
                       
                }
                if (firstCell > 0)
                myDGNDumpElement(hDGN, psElement, stdout);
            }
            

            CPLAssert(psElement->type >= 0 && psElement->type < 128);

            if (achRaw[psElement->type] != 0)
                if (output)
                {
                    DGNDumpRawElement(hDGN, psElement, outputPath);
                }
                else
                {
                    DGNDumpRawElement(hDGN, psElement, stdout);
                }
                

            if (bReportExtents)
            {
                DGNPoint sMin, sMax;
                if (DGNGetElementExtents(hDGN, psElement, &sMin, &sMax))
                    printf("  Extents: (%.6f,%.6f,%.6f)\n"
                        "        to (%.6f,%.6f,%.6f)\n",
                        sMin.x, sMin.y, sMin.z,
                        sMax.x, sMax.y, sMax.z);
            }

            DGNFreeElement(hDGN, psElement);
        }
    }
    else
    {
        const DGNElementInfo* pasEI;
        int                     nCount, i, nLevel, nType;
        int                     anLevelTypeCount[128 * 64];
        int                     anLevelCount[64];
        int                     anTypeCount[128];
        double                  adfExtents[6];

        DGNGetExtents(hDGN, adfExtents);
        printf("X Range: %.2f to %.2f\n",
            adfExtents[0], adfExtents[3]);
        printf("Y Range: %.2f to %.2f\n",
            adfExtents[1], adfExtents[4]);
        printf("Z Range: %.2f to %.2f\n",
            adfExtents[2], adfExtents[5]);

        pasEI = DGNGetElementIndex(hDGN, &nCount);

        printf("Total Elements: %d\n", nCount);

        memset(anLevelTypeCount, 0, 128 * 64 * sizeof(int));
        memset(anLevelCount, 0, 64 * sizeof(int));
        memset(anTypeCount, 0, 128 * sizeof(int));

        for (i = 0; i < nCount; i++)
        {
            anLevelTypeCount[pasEI[i].level * 128 + pasEI[i].type]++;
            anLevelCount[pasEI[i].level]++;
            anTypeCount[pasEI[i].type]++;
        }

        printf("\n");
        printf("Per Type Report\n");
        printf("===============\n");

        for (nType = 0; nType < 128; nType++)
        {
            if (anTypeCount[nType] != 0)
            {
                printf("Type %s: %d\n",
                    DGNTypeToName(nType),
                    anTypeCount[nType]);
            }
        }

        printf("\n");
        printf("Per Level Report\n");
        printf("================\n");

        for (nLevel = 0; nLevel < 64; nLevel++)
        {
            if (anLevelCount[nLevel] == 0)
                continue;

            printf("Level %d, %d elements:\n",
                nLevel,
                anLevelCount[nLevel]);

            for (nType = 0; nType < 128; nType++)
            {
                if (anLevelTypeCount[nLevel * 128 + nType] != 0)
                {
                    printf("  Type %s: %d\n",
                        DGNTypeToName(nType),
                        anLevelTypeCount[nLevel * 128 + nType]);
                }
            }

            printf("\n");
        }
    }

    DGNClose(hDGN);

    return 0;

}

/************************************************************************/
/*                         DGNDumpRawElement()                          */
/************************************************************************/

// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门使用技巧: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件

