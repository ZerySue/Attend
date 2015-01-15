<%@ Page Language="C#" EnableEventValidation = "false" ResponseEncoding="gb2312" ContentType="application/vnd.ms-excel" %>
<%@ Import Namespace="IriskingAttend.Web" %>
<%@ Import Namespace="System" %>
<html xmlns:o="urn:schemas-microsoft-com:office:office"
xmlns:x="urn:schemas-microsoft-com:office:excel"
xmlns="http://www.w3.org/TR/REC-html40">

<head>
<meta http-equiv=Content-Type content="text/html; charset=gb2312">
<meta name=ProgId content=Excel.Sheet>
<meta name=Generator content="Microsoft Excel 11">
<link rel=File-List href="周源山表1_files/filelist.xml">
<link rel=Edit-Time-Data href="周源山表1_files/editdata.mso">
<link rel=OLE-Object-Data href="周源山表1_files/oledata.mso">
<!--[if gte mso 9]><xml>
 <o:DocumentProperties>
  <o:Author>YuHailong</o:Author>
  <o:LastAuthor>微软用户</o:LastAuthor>
  <o:LastPrinted>2013-12-24T03:32:53Z</o:LastPrinted>
  <o:Created>2008-01-10T02:24:54Z</o:Created>
  <o:LastSaved>2014-01-16T07:40:56Z</o:LastSaved>
  <o:Company>iris</o:Company>
  <o:Version>11.5606</o:Version>
 </o:DocumentProperties>
</xml><![endif]-->
<style>
<!--table
	{mso-displayed-decimal-separator:"\.";
	mso-displayed-thousand-separator:"\,";}
@page
	{margin:1.0in .75in 1.0in .75in;
	mso-header-margin:.5in;
	mso-footer-margin:.5in;}
.font8
	{color:black;
	font-size:9.0pt;
	font-weight:400;
	font-style:normal;
	text-decoration:none;
	font-family:宋体;
	mso-generic-font-family:auto;
	mso-font-charset:134;}
tr
	{mso-height-source:auto;
	mso-ruby-visibility:none;}
col
	{mso-width-source:auto;
	mso-ruby-visibility:none;}
br
	{mso-data-placement:same-cell;}
.style0
	{mso-number-format:General;
	text-align:general;
	vertical-align:middle;
	white-space:nowrap;
	mso-rotate:0;
	mso-background-source:auto;
	mso-pattern:auto;
	color:windowtext;
	font-size:12.0pt;
	font-weight:400;
	font-style:normal;
	text-decoration:none;
	font-family:宋体;
	mso-generic-font-family:auto;
	mso-font-charset:134;
	border:none;
	mso-protection:locked visible;
	mso-style-name:Normal;
	mso-style-id:0;}
.style21
	{mso-number-format:General;
	text-align:general;
	vertical-align:middle;
	white-space:nowrap;
	mso-rotate:0;
	mso-background-source:auto;
	mso-pattern:auto;
	color:black;
	font-size:11.0pt;
	font-weight:400;
	font-style:normal;
	text-decoration:none;
	font-family:宋体;
	mso-generic-font-family:auto;
	mso-font-charset:134;
	border:none;
	mso-protection:locked visible;
	mso-style-name:Normal_周源山矿报表;}
td
	{mso-style-parent:style0;
	padding-top:1px;
	padding-right:1px;
	padding-left:1px;
	mso-ignore:padding;
	color:windowtext;
	font-size:12.0pt;
	font-weight:400;
	font-style:normal;
	text-decoration:none;
	font-family:宋体;
	mso-generic-font-family:auto;
	mso-font-charset:134;
	mso-number-format:General;
	text-align:general;
	vertical-align:middle;
	border:none;
	mso-background-source:auto;
	mso-pattern:auto;
	mso-protection:locked visible;
	white-space:nowrap;
	mso-rotate:0;}
.xl69
	{mso-style-parent:style21;
	color:black;
	font-size:16.0pt;
	font-weight:700;
	mso-number-format:"\@";
	text-align:center;
	border:.5pt solid windowtext;}
.xl70
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;}
.xl71
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border:.5pt solid windowtext;}
.xl72
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border:.5pt solid windowtext;
	white-space:normal;}
.xl73
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	border:.5pt solid windowtext;}
.xl74
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	text-align:center;
	border:.5pt solid windowtext;}
.xl75
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	text-align:left;
	vertical-align:top;
	border:.5pt solid windowtext;
	white-space:normal;}
ruby
	{ruby-align:left;}
rt
	{color:windowtext;
	font-size:9.0pt;
	font-weight:400;
	font-style:normal;
	text-decoration:none;
	font-family:宋体;
	mso-generic-font-family:auto;
	mso-font-charset:134;
	mso-char-type:none;
	display:none;}
-->
</style>
<!--[if gte mso 9]><xml>
 <x:ExcelWorkbook>
  <x:ExcelWorksheets>
   <x:ExcelWorksheet>
    <x:Name>(一)周源山矿个人月报表</x:Name>
    <x:WorksheetOptions>
     <x:DefaultRowHeight>270</x:DefaultRowHeight>
     <x:Print>
      <x:ValidPrinterInfo/>
      <x:PaperSizeIndex>8</x:PaperSizeIndex>
      <x:HorizontalResolution>600</x:HorizontalResolution>
      <x:VerticalResolution>600</x:VerticalResolution>
     </x:Print>
     <x:Zoom>90</x:Zoom>
     <x:Selected/>
     <x:Panes>
      <x:Pane>
       <x:Number>3</x:Number>
       <x:ActiveRow>37</x:ActiveRow>
       <x:ActiveCol>19</x:ActiveCol>
      </x:Pane>
     </x:Panes>
     <x:ProtectContents>False</x:ProtectContents>
     <x:ProtectObjects>False</x:ProtectObjects>
     <x:ProtectScenarios>False</x:ProtectScenarios>
    </x:WorksheetOptions>
   </x:ExcelWorksheet>
  </x:ExcelWorksheets>
  <x:WindowHeight>8190</x:WindowHeight>
  <x:WindowWidth>9555</x:WindowWidth>
  <x:WindowTopX>480</x:WindowTopX>
  <x:WindowTopY>15</x:WindowTopY>
  <x:ProtectStructure>False</x:ProtectStructure>
  <x:ProtectWindows>False</x:ProtectWindows>
 </x:ExcelWorkbook>
 <x:ExcelName>
  <x:Name>Print_Titles</x:Name>
  <x:SheetIndex>1</x:SheetIndex>
  <x:Formula>='(一)周源山矿个人月报表'!$1:$11</x:Formula>
 </x:ExcelName>
</xml><![endif]-->
</head>

<body link=blue vlink=purple class=xl70>

<table x:str border=0 cellpadding=0 cellspacing=0 width=3062 style='border-collapse:
 collapse;table-layout:fixed;width:2305pt'>
 <col class=xl70 width=31 style='mso-width-source:userset;mso-width-alt:992;
 width:23pt'>
 <col class=xl70 width=35 style='mso-width-source:userset;mso-width-alt:1120;
 width:26pt'>
 <col class=xl70 width=43 style='mso-width-source:userset;mso-width-alt:1376;
 width:32pt'>
 <col class=xl70 width=41 style='mso-width-source:userset;mso-width-alt:1312;
 width:31pt'>
 <col class=xl70 width=66 style='mso-width-source:userset;mso-width-alt:2112;
 width:50pt'>
 <col class=xl70 width=50 style='mso-width-source:userset;mso-width-alt:1600;
 width:38pt'>
 <col class=xl70 width=53 style='mso-width-source:userset;mso-width-alt:1696;
 width:40pt'>
 <col class=xl70 width=37 style='mso-width-source:userset;mso-width-alt:1184;
 width:28pt'>
 <col class=xl70 width=32 style='mso-width-source:userset;mso-width-alt:1024;
 width:24pt'>
 <col class=xl70 width=27 style='mso-width-source:userset;mso-width-alt:864;
 width:20pt'>
 <col class=xl70 width=28 span=8 style='mso-width-source:userset;mso-width-alt:
 896;width:21pt'>
 <col class=xl70 width=36 style='mso-width-source:userset;mso-width-alt:1152;
 width:27pt'>
 <col class=xl70 width=77 span=31 style='mso-width-source:userset;mso-width-alt:
 2464;width:58pt'>
 <tr height=33 style='mso-height-source:userset;height:24.75pt'>
  <td colspan=50 height=33 class=xl69 width=3062 style='height:24.75pt;
  width:2305pt'>周源山煤矿个人月报表详情</td>
 </tr>
 <%
      System.DateTime beginTime = Convert.ToDateTime(Request.QueryString["beginTime"]);
      System.DateTime endTime = Convert.ToDateTime(Request.QueryString["endTime"]);     
 %>
 <tr height=33 style='mso-height-source:userset;height:24.75pt'>
  <td colspan=50 height=33 class=xl69 style='height:24.75pt'>开始时间：<%=beginTime.ToString("yyyy-MM-dd")%><span style='mso-spacerun:yes'>&nbsp;&nbsp;&nbsp;
  </span>截止时间：<%=endTime.ToString("yyyy-MM-dd")%></td>
 </tr>
 <tr height=19 style='mso-height-source:userset;height:14.25pt'>
  <td rowspan=2 height=38 class=xl71 style='height:28.5pt;border-top:none'>序号</td>
  <td rowspan=2 class=xl71 style='border-top:none'>月份</td>
  <td rowspan=2 class=xl71 style='border-top:none'>工号</td>
  <td rowspan=2 class=xl71 style='border-top:none'>姓名</td>
  <td rowspan=2 class=xl71 style='border-top:none'>部门</td>
  <td rowspan=2 class=xl71 style='border-top:none'>总时长</td>
  <td rowspan=2 class=xl71 style='border-top:none'>井下均时</td>
  <td rowspan=2 class=xl71 style='border-top:none'>总次数</td>
  <td class=xl72 width=32 style='border-top:none;border-left:none;width:24pt'>有效</td>
  <td colspan=3 class=xl71 style='border-left:none'>按班次分</td>
  <td rowspan=2 class=xl71 style='border-top:none'>无效</td>
  <td colspan=5 class=xl71 style='border-left:none'>按时间分</td>
  <td rowspan=2 class=xl71 style='border-top:none'>类别</td>
  <td rowspan=2 class=xl71 style='border-top:none'>1</td>
  <td rowspan=2 class=xl71 style='border-top:none'>2</td>
  <td rowspan=2 class=xl71 style='border-top:none'>3</td>
  <td rowspan=2 class=xl71 style='border-top:none'>4</td>
  <td rowspan=2 class=xl71 style='border-top:none'>5</td>
  <td rowspan=2 class=xl71 style='border-top:none'>6</td>
  <td rowspan=2 class=xl71 style='border-top:none'>7</td>
  <td rowspan=2 class=xl71 style='border-top:none'>8</td>
  <td rowspan=2 class=xl71 style='border-top:none'>9</td>
  <td rowspan=2 class=xl71 style='border-top:none'>10</td>
  <td rowspan=2 class=xl71 style='border-top:none'>11</td>
  <td rowspan=2 class=xl71 style='border-top:none'>12</td>
  <td rowspan=2 class=xl71 style='border-top:none'>13</td>
  <td rowspan=2 class=xl71 style='border-top:none'>14</td>
  <td rowspan=2 class=xl71 style='border-top:none'>15</td>
  <td rowspan=2 class=xl71 style='border-top:none'>16</td>
  <td rowspan=2 class=xl71 style='border-top:none'>17</td>
  <td rowspan=2 class=xl71 style='border-top:none'>18</td>
  <td rowspan=2 class=xl71 style='border-top:none'>19</td>
  <td rowspan=2 class=xl71 style='border-top:none'>20</td>
  <td rowspan=2 class=xl71 style='border-top:none'>21</td>
  <td rowspan=2 class=xl71 style='border-top:none'>22</td>
  <td rowspan=2 class=xl71 style='border-top:none'>23</td>
  <td rowspan=2 class=xl71 style='border-top:none'>24</td>
  <td rowspan=2 class=xl71 style='border-top:none'>25</td>
  <td rowspan=2 class=xl71 style='border-top:none'>26</td>
  <td rowspan=2 class=xl71 style='border-top:none'>27</td>
  <td rowspan=2 class=xl71 style='border-top:none'>28</td>
  <td rowspan=2 class=xl71 style='border-top:none'>29</td>
  <td rowspan=2 class=xl71 style='border-top:none'>30</td>
  <td rowspan=2 class=xl71 style='border-top:none'>31</td>
 </tr>
 <tr height=19 style='mso-height-source:userset;height:14.25pt'>
  <td height=19 class=xl72 width=32 style='height:14.25pt;border-top:none;
  border-left:none;width:24pt'>次数</td>
  <td class=xl73 style='border-top:none;border-left:none'>早班</td>
  <td class=xl73 style='border-top:none;border-left:none'>中班</td>
  <td class=xl73 style='border-top:none;border-left:none'>晚班</td>
  <td class=xl71 style='border-top:none;border-left:none'>0-2</td>
  <td class=xl71 style='border-top:none;border-left:none'>2-4</td>
  <td class=xl71 style='border-top:none;border-left:none'>4-8</td>
  <td class=xl71 style='border-top:none;border-left:none'>8-12</td>
  <td class=xl71 style='border-top:none;border-left:none'>&gt;12</td>
 </tr>
  <% 
    int count = 0;
    int condition = DomainServiceIriskingAttend.ConditionShowElementType;

    while (condition > 0)
    {
        condition = condition & (condition - 1);
        count += 1;
    }
      
    int i=0;
    for (i = 0; i < DomainServiceIriskingAttend.PersonAttendStatisticsList.Count-1;i++ )
    {
        bool bFirst = true;
%>
 <tr height=18 style='mso-height-source:userset;height:13.5pt'>
  <td rowspan=<%=count%> height=54 class=xl71 style='height:40.5pt;border-top:none'><%=i+1 %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].month %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].work_sn %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].name %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].depart_name %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'>
        <%=string.Format("{0}:{1}",
        DomainServiceIriskingAttend.PersonAttendStatisticsList[i].total_work_time/60,
        DomainServiceIriskingAttend.PersonAttendStatisticsList[i].total_work_time % 60)%>
  </td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'>
        <%=string.Format("{0}:{1}",
        DomainServiceIriskingAttend.PersonAttendStatisticsList[i].avg_work_time/60,
        DomainServiceIriskingAttend.PersonAttendStatisticsList[i].avg_work_time % 60)%>
  　</td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].total_times %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].valid_times %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].zao_times %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].zhong_times %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].wan_times %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].invalid_times %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].sum_0_2 %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].sum_2_4 %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].sum_4_8 %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].sum_8_12 %></td>
  <td rowspan=<%=count%> class=xl71 style='border-top:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].sum_12 %></td>
  <%
      if( (DomainServiceIriskingAttend.ConditionShowElementType & (int)IriskingAttend.Web.ZhouYuanShan.ShowElementType.ClassOrder) != 0 )
      {
          bFirst = false;
  %>
  <td class=xl71 style='border-top:none;border-left:none'>班次</td>
  <%
         for (int dayIndex = 0; dayIndex < 31; dayIndex++)
         {
             if (DomainServiceIriskingAttend.PersonAttendStatisticsList[i].display_content_color[dayIndex].CompareTo("0xffffa500") == 0)
             {
  %>
         <td class=xl71 style='color:Orange;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].attend_signs[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.PersonAttendStatisticsList[i].display_content_color[dayIndex].CompareTo("0xff008000") == 0)
             {
  %>
         <td class=xl71 style='color:Green;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].attend_signs[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.PersonAttendStatisticsList[i].display_content_color[dayIndex].CompareTo("0xffff0000") == 0)
             {
  %>
         <td class=xl71 style='color:Red;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].attend_signs[dayIndex]%></td>
  <% 
             }   
             else
             {
  %>
         <td class=xl71 style='color:Black;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].attend_signs[dayIndex]%></td>
  <% 
             } 
         }    
  %>
 </tr>
 <%    
      }
      
      if ((DomainServiceIriskingAttend.ConditionShowElementType & (int)IriskingAttend.Web.ZhouYuanShan.ShowElementType.Duration) != 0)
      {
          if( !bFirst )
          {
     %>
     <tr height=18 style='mso-height-source:userset;height:13.5pt'>
     <%  
          }
          bFirst = false;
 %>
  <td height=18 class=xl71 style='height:13.5pt;border-top:none;border-left:none'>时长</td>
  <%
         for (int dayIndex = 0; dayIndex < 31; dayIndex++)
         {
             if (DomainServiceIriskingAttend.PersonAttendStatisticsList[i].display_content_color[dayIndex].CompareTo("0xffffa500") == 0)
             {
  %>
         <td class=xl71 style='color:Orange;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].work_times[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.PersonAttendStatisticsList[i].display_content_color[dayIndex].CompareTo("0xff008000") == 0)
             {
  %>
         <td class=xl71 style='color:Green;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].work_times[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.PersonAttendStatisticsList[i].display_content_color[dayIndex].CompareTo("0xffff0000") == 0)
             {
  %>
         <td class=xl71 style='color:Red;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].work_times[dayIndex]%></td>
  <% 
             }   
             else
             {
  %>
         <td class=xl71 style='color:Black;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].work_times[dayIndex]%></td>
  <% 
             } 
         }    
  %>
  
 </tr>
 <%    
      }
      
      if ((DomainServiceIriskingAttend.ConditionShowElementType & (int)IriskingAttend.Web.ZhouYuanShan.ShowElementType.Time) != 0)
      { 
          if( !bFirst )
          {
     %>
     <tr height=18 style='mso-height-source:userset;height:13.5pt'>
     <%  
          }
 %>
  <td height=18 class=xl71 style='height:13.5pt;border-top:none;border-left:none'>时间</td>
  <%
         for (int dayIndex = 0; dayIndex < 31; dayIndex++)
         {
             if (DomainServiceIriskingAttend.PersonAttendStatisticsList[i].display_content_color[dayIndex].CompareTo("0xffffa500") == 0)
             {
  %>
         <td class=xl71 style='color:Orange;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].time_descriptions[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.PersonAttendStatisticsList[i].display_content_color[dayIndex].CompareTo("0xff008000") == 0)
             {
  %>
         <td class=xl71 style='color:Green;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].time_descriptions[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.PersonAttendStatisticsList[i].display_content_color[dayIndex].CompareTo("0xffff0000") == 0)
             {
  %>
         <td class=xl71 style='color:Red;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].time_descriptions[dayIndex]%></td>
  <% 
             }   
             else
             {
  %>
         <td class=xl71 style='color:Black;border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].time_descriptions[dayIndex]%></td>
  <% 
             } 
         }    
  %>
 </tr>
 <%
     }
  }
 %> 
 <tr height=18 style='mso-height-source:userset;height:13.5pt'>
  <td height=18 class=xl71 style='height:13.5pt;border-top:none'>合计</td>
  <td class=xl71 style='border-top:none;border-left:none'> </td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'><%=string.Format("{0}:{1}",
        DomainServiceIriskingAttend.PersonAttendStatisticsList[i].total_work_time/60,
        DomainServiceIriskingAttend.PersonAttendStatisticsList[i].total_work_time % 60)%></td>
  <td class=xl71 style='border-top:none;border-left:none'>
      <%=string.Format("{0}:{1}",
        DomainServiceIriskingAttend.PersonAttendStatisticsList[i].avg_work_time/60,
        DomainServiceIriskingAttend.PersonAttendStatisticsList[i].avg_work_time % 60)%></td>
  <td class=xl71 style='border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].total_times%></td>
  <td class=xl71 style='border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].valid_times%></td>
  <td class=xl71 style='border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].zao_times%></td>
  <td class=xl71 style='border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].zhong_times%></td>
  <td class=xl71 style='border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].wan_times%></td>
  <td class=xl71 style='border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].invalid_times%></td>
  <td class=xl71 style='border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].sum_0_2%></td>
  <td class=xl71 style='border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].sum_2_4%></td>
  <td class=xl71 style='border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].sum_4_8%></td>
  <td class=xl71 style='border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].sum_8_12%></td>
  <td class=xl71 style='border-top:none;border-left:none'><%=DomainServiceIriskingAttend.PersonAttendStatisticsList[i].sum_12%></td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
  <td class=xl71 style='border-top:none;border-left:none'>　</td>
 </tr>

 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <tr height=18 style='height:13.5pt'>
 </tr>
 <![if supportMisalignedColumns]>
 <tr height=0 style='display:none'>
  <td width=31 style='width:23pt'></td>
  <td width=35 style='width:26pt'></td>
  <td width=43 style='width:32pt'></td>
  <td width=41 style='width:31pt'></td>
  <td width=66 style='width:50pt'></td>
  <td width=50 style='width:38pt'></td>
  <td width=53 style='width:40pt'></td>
  <td width=37 style='width:28pt'></td>
  <td width=32 style='width:24pt'></td>
  <td width=27 style='width:20pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=36 style='width:27pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
 </tr>
 <![endif]>
</table>

</body>

</html>
