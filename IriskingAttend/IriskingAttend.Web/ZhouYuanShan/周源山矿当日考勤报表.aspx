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
<link rel=File-List href="周源山表2_files/filelist.xml">
<link rel=Edit-Time-Data href="周源山表2_files/editdata.mso">
<link rel=OLE-Object-Data href="周源山表2_files/oledata.mso">
<!--[if gte mso 9]><xml>
 <o:DocumentProperties>
  <o:Author>YuHailong</o:Author>
  <o:LastAuthor>微软用户</o:LastAuthor>
  <o:LastPrinted>2014-01-08T09:37:34Z</o:LastPrinted>
  <o:Created>2008-01-10T02:24:54Z</o:Created>
  <o:LastSaved>2014-01-08T09:38:37Z</o:LastSaved>
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
	mso-footer-margin:.5in;
	mso-page-orientation:landscape;}
.font5
	{color:windowtext;
	font-size:12.0pt;
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
.style128
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
	mso-style-name:"常规 2";}
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
	{mso-style-parent:style128;
	white-space:nowrap;
	mso-text-control:shrinktofit;}
.xl70
	{mso-style-parent:style128;
	font-size:18.0pt;
	font-weight:700;
	text-align:center;
	border:.5pt solid windowtext;
	white-space:nowrap;
	mso-text-control:shrinktofit;}
.xl71
	{mso-style-parent:style128;
	font-weight:700;
	text-align:center;
	border:.5pt solid windowtext;}
.xl72
	{mso-style-parent:style128;
	font-size:8.0pt;
	font-weight:700;
	text-align:center;
	border:.5pt solid windowtext;
	white-space:nowrap;
	mso-text-control:shrinktofit;}
.xl73
	{mso-style-parent:style128;
	font-size:8.0pt;
	text-align:center;
	border:.5pt solid windowtext;
	white-space:nowrap;
	mso-text-control:shrinktofit;}
.xl74
	{mso-style-parent:style128;
	border:.5pt solid windowtext;
	white-space:nowrap;
	mso-text-control:shrinktofit;}
.xl75
	{mso-style-parent:style128;
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
    <x:Name>（二）周源山矿当日考勤报表</x:Name>
    <x:WorksheetOptions>
     <x:DefaultRowHeight>285</x:DefaultRowHeight>
     <x:Print>
      <x:ValidPrinterInfo/>
      <x:PaperSizeIndex>9</x:PaperSizeIndex>
      <x:HorizontalResolution>600</x:HorizontalResolution>
      <x:VerticalResolution>600</x:VerticalResolution>
     </x:Print>
     <x:Selected/>
     <x:TopRowVisible>1</x:TopRowVisible>
     <x:Panes>
      <x:Pane>
       <x:Number>3</x:Number>
       <x:ActiveRow>33</x:ActiveRow>
       <x:ActiveCol>3</x:ActiveCol>
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
</xml><![endif]-->
</head>

<body link=blue vlink=purple class=xl69>

<table x:str border=0 cellpadding=0 cellspacing=0 width=953 style='border-collapse:
 collapse;table-layout:fixed;width:716pt'>
 <col class=xl69 width=82 style='mso-width-source:userset;mso-width-alt:2624;
 width:62pt'>
 <col class=xl69 width=126 style='mso-width-source:userset;mso-width-alt:4032;
 width:95pt'>
 <col class=xl69 width=75 style='mso-width-source:userset;mso-width-alt:2400;
 width:56pt'>
 <col class=xl69 width=71 style='mso-width-source:userset;mso-width-alt:2272;
 width:53pt'>
 <col class=xl69 width=87 style='mso-width-source:userset;mso-width-alt:2784;
 width:65pt'>
 <col class=xl69 width=180 style='mso-width-source:userset;mso-width-alt:5760;
 width:135pt'>
 <col class=xl69 width=188 style='mso-width-source:userset;mso-width-alt:6016;
 width:141pt'>
 <col class=xl69 width=74 style='mso-width-source:userset;mso-width-alt:2368;
 width:56pt'>
 <col class=xl69 width=70 style='mso-width-source:userset;mso-width-alt:2240;
 width:53pt'>
 <tr height=0 style='display:none'>
  <td class=xl69 width=82 style='width:62pt'></td>
  <td class=xl69 width=126 style='width:95pt'></td>
  <td class=xl69 width=75 style='width:56pt'></td>
  <td class=xl69 width=71 style='width:53pt'></td>
  <td class=xl69 width=87 style='width:65pt'></td>
  <td class=xl69 width=180 style='width:135pt'></td>
  <td class=xl69 width=188 style='width:141pt'></td>
  <td class=xl69 width=74 style='width:56pt'></td>
  <td class=xl69 width=70 style='width:53pt'></td>
 </tr>
 <tr height=49 style='mso-height-source:userset;height:36.75pt'>
  <td colspan=9 height=49 class=xl70 style='height:36.75pt'>周源山矿当日考勤报表</td>
 </tr>
 <%
     System.DateTime beginTime = Convert.ToDateTime(Request.QueryString["beginTime"]);     
 %>
 <tr height=24 style='mso-height-source:userset;height:18.6pt'>
  <td colspan=9 height=24 class=xl71 style='height:18.6pt'>开始时间：<%=beginTime.ToString("yyyy-MM-dd:00:00:00") %>
  <span style='mso-spacerun:yes'>&nbsp;&nbsp;&nbsp;
  </span>截止时间：<%=beginTime.ToString("yyyy-MM-dd:23:59:59")%></td>
 </tr>
 <tr height=23 style='mso-height-source:userset;height:17.25pt'>
  <td height=23 class=xl72 style='height:17.25pt;border-top:none'>日期</td>
  <td class=xl72 style='border-top:none;border-left:none'>部门名称</td>
  <td class=xl72 style='border-top:none;border-left:none'>人员编码</td>
  <td class=xl72 style='border-top:none;border-left:none'>姓名</td>
  <td class=xl72 style='border-top:none;border-left:none'>职位</td>
  <td class=xl72 style='border-top:none;border-left:none'>入井时间</td>
  <td class=xl72 style='border-top:none;border-left:none'>升井时间</td>
  <td class=xl72 style='border-top:none;border-left:none'>工时</td>
  <td class=xl72 style='border-top:none;border-left:none'>记工</td>
 </tr>
 <%    
     for (int i = 0; i < DomainServiceIriskingAttend.DayPersonAttendList.Count; i++)
     {         
%>
 <tr height=23 style='mso-height-source:userset;height:17.85pt'>
  <td height=23 class=xl73 style='height:17.85pt;border-top:none'><% =DomainServiceIriskingAttend.DayPersonAttendList[i].AttendDay%></td>
  <td class=xl73 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DayPersonAttendList[i].DepartName%></td>
  <td class=xl73 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DayPersonAttendList[i].WorkSn%></td>
  <td class=xl73 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DayPersonAttendList[i].PersonName%></td>
  <td class=xl73 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DayPersonAttendList[i].Principal%></td>
  <td class=xl73 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DayPersonAttendList[i].InWellTime%></td>
  <td class=xl73 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DayPersonAttendList[i].OutWellTime%></td>
  <td class=xl73 style='border-top:none;border-left:none'><%=string.Format("{0}:{1}",
                 DomainServiceIriskingAttend.DayPersonAttendList[i].WorkTime / 60,
                 DomainServiceIriskingAttend.DayPersonAttendList[i].WorkTime % 60)%></td>
  <td class=xl73 style='border-top:none;border-left:none'><% =DomainServiceIriskingAttend.DayPersonAttendList[i].WorkCnt%></td>
 </tr>
<%
     }
%> 
 <tr height=0 style='display:none'>
  <td class=xl74 style='border-top:none'>　</td>
  <td class=xl74 style='border-top:none;border-left:none'>　</td>
  <td class=xl74 style='border-top:none;border-left:none'>　</td>
  <td class=xl74 style='border-top:none;border-left:none'>　</td>
  <td class=xl74 style='border-top:none;border-left:none'>　</td>
  <td class=xl74 style='border-top:none;border-left:none'>　</td>
  <td class=xl74 style='border-top:none;border-left:none'>　</td>
  <td class=xl74 style='border-top:none;border-left:none'>　</td>
  <td class=xl74 style='border-top:none;border-left:none'>　</td>
 </tr>
 <tr height=19 style='height:14.25pt'>
 </tr>
 <tr height=19 style='height:14.25pt'>
 </tr>
 <tr height=19 style='height:14.25pt'>
 </tr>
 <tr height=19 style='height:14.25pt'>
 </tr>
 <tr height=19 style='height:14.25pt'>
 </tr>
 <tr height=19 style='height:14.25pt'>
 </tr>
 <tr height=19 style='height:14.25pt'>
 </tr>
 <tr height=19 style='height:14.25pt'>
 </tr>
 <tr height=19 style='height:14.25pt'>
 </tr>
 <![if supportMisalignedColumns]>
 <tr height=0 style='display:none'>
  <td width=82 style='width:62pt'></td>
  <td width=126 style='width:95pt'></td>
  <td width=75 style='width:56pt'></td>
  <td width=71 style='width:53pt'></td>
  <td width=87 style='width:65pt'></td>
  <td width=180 style='width:135pt'></td>
  <td width=188 style='width:141pt'></td>
  <td width=74 style='width:56pt'></td>
  <td width=70 style='width:53pt'></td>
 </tr>
 <![endif]>
</table>

</body>

</html>
