<%@ Page Language="C#" EnableEventValidation="false" ResponseEncoding="gb2312" ContentType="application/vnd.ms-excel" %>

<%@ Import Namespace="IriskingAttend.Web" %>
<%@ Import Namespace="System" %>
<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel"
xmlns="http://www.w3.org/TR/REC-html40">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312">
    <meta name="ProgId" content="Excel.Sheet">
    <meta name="Generator" content="Microsoft Excel 11">
    <link rel="File-List" href="月考勤报表_files/filelist.xml">
    <link rel="Edit-Time-Data" href="月考勤报表_files/editdata.mso">
    <link rel="OLE-Object-Data" href="月考勤报表_files/oledata.mso">
    <!--[if gte mso 9]><xml>
 <o:DocumentProperties>
  <o:LastAuthor>cai</o:LastAuthor>
  <o:LastPrinted>2014-02-12T06:29:32Z</o:LastPrinted>
  <o:LastSaved>2014-10-22T09:00:20Z</o:LastSaved>
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
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:none;
	border-left:.5pt solid windowtext;}
.xl25
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:none;
	border-left:none;}
.xl26
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid black;
	border-bottom:none;
	border-left:none;}
.xl27
	{mso-style-parent:style0;
	text-align:center;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl28
	{mso-style-parent:style0;
	text-align:center;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl29
	{mso-style-parent:style0;
	text-align:center;
	border-top:none;
	border-right:.5pt solid black;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl30
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl31
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl32
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid black;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl33
	{mso-style-parent:style0;
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:.5pt solid windowtext;}
.xl34
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:.5pt solid windowtext;}
.xl35
	{mso-style-parent:style0;
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid black;
	border-left:.5pt solid windowtext;}
.xl36
	{mso-style-parent:style0;
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:.5pt solid windowtext;
	white-space:normal;}
.xl37
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:.5pt solid windowtext;
	white-space:normal;}
.xl38
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:none;}
.xl39
	{mso-style-parent:style0;
	text-align:center;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid black;
	border-left:.5pt solid windowtext;}
.xl40
	{mso-style-parent:style0;
	text-align:center;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid black;
	border-left:none;}
.xl41
	{mso-style-parent:style0;
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid black;
	border-left:none;}
.xl42
	{mso-style-parent:style0;
	text-align:center;
	border-top:.5pt solid black;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:.5pt solid windowtext;}
.xl43
	{mso-style-parent:style0;
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
    <x:Name>月考勤报表</x:Name>
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
     <x:LeftColumnVisible>18</x:LeftColumnVisible>
     <x:Panes>
      <x:Pane>
       <x:Number>3</x:Number>
       <x:ActiveRow>21</x:ActiveRow>
       <x:ActiveCol>55</x:ActiveCol>
      </x:Pane>
     </x:Panes>
     <x:ProtectContents>False</x:ProtectContents>
     <x:ProtectObjects>False</x:ProtectObjects>
     <x:ProtectScenarios>False</x:ProtectScenarios>
    </x:WorksheetOptions>
    <x:Sorting>
     <x:Sort>Column AC</x:Sort>
    </x:Sorting>
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
  <x:Formula>=月考勤报表!$4:$6</x:Formula>
 </x:ExcelName>
</xml><![endif]-->
</head>
<body link="blue" vlink="purple">
    <table x:str border="0" cellpadding="0" cellspacing="0" width="2445" style='border-collapse: collapse;
        table-layout: fixed; width: 1844pt'>
        <col width="77" style='mso-width-source: userset; mso-width-alt: 2464; width: 58pt'>
        <col width="90" style='mso-width-source: userset; mso-width-alt: 2880; width: 68pt'>
         <col width=200 style='mso-width-source:userset;mso-width-alt:6400;width:150pt'>
        <col width="70" style='mso-width-source: userset; mso-width-alt: 2240; width: 53pt'>
        <col width="40" style='mso-width-source: userset; mso-width-alt: 1280; width: 30pt'>
        <col width="57" style='mso-width-source: userset; mso-width-alt: 1824; width: 43pt'>
        <col width="52" style='mso-width-source: userset; mso-width-alt: 1664; width: 39pt'>
        <col width="34" style='mso-width-source: userset; mso-width-alt: 1088; width: 26pt'>
        <col width="45" span="2" style='mso-width-source: userset; mso-width-alt: 1440; width: 34pt'>
        <col width="46" span="2" style='mso-width-source: userset; mso-width-alt: 1472; width: 35pt'>
        <col width="45" span="3" style='mso-width-source: userset; mso-width-alt: 1440; width: 34pt'>
        <col width="46" span="2" style='mso-width-source: userset; mso-width-alt: 1472; width: 35pt'>
        <col width="43" style='mso-width-source: userset; mso-width-alt: 1376; width: 32pt'>
        <col width="40" style='mso-width-source: userset; mso-width-alt: 1280; width: 30pt'>
        <col width="43" style='mso-width-source: userset; mso-width-alt: 1376; width: 32pt'>
        <col width="41" style='mso-width-source: userset; mso-width-alt: 1312; width: 31pt'>
        <col width="45" style='mso-width-source: userset; mso-width-alt: 1440; width: 34pt'>
        <col width="42" style='mso-width-source: userset; mso-width-alt: 1344; width: 32pt'>
        <col width="47" style='mso-width-source: userset; mso-width-alt: 1504; width: 35pt'>
        <col width="46" style='mso-width-source: userset; mso-width-alt: 1472; width: 35pt'>
        <col width="36" style='mso-width-source: userset; mso-width-alt: 1152; width: 27pt'>
        <col width="38" style='mso-width-source: userset; mso-width-alt: 1216; width: 29pt'>
        <col width="44" style='mso-width-source: userset; mso-width-alt: 1408; width: 33pt'>
        <col width="40" style='mso-width-source: userset; mso-width-alt: 1280; width: 30pt'>
        <col width="35" style='mso-width-source: userset; mso-width-alt: 1120; width: 26pt'>
        <col width="34" style='mso-width-source: userset; mso-width-alt: 1088; width: 26pt'>
        <col width="33" style='mso-width-source: userset; mso-width-alt: 1056; width: 25pt'>
        <col width="34" style='mso-width-source: userset; mso-width-alt: 1088; width: 26pt'>
        <col width="37" span="2" style='mso-width-source: userset; mso-width-alt: 1184; width: 28pt'>
        <col width="31" style='mso-width-source: userset; mso-width-alt: 992; width: 23pt'>
        <col width="34" style='mso-width-source: userset; mso-width-alt: 1088; width: 26pt'>
        <col width="33" style='mso-width-source: userset; mso-width-alt: 1056; width: 25pt'>
        <col width="32" style='mso-width-source: userset; mso-width-alt: 1024; width: 24pt'>
        <col width="35" style='mso-width-source: userset; mso-width-alt: 1120; width: 26pt'>
        <col width="30" style='mso-width-source: userset; mso-width-alt: 960; width: 23pt'>
        <col width="33" span="2" style='mso-width-source: userset; mso-width-alt: 1056; width: 25pt'>
        <col width="35" style='mso-width-source: userset; mso-width-alt: 1120; width: 26pt'>
        <col width="29" style='mso-width-source: userset; mso-width-alt: 928; width: 22pt'>
        <col width="34" style='mso-width-source: userset; mso-width-alt: 1088; width: 26pt'>
        <col width="31" style='mso-width-source: userset; mso-width-alt: 992; width: 23pt'>
        <col width="32" style='mso-width-source: userset; mso-width-alt: 1024; width: 24pt'>
        <col width="31" span="2" style='mso-width-source: userset; mso-width-alt: 992; width: 23pt'>
        <col width="33" span="2" style='mso-width-source: userset; mso-width-alt: 1056; width: 25pt'>
        <col width="30" style='mso-width-source: userset; mso-width-alt: 960; width: 23pt'>
        <col width="32" style='mso-width-source: userset; mso-width-alt: 1024; width: 24pt'>
        <col width="28" style='mso-width-source: userset; mso-width-alt: 896; width: 21pt'>
        <col width="30" style='mso-width-source: userset; mso-width-alt: 960; width: 23pt'>
        <col width="31" style='mso-width-source: userset; mso-width-alt: 992; width: 23pt'>
        <col width="32" style='mso-width-source: userset; mso-width-alt: 1024; width: 24pt'>
        <col width="35" style='mso-width-source: userset; mso-width-alt: 1120; width: 26pt'>
        <col width=200 style='mso-width-source:userset;mso-width-alt:6400;width:150pt'>
        <%
            System.DateTime beginTime = Convert.ToDateTime(Request.QueryString["beginTime"]);
            System.DateTime endTime = Convert.ToDateTime(Request.QueryString["endTime"]);
            int col = 28;
            int days = (endTime - beginTime).Days;
            bool printDay = true;
            if (days>30)
            {
                printDay = false;
                days = -1;
            }
        %>
        <tr height="19" style='height: 14.25pt'>
            <td colspan="<%=60-(30-days) %>" rowspan="2" height="38" class="xl24" width="2445" style='border-right: .5pt solid black;
                border-bottom: .5pt solid black; height: 28.5pt; width: 1844pt'>
                月考勤报表
            </td>
        </tr>
        <tr height="19" style='height: 14.25pt'>
        </tr>
      
        <tr height="19" style='height: 14.25pt'>
            <td colspan="<%=60-(30-days) %>" height="19" class="xl30" style='border-right: .5pt solid black;
                height: 14.25pt'>
                考勤周期：<%=beginTime.ToString("yyyy.MM.dd")%><span style='mso-spacerun: yes'>&nbsp;
                </span>至<%=endTime.ToString("yyyy.MM.dd")%><span style='mso-spacerun: yes'>&nbsp;</span>
            </td>
        </tr>

        <tr height="24" style='mso-height-source: userset; height: 18.0pt'>
            <td rowspan="6" height="139" class="xl34" style='height: 104.25pt; border-top: none'>
                序号
            </td>
            <td rowspan="6" class="xl37" width="90" style='border-top: none; width: 68pt'>
                内部关<br>
                联号
            </td>
            <td rowspan="6" class="xl34" style='border-top: none'>
                单位
            </td>
            <td rowspan="6" class="xl34" style='border-top: none'>
                姓名
            </td>
            <td rowspan="6" class="xl37" width="40" style='border-top: none; width: 30pt'>
                出<br>
                勤<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="57" style='border-top: none; width: 43pt'>
                出<br>
                勤<br>
                总<br>
                时<br>
                长
            </td>
            <td rowspan="6" class="xl37" width="52" style='border-top: none; width: 39pt'>
                下<br>
                井<br>
                总<br>
                时<br>
                长
            </td>
            <td rowspan="6" class="xl37" width="34" style='border-top: none; width: 26pt'>
                加<br>
                班<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="45" style='border-top: none; width: 34pt'>
                大<br>
                班<br>
                值<br>
                班<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="45" style='border-top: none; width: 34pt'>
                大<br>
                班<br>
                入<br>
                井<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="46" style='border-top: none; width: 35pt'>
                小<br>
                班<br>
                入<br>
                井<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="46" style='border-top: none; width: 35pt'>
                地<br>
                面<br>
                大<br>
                班<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="45" style='border-top: none; width: 34pt'>
                地<br>
                面<br>
                早<br>
                班<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="45" style='border-top: none; width: 34pt'>
                地<br>
                面<br>
                中<br>
                班<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="45" style='border-top: none; width: 34pt'>
                地<br>
                面<br>
                夜<br>
                班<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="46" style='border-top: none; width: 35pt'>
                井<br>
                下<br>
                早<br>
                班<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="46" style='border-top: none; width: 35pt'>
                井<br>
                下<br>
                中<br>
                班<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="43" style='border-top: none; width: 32pt'>
                井<br>
                下<br>
                夜<br>
                班<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="40" style='border-top: none; width: 30pt'>
                大<br>
                班<br>
                公<br>
                出<br>
                工<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="43" style='border-top: none; width: 32pt'>
                婚<br>
                丧<br>
                探<br>
                天<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="41" style='border-top: none; width: 31pt'>
                年<br>
                休<br>
                天<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="45" style='border-top: none; width: 34pt'>
                病<br>
                假<br>
                天<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="42" style='border-top: none; width: 32pt'>
                伤<br>
                假<br>
                天<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="47" style='border-top: none; width: 35pt'>
                事<br>
                假<br>
                天<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="46" style='border-top: none; width: 35pt'>
                旷<br>
                工<br>
                天<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="36" style='border-top: none; width: 27pt'>
                产<br>
                假<br>
                天<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="38" style='border-top: none; width: 29pt'>
                育<br>
                儿<br>
                假<br>
                天<br>
                数
            </td>
            <td rowspan="6" class="xl37" width="44" style='border-top: none; width: 33pt'>
                待<br>
                岗<br>
                学<br>
                习<br>
                天<br>
                数
            </td>
              <%if (printDay){%>
            <td colspan="<%=31-(30-days) %>" rowspan="2" class="xl24" style='border-right: .5pt solid black;
                border-bottom: .5pt solid black'>
                出勤明细
            </td>
            <%} %>
            <td rowspan="2" class="xl34" style='border-bottom: .5pt solid black; border-top: none'>
                备注
            </td>
        </tr>
        <tr height="24" style='mso-height-source: userset; height: 18.0pt'>
        </tr>
        <tr height="24" style='mso-height-source: userset; height: 18.0pt'>
            <% for (int index = 0; index <= days; index++)
               {
                   if (col <= 58 && printDay)
                   {%>
            <td rowspan="4" class="xl42" style='border-top: none' x:num>
                <%=beginTime.AddDays(index).Day.ToString("d2")%>
            </td>
            <%  }

     } %>
            <td rowspan="4" class="xl42" style='border-top: none'>
            </td>
        </tr>
        <tr height="24" style='mso-height-source: userset; height: 18.0pt'>
        </tr>
        <tr height="24" style='mso-height-source: userset; height: 18.0pt'>
        </tr>
        <tr height="19" style='height: 14.25pt'>
        </tr>
        <%
            for (int i = 0; i < DomainServiceIriskingAttend.PersonMonthAttendList.Count; i++)
            {%>
        <tr height="19" style='height: 14.25pt'>
            <td height="19" class="xl43" style='height: 14.25pt'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].Index%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].WorkSn%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].DepartName%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].PersonName%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].InOutTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].WorkTotalTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].WellTotalTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].OverTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].BigDutyTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].BigInWellTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].SmallInWellTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].GroupBigTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].GroupMorningTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].GroupMiddleTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].GroupNightTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].InWellMorningTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].InWellMiddleTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].InWellNightTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].BigClassOutTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].WeddingsTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].AnnualLeaveTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].SickLeaveTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].FakeInjuryTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].LeaveTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].AbsenteeismTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].MaternityLeaveTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].ParentalLeaveTimes%>
            </td>
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].StudyDays%>
            </td>
            <% for (int index = 0; index <= days; index++)
               {
                   if (col <= 58 && printDay)
                   {%>
           <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].ClassOrder[index]%>
            </td>
            <%  }

     } %>
            
            <td class="xl43" style='border-left: none'>
                <%=DomainServiceIriskingAttend.PersonMonthAttendList[i].Remark%>
            </td>
        </tr>
        <%}%>
        <![if supportMisalignedColumns]>
        <tr height="0" style='display: none'>
            <td width="77" style='width: 58pt'>
            </td>
            <td width="90" style='width: 68pt'>
            </td>
            <td width="61" style='width: 46pt'>
            </td>
            <td width="70" style='width: 53pt'>
            </td>
            <td width="40" style='width: 30pt'>
            </td>
            <td width="57" style='width: 43pt'>
            </td>
            <td width="52" style='width: 39pt'>
            </td>
            <td width="34" style='width: 26pt'>
            </td>
            <td width="45" style='width: 34pt'>
            </td>
            <td width="45" style='width: 34pt'>
            </td>
            <td width="46" style='width: 35pt'>
            </td>
            <td width="46" style='width: 35pt'>
            </td>
            <td width="45" style='width: 34pt'>
            </td>
            <td width="45" style='width: 34pt'>
            </td>
            <td width="45" style='width: 34pt'>
            </td>
            <td width="46" style='width: 35pt'>
            </td>
            <td width="46" style='width: 35pt'>
            </td>
            <td width="43" style='width: 32pt'>
            </td>
            <td width="40" style='width: 30pt'>
            </td>
            <td width="43" style='width: 32pt'>
            </td>
            <td width="41" style='width: 31pt'>
            </td>
            <td width="45" style='width: 34pt'>
            </td>
            <td width="42" style='width: 32pt'>
            </td>
            <td width="47" style='width: 35pt'>
            </td>
            <td width="46" style='width: 35pt'>
            </td>
            <td width="36" style='width: 27pt'>
            </td>
            <td width="38" style='width: 29pt'>
            </td>
            <td width="44" style='width: 33pt'>
            </td>
            <td width="40" style='width: 30pt'>
            </td>
            <td width="35" style='width: 26pt'>
            </td>
            <td width="34" style='width: 26pt'>
            </td>
            <td width="33" style='width: 25pt'>
            </td>
            <td width="34" style='width: 26pt'>
            </td>
            <td width="37" style='width: 28pt'>
            </td>
            <td width="37" style='width: 28pt'>
            </td>
            <td width="31" style='width: 23pt'>
            </td>
            <td width="34" style='width: 26pt'>
            </td>
            <td width="33" style='width: 25pt'>
            </td>
            <td width="32" style='width: 24pt'>
            </td>
            <td width="35" style='width: 26pt'>
            </td>
            <td width="30" style='width: 23pt'>
            </td>
            <td width="33" style='width: 25pt'>
            </td>
            <td width="33" style='width: 25pt'>
            </td>
            <td width="35" style='width: 26pt'>
            </td>
            <td width="29" style='width: 22pt'>
            </td>
            <td width="34" style='width: 26pt'>
            </td>
            <td width="31" style='width: 23pt'>
            </td>
            <td width="32" style='width: 24pt'>
            </td>
            <td width="31" style='width: 23pt'>
            </td>
            <td width="31" style='width: 23pt'>
            </td>
            <td width="33" style='width: 25pt'>
            </td>
            <td width="33" style='width: 25pt'>
            </td>
            <td width="30" style='width: 23pt'>
            </td>
            <td width="32" style='width: 24pt'>
            </td>
            <td width="28" style='width: 21pt'>
            </td>
            <td width="30" style='width: 23pt'>
            </td>
            <td width="31" style='width: 23pt'>
            </td>
            <td width="32" style='width: 24pt'>
            </td>
            <td width="35" style='width: 26pt'>
            </td>
            <td width="72" style='width: 54pt'>
            </td>
        </tr>
        <![endif]>
    </table>
</body>
</html>
