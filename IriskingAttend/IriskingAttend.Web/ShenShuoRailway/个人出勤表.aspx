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
<link rel=File-List href="个人出勤表_files/filelist.xml">
<link rel=Edit-Time-Data href="个人出勤表_files/editdata.mso">
<link rel=OLE-Object-Data href="个人出勤表_files/oledata.mso">
<!--[if gte mso 9]><xml>
 <o:DocumentProperties>
  <o:LastAuthor>微软用户</o:LastAuthor>
  <o:LastPrinted>2014-02-12T06:29:32Z</o:LastPrinted>
  <o:LastSaved>2014-02-12T07:12:15Z</o:LastSaved>
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
.xl24
	{mso-style-parent:style0;
	font-size:10.0pt;
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl25
	{mso-style-parent:style0;
	font-size:10.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl26
	{mso-style-parent:style0;
	font-size:10.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl27
	{mso-style-parent:style0;
	font-size:10.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:yellow;
	mso-pattern:black none;}
.xl28
	{mso-style-parent:style0;
	font-size:10.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:#969696;
	mso-pattern:black none;}
.xl29
	{mso-style-parent:style0;
	font-size:16.0pt;
	font-weight:700;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:none;
	border-left:.5pt solid windowtext;}
.xl30
	{mso-style-parent:style0;
	font-size:16.0pt;
	font-weight:700;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:none;
	border-left:none;}
.xl31
	{mso-style-parent:style0;
	font-size:16.0pt;
	font-weight:700;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid black;
	border-bottom:none;
	border-left:none;}
.xl32
	{mso-style-parent:style0;
	font-size:16.0pt;
	font-weight:700;
	text-align:center;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl33
	{mso-style-parent:style0;
	font-size:16.0pt;
	font-weight:700;
	text-align:center;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl34
	{mso-style-parent:style0;
	font-size:16.0pt;
	font-weight:700;
	text-align:center;
	border-top:none;
	border-right:.5pt solid black;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl35
	{mso-style-parent:style0;
	font-size:10.0pt;
	font-weight:700;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl36
	{mso-style-parent:style0;
	font-size:10.0pt;
	font-weight:700;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl37
	{mso-style-parent:style0;
	font-size:10.0pt;
	font-weight:700;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid black;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl38
	{mso-style-parent:style0;
	font-size:10.0pt;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:.5pt solid windowtext;}
.xl39
	{mso-style-parent:style0;
	font-size:10.0pt;
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid black;
	border-left:.5pt solid windowtext;}
.xl40
	{mso-style-parent:style0;
	font-size:10.0pt;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl41
	{mso-style-parent:style0;
	font-size:10.0pt;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid black;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl42
	{mso-style-parent:style0;
	font-size:10.0pt;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid black;}
.xl43
	{mso-style-parent:style0;
	font-size:10.0pt;
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
    <x:Name>个人出勤表</x:Name>
    <x:WorksheetOptions>
     <x:DefaultRowHeight>285</x:DefaultRowHeight>
     <x:Print>
      <x:ValidPrinterInfo/>
      <x:PaperSizeIndex>9</x:PaperSizeIndex>
      <x:HorizontalResolution>300</x:HorizontalResolution>
      <x:VerticalResolution>300</x:VerticalResolution>
     </x:Print>
     <x:Selected/>
     <x:DoNotDisplayGridlines/>
     <x:Panes>
      <x:Pane>
       <x:Number>3</x:Number>
       <x:RangeSelection>$A$1:$H$2</x:RangeSelection>
      </x:Pane>
     </x:Panes>
     <x:ProtectContents>False</x:ProtectContents>
     <x:ProtectObjects>False</x:ProtectObjects>
     <x:ProtectScenarios>False</x:ProtectScenarios>
    </x:WorksheetOptions>
   </x:ExcelWorksheet>
  </x:ExcelWorksheets>
  <x:WindowHeight>10695</x:WindowHeight>
  <x:WindowWidth>20355</x:WindowWidth>
  <x:WindowTopX>240</x:WindowTopX>
  <x:WindowTopY>120</x:WindowTopY>
  <x:ProtectStructure>False</x:ProtectStructure>
  <x:ProtectWindows>False</x:ProtectWindows>
 </x:ExcelWorkbook>
 <x:ExcelName>
  <x:Name>Print_Titles</x:Name>
  <x:SheetIndex>1</x:SheetIndex>
  <x:Formula>=个人出勤表!$1:$5</x:Formula>
 </x:ExcelName>
</xml><![endif]-->
</head>

<body link=blue vlink=purple>
<!--下列信息由 Microsoft Office Excel 的“发布为网页”向导生成。--><!--如果同一条目从 Excel 中重新发布，则所有位于 DIV 标记之间的信息均将被替换。--><!-----------------------------><!--“从 EXCEL 发布网页”向导开始--><!----------------------------->

<table x:str border=0 cellpadding=0 cellspacing=0 width=641 style='border-collapse:
 collapse;table-layout:fixed;width:482pt'>
 <col width=77 style='mso-width-source:userset;mso-width-alt:2464;width:58pt'>
 <col width=111 span=2 style='mso-width-source:userset;mso-width-alt:3552;
 width:83pt'>
 <col width=77 span=4 style='mso-width-source:userset;mso-width-alt:2464;
 width:58pt'>
 <col width=34 style='mso-width-source:userset;mso-width-alt:1088;width:26pt'>
 <tr height=24 style='mso-height-source:userset;height:18.0pt'>
  <td colspan=8 rowspan=2 height=48 class=xl29 width=641 style='border-right:
  .5pt solid black;border-bottom:.5pt solid black;height:36.0pt;width:482pt'><!--下列信息由 Microsoft Office Excel 的“发布为网页”向导生成。--><!--如果同一条目从 Excel 中重新发布，则所有位于 DIV 标记之间的信息均将被替换。--><!-----------------------------><!--“从 EXCEL 发布网页”向导开始--><!----------------------------->个人出勤表</td>
 </tr>
 <tr height=24 style='mso-height-source:userset;height:18.0pt'>
 </tr>
 <%
     System.DateTime beginTime = Convert.ToDateTime(Request.QueryString["beginTime"]);
     System.DateTime endTime = Convert.ToDateTime(Request.QueryString["endTime"]);
 %>
 <tr height=24 style='mso-height-source:userset;height:18.0pt'>
  <td colspan=8 height=24 class=xl35 style='border-right:.5pt solid black;
  height:18.0pt'>考勤周期：<%=beginTime.ToString("yyyy.MM.dd")%><span 
  style='mso-spacerun:yes'>&nbsp; </span>至<%=endTime.AddDays(-1).ToString("yyyy.MM.dd")%><span 
  style='mso-spacerun:yes'>&nbsp;</span></td>
 </tr>
 <tr height=24 style='mso-height-source:userset;height:18.0pt'>
  <td rowspan=2 height=48 class=xl38 style='border-bottom:.5pt solid black;
  height:36.0pt;border-top:none'>日期</td>
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:
  none'>部门</td>
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:
  none'>员工姓名</td>
  <td colspan=2 class=xl40 style='border-right:.5pt solid black;border-left:
  none'>上午</td>
  <td colspan=2 class=xl42 style='border-right:.5pt solid black;border-left:
  none'>下午</td>
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:
  none'>备注</td>
 </tr>
 <tr height=24 style='mso-height-source:userset;height:18.0pt'>
  <td height=24 class=xl24 style='height:18.0pt'>上班时间</td>
  <td class=xl24>下班时间</td>
  <td class=xl24>上班时间</td>
  <td class=xl24>下班时间</td>
 </tr>
 <%
     int NormalDay = 0;
     int ShiftDay = 1;
     int WeekendDay = 2;
     int FestivalDay = 3;    
     for (int i = 0; i < DomainServiceIriskingAttend.PersonAttendList.Count; i++)
     {
         if (DomainServiceIriskingAttend.PersonAttendList[i].DayType == WeekendDay)
         {   
%>
<tr height=24 style='mso-height-source:userset;height:18.0pt'>
  <td height=24 class=xl28 style='height:18.0pt'><%=DomainServiceIriskingAttend.PersonAttendList[i].AttendDay%></td>
  <td class=xl28><%=DomainServiceIriskingAttend.PersonAttendList[i].DepartName%></td>
  <td class=xl28><%=DomainServiceIriskingAttend.PersonAttendList[i].PersonName%></td>
  <td class=xl28><%=DomainServiceIriskingAttend.PersonAttendList[i].MoringInWellTime%></td>
  <td class=xl28><%=DomainServiceIriskingAttend.PersonAttendList[i].MoringOutWellTime%></td>
  <td class=xl28><%=DomainServiceIriskingAttend.PersonAttendList[i].AfternoonInWellTime%></td>
  <td class=xl28><%=DomainServiceIriskingAttend.PersonAttendList[i].AfternoonOutWellTime%></td>
  <td class=xl28>　</td>
 </tr>
<%
         }
         else if (DomainServiceIriskingAttend.PersonAttendList[i].DayType == FestivalDay)
         {
%>
<tr height=24 style='mso-height-source:userset;height:18.0pt'>
  <td height=24 class=xl27 style='height:18.0pt'><%=DomainServiceIriskingAttend.PersonAttendList[i].AttendDay%> </td>
  <td class=xl27><%=DomainServiceIriskingAttend.PersonAttendList[i].DepartName%></td>
  <td class=xl27><%=DomainServiceIriskingAttend.PersonAttendList[i].PersonName%></td>
  <td class=xl27><%=DomainServiceIriskingAttend.PersonAttendList[i].MoringInWellTime%></td>
  <td class=xl27><%=DomainServiceIriskingAttend.PersonAttendList[i].MoringOutWellTime%></td>
  <td class=xl27><%=DomainServiceIriskingAttend.PersonAttendList[i].AfternoonInWellTime%></td>
  <td class=xl27><%=DomainServiceIriskingAttend.PersonAttendList[i].AfternoonOutWellTime%></td>
  <td class=xl27>　</td>
 </tr>
<%
         }
         else
         {
 %>
 <tr height=24 style='mso-height-source:userset;height:18.0pt'>
  <td height=24 class=xl25 style='height:18.0pt'><%=DomainServiceIriskingAttend.PersonAttendList[i].AttendDay%> </td>
  <td class=xl26><%=DomainServiceIriskingAttend.PersonAttendList[i].DepartName%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.PersonAttendList[i].PersonName%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.PersonAttendList[i].MoringInWellTime%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.PersonAttendList[i].MoringOutWellTime%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.PersonAttendList[i].AfternoonInWellTime%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.PersonAttendList[i].AfternoonOutWellTime%></td>
  <td class=xl26>　</td>
 </tr>
<%
         } 
     }        
%>
 <![if supportMisalignedColumns]>
 <tr height=0 style='display:none'>
  <td width=77 style='width:58pt'></td>
  <td width=111 style='width:83pt'></td>
  <td width=111 style='width:83pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=77 style='width:58pt'></td>
  <td width=34 style='width:26pt'></td>
 </tr>
 <![endif]>
</table>

</body>

</html>
