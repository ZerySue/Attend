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
<link rel=File-List href="新巨龙月度考勤报表_files/filelist.xml">
<link rel=Edit-Time-Data href="新巨龙月度考勤报表_files/editdata.mso">
<link rel=OLE-Object-Data href="新巨龙月度考勤报表_files/oledata.mso">
<!--[if gte mso 9]><xml>
 <o:DocumentProperties>
  <o:Author>YuHailong</o:Author>
  <o:LastAuthor>Windows 用户</o:LastAuthor>
  <o:LastPrinted>2013-12-24T03:32:53Z</o:LastPrinted>
  <o:Created>2008-01-10T02:24:54Z</o:Created>
  <o:LastSaved>2014-03-24T08:57:40Z</o:LastSaved>
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
	mso-style-name:Normal_新巨龙月度考勤报表;}
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
.xl25
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;}
.xl26
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl27
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl28
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl29
	{mso-style-parent:style21;
	color:black;
	font-size:16.0pt;
	font-weight:700;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl30
	{mso-style-parent:style21;
	color:black;
	font-size:16.0pt;
	font-weight:700;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl31
	{mso-style-parent:style21;
	color:black;
	font-size:16.0pt;
	font-weight:700;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl32
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	font-weight:700;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl33
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	font-weight:700;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl34
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	font-weight:700;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl35
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:.5pt solid windowtext;}
.xl36
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl37
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl38
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl39
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
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
    <x:Name>新巨龙月度考勤报表</x:Name>
    <x:WorksheetOptions>
     <x:DefaultRowHeight>270</x:DefaultRowHeight>
     <x:Print>
      <x:ValidPrinterInfo/>
      <x:PaperSizeIndex>8</x:PaperSizeIndex>
      <x:HorizontalResolution>600</x:HorizontalResolution>
      <x:VerticalResolution>600</x:VerticalResolution>
     </x:Print>
     <x:Selected/>
     <x:Panes>
      <x:Pane>
       <x:Number>3</x:Number>
       <x:RangeSelection>$A$1:$AX$1</x:RangeSelection>
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
  <x:Formula>=新巨龙月度考勤报表!$1:$5</x:Formula>
 </x:ExcelName>
</xml><![endif]-->
</head>

<body link=blue vlink=purple class=xl25>

<table x:str border=0 cellpadding=0 cellspacing=0 width=2494 style='border-collapse:
 collapse;table-layout:fixed;width:1883pt'>
 <col class=xl25 width=29 style='mso-width-source:userset;mso-width-alt:928;
 width:22pt'>
 <col class=xl25 width=85 style='mso-width-source:userset;mso-width-alt:2720;
 width:64pt'>
 <col class=xl25 width=69 span=3 style='mso-width-source:userset;mso-width-alt:
 2208;width:52pt'>
 <col class=xl25 width=29 span=14 style='mso-width-source:userset;mso-width-alt:
 928;width:22pt'>
 <col class=xl25 width=57 span=31 style='mso-width-source:userset;mso-width-alt:
 1824;width:43pt'>
 <tr height=33 style='mso-height-source:userset;height:24.75pt'>
  <td colspan=50 height=33 class=xl29 width=2494 style='border-right:.5pt solid black;
  height:24.75pt;width:1883pt'>月度考勤报表</td>
 </tr>
 <%
      System.DateTime beginTime = Convert.ToDateTime(Request.QueryString["beginTime"]);      
 %>
 <tr height=33 style='mso-height-source:userset;height:24.75pt'>
  <td colspan=50 height=33 class=xl32 style='border-right:.5pt solid black;
  height:24.75pt'><%=beginTime.ToString("yyyy")%>年<%=beginTime.ToString("MM")%>月</td>
 </tr>
 <tr height=19 style='mso-height-source:userset;height:14.25pt'>
  <td rowspan=2 height=38 class=xl35 style='border-bottom:.5pt solid black;
  height:28.5pt;border-top:none'>序号</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>部门</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>姓名</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>工号</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>下井个数</td>
  <td colspan=7 class=xl38 style='border-right:.5pt solid black;border-left:
  none'>按班次分</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>无效</td>
  <td colspan=5 class=xl38 style='border-right:.5pt solid black;border-left:
  none'>按时间分</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>类别</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>1</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>2</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>3</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>4</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>5</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>6</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>7</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>8</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>9</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>10</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>11</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>12</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>13</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>14</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>15</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>16</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>17</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>18</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>19</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>20</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>21</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>22</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>23</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>24</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>25</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>26</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>27</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>28</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>29</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>30</td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'>31</td>
 </tr>
 <tr height=19 style='mso-height-source:userset;height:14.25pt'>
  <td height=19 class=xl26 style='height:14.25pt'>夜班</td>
  <td class=xl26>早班</td>
  <td class=xl26>中班</td>
  <td class=xl26>一班</td>
  <td class=xl26>二班</td>
  <td class=xl26>三班</td>
  <td class=xl26>四班</td>
  <td class=xl27>0-3</td>
  <td class=xl27>3-4</td>
  <td class=xl27>4-6</td>
  <td class=xl27>6-8</td>
  <td class=xl27>&gt;=8</td>
 </tr> 
 <% 
    int i=0;
    for (i = 0; i < DomainServiceIriskingAttend.ReportPersonMonthAttendList.Count;i++ )
    {
 %>        
 <tr height=18 style='mso-height-source:userset;height:13.5pt'>
  <td rowspan=2 height=36 class=xl35 style='border-bottom:.5pt solid black;
  height:27.0pt;border-top:none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].Index %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].depart_name %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].name %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].work_sn %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].valid_times %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].wan_times %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].zao_times %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].zhong_times %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].one_times %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].two_times %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].three_times %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].four_times %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].invalid_times %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].sum_0_3 %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].sum_3_4 %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].sum_4_6 %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].sum_6_8 %></td>
  <td rowspan=2 class=xl35 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].sum_8 %></td>
  <td class=xl27>班次</td>
  <%
      for( int dayIndex = 0; dayIndex < 31; dayIndex++)
      {
  %>
  <td class=xl27><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].attend_signs[dayIndex] %></td>
  <%
      }
  %> 
 </tr>
 <tr height=18 style='mso-height-source:userset;height:13.5pt'>
  <td height=18 class=xl27 style='height:13.5pt'>时长</td>
  <%
      for( int dayIndex = 0; dayIndex < 31; dayIndex++)
      {
  %>
  <td class=xl27><%=DomainServiceIriskingAttend.ReportPersonMonthAttendList[i].work_times[dayIndex] %></td>
  <%
      }
  %>  
 </tr>
 <%
   }
 %> 
 <![if supportMisalignedColumns]>
 <tr height=0 style='display:none'>
  <td width=29 style='width:22pt'></td>
  <td width=85 style='width:64pt'></td>
  <td width=69 style='width:52pt'></td>
  <td width=69 style='width:52pt'></td>
  <td width=69 style='width:52pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=57 style='width:43pt'></td>
 </tr>
 <![endif]>
</table>

</body>

</html>
