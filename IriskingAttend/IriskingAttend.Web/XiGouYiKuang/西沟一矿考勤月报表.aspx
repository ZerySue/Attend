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
<link rel=File-List href="西沟一矿日报表_files/filelist.xml">
<link rel=Edit-Time-Data href="西沟一矿日报表_files/editdata.mso">
<link rel=OLE-Object-Data href="西沟一矿日报表_files/oledata.mso">
<!--[if gte mso 9]><xml>
 <o:DocumentProperties>
  <o:Author>YuHailong</o:Author>
  <o:LastAuthor>Windows 用户</o:LastAuthor>
  <o:LastPrinted>2008-01-10T07:45:04Z</o:LastPrinted>
  <o:Created>2008-01-10T02:24:54Z</o:Created>
  <o:LastSaved>2014-07-09T09:48:50Z</o:LastSaved>
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
	font-size:9.0pt;}
.xl25
	{mso-style-parent:style0;
	font-size:9.0pt;
	text-align:center;}
.xl26
	{mso-style-parent:style0;
	font-size:9.0pt;
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl27
	{mso-style-parent:style0;
	font-size:9.0pt;
	text-align:center;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl28
	{mso-style-parent:style0;
	font-size:9.0pt;
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl29
	{mso-style-parent:style0;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl30
	{mso-style-parent:style0;
	text-align:center;}
.xl31
	{mso-style-parent:style0;
	font-size:9.0pt;
	text-align:left;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl32
	{mso-style-parent:style0;
	font-size:9.0pt;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:.5pt solid windowtext;}
.xl33
	{mso-style-parent:style0;
	font-size:9.0pt;
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid black;
	border-left:.5pt solid windowtext;}
.xl34
	{mso-style-parent:style0;
	font-size:9.0pt;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl35
	{mso-style-parent:style0;
	font-size:9.0pt;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl36
	{mso-style-parent:style0;
	font-size:9.0pt;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid black;}
.xl37
	{mso-style-parent:style0;
	font-size:9.0pt;
	text-align:center;
	border:.5pt solid windowtext;}
.xl38
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid black;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl39
	{mso-style-parent:style0;
	border-top:none;
	border-right:none;
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
    <x:Name>阜康市西沟煤焦有限责任公司考勤月报表</x:Name>
    <x:WorksheetOptions>
     <x:DefaultRowHeight>285</x:DefaultRowHeight>
     <x:Print>
      <x:ValidPrinterInfo/>
      <x:PaperSizeIndex>9</x:PaperSizeIndex>
      <x:HorizontalResolution>600</x:HorizontalResolution>
      <x:VerticalResolution>0</x:VerticalResolution>
     </x:Print>
     <x:Selected/>
     <x:Panes>
      <x:Pane>
       <x:Number>3</x:Number>
       <x:ActiveRow>11</x:ActiveRow>
       <x:ActiveCol>18</x:ActiveCol>
      </x:Pane>
     </x:Panes>
     <x:ProtectContents>False</x:ProtectContents>
     <x:ProtectObjects>False</x:ProtectObjects>
     <x:ProtectScenarios>False</x:ProtectScenarios>
    </x:WorksheetOptions>
    <x:Watches>
     <x:Watch>
      <x:Source>$C$5</x:Source>
     </x:Watch>
    </x:Watches>
   </x:ExcelWorksheet>
  </x:ExcelWorksheets>
  <x:WindowHeight>3885</x:WindowHeight>
  <x:WindowWidth>17175</x:WindowWidth>
  <x:WindowTopX>525</x:WindowTopX>
  <x:WindowTopY>1065</x:WindowTopY>
  <x:ProtectStructure>False</x:ProtectStructure>
  <x:ProtectWindows>False</x:ProtectWindows>
 </x:ExcelWorkbook>
</xml><![endif]-->
</head>

<body link=blue vlink=purple>

<table x:str border=0 cellpadding=0 cellspacing=0 width=2003 style='border-collapse:
 collapse;table-layout:fixed;width:1506pt'>
 <col width=45 style='mso-width-source:userset;mso-width-alt:1440;width:34pt'>
 <col width=101 style='mso-width-source:userset;mso-width-alt:3232;width:76pt'>
 <col width=56 span=4 style='mso-width-source:userset;mso-width-alt:1792;
 width:42pt'>
 <col width=37 span=2 style='mso-width-source:userset;mso-width-alt:1184;
 width:28pt'>
 <col width=69 style='mso-width-source:userset;mso-width-alt:2208;width:52pt'>
 <col width=37 span=3 style='mso-width-source:userset;mso-width-alt:1184;
 width:28pt'>
 <col width=64 style='mso-width-source:userset;mso-width-alt:2048;width:48pt'>
 <col width=69 style='mso-width-source:userset;mso-width-alt:2208;width:52pt'>
 <col width=61 span=3 style='mso-width-source:userset;mso-width-alt:1952;
 width:46pt'>
 <col width=66 style='mso-width-source:userset;mso-width-alt:2112;width:50pt'>
 <col width=61 style='mso-width-source:userset;mso-width-alt:1952;width:46pt'>
 <col width=72 span=13 style='width:54pt'>
 <tr height=19 style='height:14.25pt'>
  <td colspan=20 height=19 class=xl30 width=1139 style='height:14.25pt;
  width:858pt'>阜康市西沟煤焦有限责任公司考勤月报表</td>
  <td class=xl25 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
 </tr>
  <%
     System.DateTime beginTime = Convert.ToDateTime(Request.QueryString["beginTime"]);
     System.DateTime endTime = Convert.ToDateTime(Request.QueryString["endTime"]);
 %>
 <tr height=19 style='height:14.25pt'>
  <td colspan=20 height=19 class=xl31 style='height:14.25pt'>开始时间：<%=beginTime.ToString("yyyy/MM/dd")%>
  截止时间：<%=endTime.ToString("yyyy/MM/dd")%></td>
  <td class=xl25></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td rowspan=2 height=38 class=xl32 style='border-bottom:.5pt solid black;
  height:28.5pt;border-top:none'>序号</td>
  <td rowspan=2 class=xl32 style='border-bottom:.5pt solid black;border-top:
  none'>部门</td>
  <td rowspan=2 class=xl32 style='border-bottom:.5pt solid black;border-top:
  none'>姓名</td>
  <td rowspan=2 class=xl32 style='border-bottom:.5pt solid black;border-top:
  none'>岗位</td>
  <td rowspan=2 class=xl32 style='border-bottom:.5pt solid black;border-top:
  none'>工号</td>
  <td rowspan=2 class=xl32 style='border-bottom:.5pt solid black;border-top:
  none'>工资形式</td>
  <td colspan=9 class=xl35 style='border-right:.5pt solid black;border-left:
  none'>请假</td>
  <td colspan=4 class=xl36 style='border-left:none'>出勤时间</td>
  <td rowspan=2 class=xl37 style='border-top:none'>考核分数</td>
  <td class=xl25></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 class=xl26 style='height:14.25pt'>事假</td>
  <td class=xl26>病假</td>
  <td class=xl26>其它无薪假</td>
  <td class=xl26>婚假</td>
  <td class=xl26>产假</td>
  <td class=xl26>丧假</td>
  <td class=xl26>调休/年休</td>
  <td class=xl26>其它有薪假</td>
  <td class=xl26>请假合计</td>
  <td class=xl28 style='border-left:none'>平时上班</td>
  <td class=xl26>周末加班</td>
  <td class=xl26>节假日加班</td>
  <td class=xl27>工作时长</td>
  <td class=xl25></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
  <%        
     for (int i = 0; i < DomainServiceIriskingAttend.XiGouMonthAttendReportList.Count; i++)
     {
%>
 <tr height=19 style='height:14.25pt'>
  <td height=19 class=xl29 style='height:14.25pt'><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].Index%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].DepartName%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].PersonName%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].PrincipalName%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].WorkSn%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].WorkType%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].LeaveTypeName[0]%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].LeaveTypeName[1]%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].LeaveTypeName[2]%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].LeaveTypeName[3]%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].LeaveTypeName[4]%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].LeaveTypeName[5]%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].LeaveTypeName[6]%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].LeaveTypeName[7]%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].SumLeave%></td>
  <td class=xl28><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].WeekendType[0]%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].WeekendType[1]%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].WeekendType[2]%></td>
  <td class=xl26><%=DomainServiceIriskingAttend.XiGouMonthAttendReportList[i].WorkTime%></td>
  <td class=xl26>　</td>
  <td class=xl25></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <%        
     }        
%>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=20 class=xl25 style='mso-ignore:colspan'></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=20 class=xl25 style='mso-ignore:colspan'></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=20 class=xl25 style='mso-ignore:colspan'></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=20 class=xl25 style='mso-ignore:colspan'></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=20 class=xl25 style='mso-ignore:colspan'></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=20 class=xl25 style='mso-ignore:colspan'></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=20 class=xl25 style='mso-ignore:colspan'></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=20 class=xl25 style='mso-ignore:colspan'></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=20 class=xl25 style='mso-ignore:colspan'></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=20 class=xl25 style='mso-ignore:colspan'></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=20 class=xl25 style='mso-ignore:colspan'></td>
  <td colspan=11 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=31 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=31 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=31 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=31 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=31 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=19 style='height:14.25pt'>
  <td height=19 style='height:14.25pt'></td>
  <td colspan=31 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <![if supportMisalignedColumns]>
 <tr height=0 style='display:none'>
  <td width=45 style='width:34pt'></td>
  <td width=101 style='width:76pt'></td>
  <td width=56 style='width:42pt'></td>
  <td width=56 style='width:42pt'></td>
  <td width=56 style='width:42pt'></td>
  <td width=56 style='width:42pt'></td>
  <td width=37 style='width:28pt'></td>
  <td width=37 style='width:28pt'></td>
  <td width=69 style='width:52pt'></td>
  <td width=37 style='width:28pt'></td>
  <td width=37 style='width:28pt'></td>
  <td width=37 style='width:28pt'></td>
  <td width=64 style='width:48pt'></td>
  <td width=69 style='width:52pt'></td>
  <td width=61 style='width:46pt'></td>
  <td width=61 style='width:46pt'></td>
  <td width=61 style='width:46pt'></td>
  <td width=66 style='width:50pt'></td>
  <td width=61 style='width:46pt'></td>
  <td width=72 style='width:54pt'></td>
  <td width=72 style='width:54pt'></td>
  <td width=72 style='width:54pt'></td>
  <td width=72 style='width:54pt'></td>
  <td width=72 style='width:54pt'></td>
  <td width=72 style='width:54pt'></td>
  <td width=72 style='width:54pt'></td>
  <td width=72 style='width:54pt'></td>
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
