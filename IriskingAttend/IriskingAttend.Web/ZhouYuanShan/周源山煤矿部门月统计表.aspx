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
<link rel=File-List href="周源山表4_files/filelist.xml">
<link rel=Edit-Time-Data href="周源山表4_files/editdata.mso">
<link rel=OLE-Object-Data href="周源山表4_files/oledata.mso">
<!--[if gte mso 9]><xml>
 <o:DocumentProperties>
  <o:Author>YuHailong</o:Author>
  <o:LastAuthor>微软用户</o:LastAuthor>
  <o:LastPrinted>2014-01-08T07:27:05Z</o:LastPrinted>
  <o:Created>2008-01-10T02:24:54Z</o:Created>
  <o:LastSaved>2014-01-08T07:41:28Z</o:LastSaved>
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
.font7
	{color:black;
	font-size:11.0pt;
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
.style121
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
	{mso-style-parent:style121;
	color:black;
	font-size:16.0pt;
	font-weight:700;}
.xl70
	{mso-style-parent:style121;
	color:black;
	font-size:11.0pt;}
.xl71
	{mso-style-parent:style121;
	color:black;
	font-size:16.0pt;
	font-weight:700;
	text-align:center;
	border:.5pt solid windowtext;}
.xl72
	{mso-style-parent:style121;
	color:black;
	font-weight:700;
	text-align:center;
	border:.5pt solid windowtext;}
.xl73
	{mso-style-parent:style121;
	color:black;
	font-size:9.0pt;
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl74
	{mso-style-parent:style121;
	color:black;
	font-size:9.0pt;
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl75
	{mso-style-parent:style121;
	color:black;
	font-size:9.0pt;
	text-align:center;
	border:.5pt solid windowtext;}
.xl76
	{mso-style-parent:style121;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border:.5pt solid windowtext;}
.xl77
	{mso-style-parent:style121;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl78
	{mso-style-parent:style121;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl79
	{mso-style-parent:style121;
	color:black;
	font-size:11.0pt;
	border:.5pt solid windowtext;}
.xl80
	{mso-style-parent:style121;
	color:black;
	font-size:11.0pt;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl81
	{mso-style-parent:style121;
	color:black;
	font-size:11.0pt;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl82
	{mso-style-parent:style121;
	color:black;
	font-size:11.0pt;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl83
	{mso-style-parent:style121;
	color:black;
	font-size:11.0pt;
	text-align:left;}
.xl84
	{mso-style-parent:style121;
	color:black;
	font-size:11.0pt;
	text-align:center;
	border:.5pt solid windowtext;}
.xl85
	{mso-style-parent:style121;
	color:black;
	font-size:11.0pt;
	mso-number-format:"\@";
	border:.5pt solid windowtext;}
.xl86
	{mso-style-parent:style121;
	color:black;
	font-size:11.0pt;
	text-align:left;
	vertical-align:top;
	border:.5pt solid windowtext;
	white-space:normal;}
.xl87
	{mso-style-parent:style121;
	color:black;
	font-size:11.0pt;
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
    <x:Name>（四）周源山矿部门月统计表</x:Name>
    <x:WorksheetOptions>
     <x:DefaultRowHeight>270</x:DefaultRowHeight>
     <x:Print>
      <x:ValidPrinterInfo/>
      <x:PaperSizeIndex>9</x:PaperSizeIndex>
      <x:HorizontalResolution>600</x:HorizontalResolution>
      <x:VerticalResolution>600</x:VerticalResolution>
     </x:Print>
     <x:Selected/>
     <x:Panes>
      <x:Pane>
       <x:Number>3</x:Number>
       <x:ActiveRow>6</x:ActiveRow>
       <x:ActiveCol>21</x:ActiveCol>
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
  <x:Formula>='（四）周源山矿部门月统计表'!$2:$6</x:Formula>
 </x:ExcelName>
</xml><![endif]-->
</head>

<body link=blue vlink=purple class=xl70>

<table x:str border=0 cellpadding=0 cellspacing=0 width=993 style='border-collapse:
 collapse;table-layout:fixed;width:748pt'>
 <col class=xl70 width=31 style='mso-width-source:userset;mso-width-alt:992;
 width:23pt'>
 <col class=xl70 width=34 style='mso-width-source:userset;mso-width-alt:1088;
 width:26pt'>
 <col class=xl70 width=103 style='mso-width-source:userset;mso-width-alt:3296;
 width:77pt'>
 <col class=xl70 width=54 span=2 style='mso-width-source:userset;mso-width-alt:
 1728;width:41pt'>
 <col class=xl70 width=42 style='mso-width-source:userset;mso-width-alt:1344;
 width:32pt'>
 <col class=xl70 width=48 style='mso-width-source:userset;mso-width-alt:1536;
 width:36pt'>
 <col class=xl70 width=28 span=2 style='mso-width-source:userset;mso-width-alt:
 896;width:21pt'>
 <col class=xl70 width=27 style='mso-width-source:userset;mso-width-alt:864;
 width:20pt'>
 <col class=xl70 width=30 span=3 style='mso-width-source:userset;mso-width-alt:
 960;width:23pt'>
 <col class=xl70 width=33 style='mso-width-source:userset;mso-width-alt:1056;
 width:25pt'>
 <col class=xl70 width=31 style='mso-width-source:userset;mso-width-alt:992;
 width:23pt'>
 <col class=xl70 width=30 style='mso-width-source:userset;mso-width-alt:960;
 width:23pt'>
 <col class=xl70 width=72 span=5 style='width:54pt'> 
 <tr height=38 style='mso-height-source:userset;height:28.5pt'>
  <td colspan=16 height=38 class=xl71 style='height:28.5pt'>周源山煤矿部门月统计表</td>
  <td colspan=5 class=xl70 style='mso-ignore:colspan'></td>
 </tr>
 <%
     System.DateTime beginTime = Convert.ToDateTime(Request.QueryString["beginTime"]);  
     System.DateTime endTime = Convert.ToDateTime(Request.QueryString["endTime"]);   
 %>
 <tr height=33 style='mso-height-source:userset;height:24.75pt'>
  <td colspan=16 height=33 class=xl72 style='height:24.75pt'>开始时间：<%=beginTime.ToString("yyyy-MM-dd:00:00:00") %><span style='mso-spacerun:yes'>&nbsp;&nbsp;&nbsp;
  </span>截止时间：<%=endTime.ToString("yyyy-MM-dd:23:59:59")%></td>
  <td colspan=5 class=xl70 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='mso-height-source:userset;height:13.5pt'>
  <td height=18 class=xl73 style='height:13.5pt'>序号</td>
  <td class=xl74>月份</td>
  <td class=xl74>部门</td>
  <td class=xl75 style='border-top:none;border-left:none'>总时长</td>
  <td class=xl75 style='border-top:none;border-left:none'>井下均时</td>
  <td class=xl75 style='border-top:none;border-left:none'>总次数</td>
  <td class=xl75 style='border-top:none;border-left:none'>有效次数</td>
  <td class=xl75 style='border-top:none;border-left:none'>早班</td>
  <td class=xl75 style='border-top:none;border-left:none'>中班</td>
  <td class=xl75 style='border-top:none;border-left:none'>晚班</td>
  <td class=xl75 style='border-top:none;border-left:none'>无效</td>
  <td class=xl76 style='border-top:none;border-left:none'>0-2</td>
  <td class=xl76 style='border-top:none;border-left:none'>2-4</td>
  <td class=xl76 style='border-top:none;border-left:none'>4-8</td>
  <td class=xl76 style='border-top:none;border-left:none'>8-12</td>
  <td class=xl76 style='border-top:none;border-left:none'>&gt;12</td>
  <td colspan=5 class=xl70 style='mso-ignore:colspan'></td>
 </tr>
 <%    
     for (int i = 0; i < DomainServiceIriskingAttend.DepartMonthAttendList.Count; i++)
     {         
%>
 <tr height=18 style='mso-height-source:userset;height:13.5pt'>
  <td height=18 class=xl77 style='height:13.5pt'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].Index%></td>
  <td class=xl78><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].AttendMonth%></td>
  <td class=xl78><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].DepartName%></td>
  <td class=xl76 style='border-top:none;border-left:none'><% =string.Format("{0}:{1}", DomainServiceIriskingAttend.DepartMonthAttendList[i].TotalWorkTime / 60, (DomainServiceIriskingAttend.DepartMonthAttendList[i].TotalWorkTime % 60).ToString("d2"))%></td>
  <td class=xl76 style='border-top:none;border-left:none'><% =string.Format("{0}:{1}", DomainServiceIriskingAttend.DepartMonthAttendList[i].AvgWorkTime / 60, (DomainServiceIriskingAttend.DepartMonthAttendList[i].AvgWorkTime % 60).ToString("d2"))%></td>
  <td class=xl75 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].TotalTimes%></td>
  <td class=xl75 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].ValidTimes%></td>
  <td class=xl75 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].ZaoTimes%></td>
  <td class=xl75 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].ZhongTimes%></td>
  <td class=xl75 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].WanTimes%></td>
  <td class=xl75 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].InvalidTimes%></td>
  <td class=xl75 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].Sum0To2%></td>
  <td class=xl75 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].Sum2To4%></td>
  <td class=xl75 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].Sum4To8%></td>
  <td class=xl75 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].Sum8To12%></td>
  <td class=xl75 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DepartMonthAttendList[i].Sum12Up%></td>
  <td colspan=5 class=xl70 style='mso-ignore:colspan'></td>
 </tr> 
<%
     }
%> 
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <tr height=18 style='height:13.5pt'>
  <td height=18 colspan=5 class=xl70 style='height:13.5pt;mso-ignore:colspan'></td>
 </tr>
 <![if supportMisalignedColumns]>
 <tr height=0 style='display:none'>
  <td width=31 style='width:23pt'></td>
  <td width=34 style='width:26pt'></td>
  <td width=103 style='width:77pt'></td>
  <td width=54 style='width:41pt'></td>
  <td width=54 style='width:41pt'></td>
  <td width=42 style='width:32pt'></td>
  <td width=48 style='width:36pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=27 style='width:20pt'></td>
  <td width=30 style='width:23pt'></td>
  <td width=30 style='width:23pt'></td>
  <td width=30 style='width:23pt'></td>
  <td width=33 style='width:25pt'></td>
  <td width=31 style='width:23pt'></td>
  <td width=30 style='width:23pt'></td>
  <td width=72 style='width:54pt'></td>
  <td width=72 style='width:54pt'></td>
  <td width=72 style='width:54pt'></td>
  <td width=72 style='width:54pt'></td>
  <td width=72 style='width:54pt'></td>
 </tr>
 <![endif]>
</table>

</body>

</html>
