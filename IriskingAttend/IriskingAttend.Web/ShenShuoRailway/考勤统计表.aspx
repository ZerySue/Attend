<%@ Page Language="C#" EnableEventValidation = "false" ResponseEncoding="gb2312" ContentType="application/vnd.ms-excel" %>
<%@ Import Namespace="IriskingAttend.Web" %>
<%@ Import Namespace="System" %>
<html xmlns:v="urn:schemas-microsoft-com:vml"
xmlns:o="urn:schemas-microsoft-com:office:office"
xmlns:x="urn:schemas-microsoft-com:office:excel"
xmlns="http://www.w3.org/TR/REC-html40">

<head>
<meta http-equiv=Content-Type content="text/html; charset=gb2312">
<meta name=ProgId content=Excel.Sheet>
<meta name=Generator content="Microsoft Excel 11">
<link rel=File-List href="考勤统计表_files/filelist.xml">
<link rel=Edit-Time-Data href="考勤统计表_files/editdata.mso">
<link rel=OLE-Object-Data href="考勤统计表_files/oledata.mso">
<style>
<!--table
	{mso-displayed-decimal-separator:"\.";
	mso-displayed-thousand-separator:"\,";}
@page
	{mso-footer-data:"&L&9备注：正常=\0022+\0022\,未签到=\0022\[\0022\,未签退=\0022\]\0022\,旷工=\0022-\0022\,迟到=\0022>\0022\,早退=\0022<\0022\,加班=\0022加\0022\,公出=\0022差\0022\,产假=\0022产\0022\000A      护理假=\0022护\0022\,病假=\0022病\0022\,事假=\0022事\0022\,探亲假=\0022探\0022\,年休假=\0022年\0022\,助勤=\0022助\0022\,调休=\0022调\0022\,婚假=\0022婚\0022";
	margin:.51in .2in .71in .39in;
	mso-header-margin:.28in;
	mso-footer-margin:.2in;
	mso-page-orientation:landscape;}
.font7
	{color:black;
	font-size:20.0pt;
	font-weight:700;
	font-style:normal;
	text-decoration:none;
	font-family:宋体;
	mso-generic-font-family:auto;
	mso-font-charset:134;}
.font8
	{color:black;
	font-size:12.0pt;
	font-weight:700;
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
	color:black;
	font-size:11.0pt;}
.xl25
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl26
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl27
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	white-space:normal;}
.xl28
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	vertical-align:top;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:none;
	white-space:normal;}
.xl29
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	vertical-align:top;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:none;
	background:white;
	mso-pattern:black none;
	white-space:normal;}
.xl30
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	vertical-align:top;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:none;
	background:#969696;
	mso-pattern:black none;
	white-space:normal;}
.xl31
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	vertical-align:top;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:none;
	background:yellow;
	mso-pattern:black none;
	white-space:normal;}
.xl32
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	vertical-align:top;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	white-space:normal;}
.xl33
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	vertical-align:top;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:white;
	mso-pattern:black none;
	white-space:normal;}
.xl34
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	vertical-align:top;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:#969696;
	mso-pattern:black none;
	white-space:normal;}
.xl35
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	vertical-align:top;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	background:yellow;
	mso-pattern:black none;
	white-space:normal;}
.xl36
	{mso-style-parent:style0;
	color:black;
	font-size:20.0pt;
	font-weight:700;
	mso-number-format:"\@";
	text-align:center;
	white-space:normal;}
.xl37
	{mso-style-parent:style0;
	color:black;
	font-size:11.0pt;
	font-weight:700;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl38
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:.5pt solid windowtext;
	white-space:nowrap;
	mso-text-control:shrinktofit;}
.xl39
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid black;
	border-left:.5pt solid windowtext;
	white-space:nowrap;
	mso-text-control:shrinktofit;}
.xl40
	{mso-style-parent:style0;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border:.5pt solid windowtext;}
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
    <x:Name>考勤统计表</x:Name>
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
       <x:ActiveRow>2</x:ActiveRow>
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
  <x:Formula>=考勤统计表!$1:$3</x:Formula>
 </x:ExcelName>
</xml><![endif]--><!--[if gte mso 9]><xml>
 <o:shapedefaults v:ext="edit" spidmax="1025"/>
</xml><![endif]-->
</head>

<body link=blue vlink=purple>
<!--下列信息由 Microsoft Office Excel 的“发布为网页”向导生成。--><!--如果同一条目从 Excel 中重新发布，则所有位于 DIV 标记之间的信息均将被替换。--><!-----------------------------><!--“从 EXCEL 发布网页”向导开始--><!----------------------------->

<table x:str border=0 cellpadding=0 cellspacing=0 width=1572 style='border-collapse:
 collapse;table-layout:fixed;width:1182pt'>
 <col width=77 style='mso-width-source:userset;mso-width-alt:2464;width:58pt'>
 <col width=65 style='mso-width-source:userset;mso-width-alt:2080;width:49pt'>
 <col width=30 style='mso-width-source:userset;mso-width-alt:960;width:23pt'>
 <col width=24 span=31 style='mso-width-source:userset;mso-width-alt:768;
 width:18pt'>
 <col width=33 span=3 style='mso-width-source:userset;mso-width-alt:1056;
 width:25pt'>
 <col width=25 span=5 style='mso-width-source:userset;mso-width-alt:800;
 width:19pt'>
 <col class=xl24 width=72 span=6 style='width:54pt'>
 <%
     string departName = "";
     for (int index = 0; index < DomainServiceIriskingAttend.DepartName.Count(); index++)
     {
         departName += string.Format( "{0},", DomainServiceIriskingAttend.DepartName[index]);
     }
     departName = departName.Remove( departName.LastIndexOf(","), 1);

     System.DateTime beginTime = Convert.ToDateTime(Request.QueryString["beginTime"]);
     System.DateTime endTime = Convert.ToDateTime(Request.QueryString["endTime"]);
 %>
 <tr height=40 style='mso-height-source:userset;height:30.0pt'>
  <td colspan=42 height=40 class=xl36 width=1140 style='height:30.0pt;
  width:858pt'>考勤统计表 <font class="font8">(<%=departName%>)</font><font class="font7"><span
  style='mso-spacerun:yes'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></font></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
  <td class=xl24 width=72 style='width:54pt'></td>
 </tr>
 <tr height=40 style='mso-height-source:userset;height:30.0pt'>
  <td colspan=42 height=40 class=xl37 style='height:30.0pt'>考勤周期：<%=beginTime.ToString("yyyy.MM.dd")%>至<%=endTime.AddDays(-1).ToString("yyyy.MM.dd")%></td>
  <td colspan=6 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=50 style='mso-height-source:userset;height:37.5pt'>
  <td height=50 class=xl25 style='height:37.5pt'>部门</td>  
  <td class=xl40>姓名</td>
  <td class=xl26>班次</td>
<%
     string[] Day = new string[] { "日", "一", "二", "三", "四", "五", "六" };
     
     for( int index = 0; index < 31; index++ )
     {
%>
  <td class=xl27 width=24 style='width:18pt'><%=beginTime.AddDays(index).ToString("dd")%><br>
    <%= Day[(Int32)beginTime.AddDays(index).DayOfWeek]%></td>
<%
     }
%>
  <td class=xl27 width=33 style='width:25pt'>应<br>
    到</td>
  <td class=xl27 width=33 style='width:25pt'>实<br>
    到</td>
  <td class=xl27 width=33 style='width:25pt'>未<br>
    签</td>
  <td class=xl27 width=25 style='width:19pt'>迟<br>
    到</td>
  <td class=xl27 width=25 style='width:19pt'>早<br>
    退</td>
  <td class=xl27 width=25 style='width:19pt'>加<br>
    班</td>
  <td class=xl27 width=25 style='width:19pt'>请<br>
    假</td>
  <td class=xl27 width=25 style='width:19pt'>公<br>
    出</td>
  <td colspan=6 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
  <% 
    int NormalDay = 0;
    int ShiftDay = 1;
    int WeekendDay = 2;
    int FestivalDay = 3; //&lt; <  &gt;>    
    for (int i = 0; i < DomainServiceIriskingAttend.TotalAttendList.Count; i++)
    {         
%>
 <tr height=20 style='mso-height-source:userset;height:15.0pt'>
  <td rowspan=2 height=40 class=xl38 style='border-bottom:.5pt solid black;
  height:30.0pt;border-top:none'><%=DomainServiceIriskingAttend.TotalAttendList[i].DepartName %></td>  
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:none'><%=DomainServiceIriskingAttend.TotalAttendList[i].PersonName %></td>
  <td class=xl28 width=30 style='width:23pt'>上午</td>
<%   
        int dayOffset =0;
        for (; dayOffset < DomainServiceIriskingAttend.TotalAttendList[i].DayType.Count(); dayOffset++)
        {
            DomainServiceIriskingAttend.TotalAttendList[i].MoringSignal[dayOffset] =
                        DomainServiceIriskingAttend.TotalAttendList[i].MoringSignal[dayOffset].Replace("<", @"&lt;");
            DomainServiceIriskingAttend.TotalAttendList[i].MoringSignal[dayOffset] =
                        DomainServiceIriskingAttend.TotalAttendList[i].MoringSignal[dayOffset].Replace(">", @"&gt;");
            
            DomainServiceIriskingAttend.TotalAttendList[i].AfternoonSignal[dayOffset] =
                       DomainServiceIriskingAttend.TotalAttendList[i].AfternoonSignal[dayOffset].Replace("<", @"&lt;");
            DomainServiceIriskingAttend.TotalAttendList[i].AfternoonSignal[dayOffset] =
                        DomainServiceIriskingAttend.TotalAttendList[i].AfternoonSignal[dayOffset].Replace(">", @"&gt;");

            if (DomainServiceIriskingAttend.TotalAttendList[i].DayType[dayOffset] < WeekendDay)
            {         
%>  
  <td class=xl29 width=24 style='width:18pt'><%=DomainServiceIriskingAttend.TotalAttendList[i].MoringSignal[dayOffset].ToString()%></td>
<%
            }
            if (DomainServiceIriskingAttend.TotalAttendList[i].DayType[dayOffset] == WeekendDay)
            {         
%>  
  <td class=xl30 width=24 style='width:18pt'><%=DomainServiceIriskingAttend.TotalAttendList[i].MoringSignal[dayOffset].ToString()%></td>
<%
            }
            if (DomainServiceIriskingAttend.TotalAttendList[i].DayType[dayOffset] == FestivalDay)
            {         
%>
  <td class=xl31 width=24 style='width:18pt'><%=DomainServiceIriskingAttend.TotalAttendList[i].MoringSignal[dayOffset].ToString()%></td>
<%
            } 
         }
      
         for(; dayOffset < 31; dayOffset++)  
         {      
%>  
  <td class=xl29 width=24 style='width:18pt'>　</td>
<%
         }
%> 
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:none'><%=DomainServiceIriskingAttend.TotalAttendList[i].SupposeNum %></td>
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:none'><%=DomainServiceIriskingAttend.TotalAttendList[i].ActualNum %></td>
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:none'><%=DomainServiceIriskingAttend.TotalAttendList[i].AbsentNum %></td>
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:none'><%=DomainServiceIriskingAttend.TotalAttendList[i].LateNum %></td>
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:none'><%=DomainServiceIriskingAttend.TotalAttendList[i].LeaveEarlyNum %></td>
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:none'><%=DomainServiceIriskingAttend.TotalAttendList[i].ExtraNum %></td>
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:none'><%=DomainServiceIriskingAttend.TotalAttendList[i].AskLeaveNum %></td>
  <td rowspan=2 class=xl38 style='border-bottom:.5pt solid black;border-top:none'><%=DomainServiceIriskingAttend.TotalAttendList[i].BusinessNum %></td>
  <td colspan=6 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
 <tr height=20 style='mso-height-source:userset;height:15.0pt'>
  <td height=20 class=xl32 width=30 style='height:15.0pt;width:23pt'>下午</td>
<%   
        dayOffset =0;
        for (; dayOffset < DomainServiceIriskingAttend.TotalAttendList[i].DayType.Count(); dayOffset++)
        {
            if (DomainServiceIriskingAttend.TotalAttendList[i].DayType[dayOffset] < WeekendDay)
            {         
%>  
  <td class=xl33 width=24 style='width:18pt'><%=DomainServiceIriskingAttend.TotalAttendList[i].AfternoonSignal[dayOffset].ToString()%></td>
<%
            }
            if (DomainServiceIriskingAttend.TotalAttendList[i].DayType[dayOffset] == WeekendDay)
            {         
%> 
  <td class=xl34 width=24 style='width:18pt'><%=DomainServiceIriskingAttend.TotalAttendList[i].AfternoonSignal[dayOffset].ToString()%></td>
<%
            }
            if (DomainServiceIriskingAttend.TotalAttendList[i].DayType[dayOffset] == FestivalDay)
            {         
%>  
  <td class=xl35 width=24 style='width:18pt'><%=DomainServiceIriskingAttend.TotalAttendList[i].AfternoonSignal[dayOffset].ToString()%></td>
<%
            } 
         }
      
         for(; dayOffset < 31; dayOffset++)  
         {      
%>  
  <td class=xl33 width=24 style='width:18pt'>　</td>
<%
         }
%> 
  <td colspan=6 class=xl24 style='mso-ignore:colspan'></td>
 </tr>
<%
    }   
%>  
 <![if supportMisalignedColumns]>
 <tr height=0 style='display:none'>
  <td width=77 style='width:58pt'></td>
  <td width=65 style='width:49pt'></td>
  <td width=30 style='width:23pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=24 style='width:18pt'></td>
  <td width=33 style='width:25pt'></td>
  <td width=33 style='width:25pt'></td>
  <td width=33 style='width:25pt'></td>
  <td width=25 style='width:19pt'></td>
  <td width=25 style='width:19pt'></td>
  <td width=25 style='width:19pt'></td>
  <td width=25 style='width:19pt'></td>
  <td width=25 style='width:19pt'></td>
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
