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
<link rel=File-List href="周源山表3_files/filelist.xml">
<link rel=Edit-Time-Data href="周源山表3_files/editdata.mso">
<link rel=OLE-Object-Data href="周源山表3_files/oledata.mso">
<!--[if gte mso 9]><xml>
 <o:DocumentProperties>
  <o:Author>YuHailong</o:Author>
  <o:LastAuthor>微软用户</o:LastAuthor>
  <o:LastPrinted>2013-12-24T03:32:53Z</o:LastPrinted>
  <o:Created>2008-01-10T02:24:54Z</o:Created>
  <o:LastSaved>2014-01-09T02:13:19Z</o:LastSaved>
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
.xl25
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;}
.xl26
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	white-space:normal;}
.xl27
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl28
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl29
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
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
	border-left:.5pt solid windowtext;}
.xl31
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
.xl32
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
	border-left:.5pt solid windowtext;}
.xl34
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
.xl35
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
.xl36
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:.5pt solid windowtext;}
.xl37
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:.5pt solid windowtext;}
.xl38
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl39
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl40
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl41
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;}
.xl42
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:none;
	border-left:.5pt solid windowtext;}
.xl43
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:none;}
.xl44
	{mso-style-parent:style21;
	color:black;
	font-size:9.0pt;
	mso-number-format:"\@";
	text-align:center;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;}
.xl45
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	text-align:left;
	vertical-align:top;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:none;
	border-left:.5pt solid windowtext;
	white-space:normal;}
.xl46
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	text-align:left;
	vertical-align:top;
	border-top:.5pt solid windowtext;
	border-right:none;
	border-bottom:none;
	border-left:none;
	white-space:normal;}
.xl47
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	text-align:left;
	vertical-align:top;
	border-top:.5pt solid windowtext;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:none;
	white-space:normal;}
.xl48
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	text-align:left;
	vertical-align:top;
	border-top:none;
	border-right:none;
	border-bottom:none;
	border-left:.5pt solid windowtext;
	white-space:normal;}
.xl49
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	text-align:left;
	vertical-align:top;
	white-space:normal;}
.xl50
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	text-align:left;
	vertical-align:top;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:none;
	border-left:none;
	white-space:normal;}
.xl51
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	text-align:left;
	vertical-align:top;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:.5pt solid windowtext;
	white-space:normal;}
.xl52
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	text-align:left;
	vertical-align:top;
	border-top:none;
	border-right:none;
	border-bottom:.5pt solid windowtext;
	border-left:none;
	white-space:normal;}
.xl53
	{mso-style-parent:style21;
	color:black;
	font-size:11.0pt;
	text-align:left;
	vertical-align:top;
	border-top:none;
	border-right:.5pt solid windowtext;
	border-bottom:.5pt solid windowtext;
	border-left:none;
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
    <x:Name>（三）周源山矿部门详单</x:Name>
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
       <x:ActiveRow>10</x:ActiveRow>
       <x:ActiveCol>24</x:ActiveCol>
       <x:RangeSelection>$Y$11:$Y$12</x:RangeSelection>
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
  <x:Formula>='（三）周源山矿部门详单'!$1:$5</x:Formula>
 </x:ExcelName>
</xml><![endif]-->
</head>

<body link=blue vlink=purple class=xl25>

<table x:str border=0 cellpadding=0 cellspacing=0 width=3036 style='border-collapse:
 collapse;table-layout:fixed;width:2285pt'>
 <col class=xl25 width=27 style='mso-width-source:userset;mso-width-alt:864;
 width:20pt'>
 <col class=xl25 width=39 style='mso-width-source:userset;mso-width-alt:1248;
 width:29pt'>
 <col class=xl25 width=44 style='mso-width-source:userset;mso-width-alt:1408;
 width:33pt'>
 <col class=xl25 width=80 style='mso-width-source:userset;mso-width-alt:2560;
 width:60pt'>
 <col class=xl25 width=57 style='mso-width-source:userset;mso-width-alt:1824;
 width:43pt'>
 <col class=xl25 width=52 style='mso-width-source:userset;mso-width-alt:1664;
 width:39pt'>
 <col class=xl25 width=38 style='mso-width-source:userset;mso-width-alt:1216;
 width:29pt'>
 <col class=xl25 width=29 style='mso-width-source:userset;mso-width-alt:928;
 width:22pt'>
 <col class=xl25 width=27 style='mso-width-source:userset;mso-width-alt:864;
 width:20pt'>
 <col class=xl25 width=28 style='mso-width-source:userset;mso-width-alt:896;
 width:21pt'>
 <col class=xl25 width=27 style='mso-width-source:userset;mso-width-alt:864;
 width:20pt'>
 <col class=xl25 width=28 span=6 style='mso-width-source:userset;mso-width-alt:
 896;width:21pt'>
 <col class=xl25 width=33 style='mso-width-source:userset;mso-width-alt:1056;
 width:25pt'>
 <col class=xl25 width=77 span=31 style='mso-width-source:userset;mso-width-alt:
 2464;width:58pt'>
 <tr height=33 style='mso-height-source:userset;height:24.75pt'>
  <td colspan=49 height=33 class=xl30 width=3036 style='border-right:.5pt solid black;
  height:24.75pt;width:2285pt'>周源山煤矿部门详单</td>
 </tr>
  <%
      System.DateTime beginTime = Convert.ToDateTime(Request.QueryString["beginTime"]);
      System.DateTime endTime = Convert.ToDateTime(Request.QueryString["endTime"]);     
 %>
 <tr height=33 style='mso-height-source:userset;height:24.75pt'>
  <td colspan=49 height=33 class=xl33 style='border-right:.5pt solid black;
  height:24.75pt'><%=beginTime.ToString("yyyy-MM-dd")%>到<%=endTime.ToString("yyyy-MM-dd")%></td>
 </tr>
 <tr height=19 style='mso-height-source:userset;height:14.25pt'>
  <td rowspan=2 height=38 class=xl37 style='border-bottom:.5pt solid black;
  height:28.5pt;border-top:none'>序号</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>工号</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>姓名</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>部门</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>总时长</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>井下均时</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>总次数</td>
  <td class=xl26 width=29 style='width:22pt'>有效</td>
  <td colspan=3 class=xl40 style='border-right:.5pt solid black;border-left:
  none'>按班次分</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>无效</td>
  <td colspan=5 class=xl40 style='border-right:.5pt solid black;border-left:
  none'>按时间分</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>类别</td>
  <%
      for (int index = 1; index < 32; index++)
     {
  %>
   <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
   none'><%=index%></td>
  <%
     }
  %>
   
  
 </tr>
 <tr height=19 style='mso-height-source:userset;height:14.25pt'>
  <td height=19 class=xl26 width=29 style='height:14.25pt;width:22pt'>次数</td>
  <td class=xl27>早班</td>
  <td class=xl27>中班</td>
  <td class=xl27>晚班</td>
  <td class=xl28>0-2</td>
  <td class=xl28>2-4</td>
  <td class=xl28>4-8</td>
  <td class=xl28>8-12</td>
  <td class=xl28>&gt;12</td>
 </tr>
 <% 
     int count = 0;     
     int condition = DomainServiceIriskingAttend.ConditionShowElementType;

　　 while( condition > 0 )
     {
　　　　condition = condition & (condition - 1);
　　　　count +=1;
     }
     
     int i=0;
     for (i = 0; i < DomainServiceIriskingAttend.DepartDetailList.Count-1;i++ )
     {
         bool bFirst = true;
%>
<tr height=18 style='mso-height-source:userset;height:13.5pt'>
  <td rowspan=<%=count%> height=54 class=xl37 style='border-bottom:.5pt solid black;
  height:40.5pt;border-top:none'><%=i+1 %></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].work_sn%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].name%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].depart_name%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=string.Format("{0}:{1}",
        DomainServiceIriskingAttend.DepartDetailList[i].total_work_time/60,
        DomainServiceIriskingAttend.DepartDetailList[i].total_work_time % 60)%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=string.Format("{0}:{1}",
        DomainServiceIriskingAttend.DepartDetailList[i].avg_work_time/60,
        DomainServiceIriskingAttend.DepartDetailList[i].avg_work_time % 60)%>　</td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%= DomainServiceIriskingAttend.DepartDetailList[i].total_times%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%= DomainServiceIriskingAttend.DepartDetailList[i].valid_times%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%= DomainServiceIriskingAttend.DepartDetailList[i].zao_times%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%= DomainServiceIriskingAttend.DepartDetailList[i].zhong_times%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%= DomainServiceIriskingAttend.DepartDetailList[i].wan_times%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%= DomainServiceIriskingAttend.DepartDetailList[i].invalid_times%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%= DomainServiceIriskingAttend.DepartDetailList[i].sum_0_2%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%= DomainServiceIriskingAttend.DepartDetailList[i].sum_2_4%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%= DomainServiceIriskingAttend.DepartDetailList[i].sum_4_8%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%= DomainServiceIriskingAttend.DepartDetailList[i].sum_8_12%></td>
  <td rowspan=<%=count%> class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%= DomainServiceIriskingAttend.DepartDetailList[i].sum_12%></td>
  <%
      if( (DomainServiceIriskingAttend.ConditionShowElementType & (int)IriskingAttend.Web.ZhouYuanShan.ShowElementType.ClassOrder) != 0 )
      {
          bFirst = false;
  %>
  <td class=xl28>班次</td>
  <%
         for (int dayIndex = 0; dayIndex < 31; dayIndex++)
         {
             if (DomainServiceIriskingAttend.DepartDetailList[i].display_content_color[dayIndex].CompareTo("0xffffa500") == 0)
             {
  %>
         <td class=xl28 style='color:Orange'><%=DomainServiceIriskingAttend.DepartDetailList[i].attend_signs[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.DepartDetailList[i].display_content_color[dayIndex].CompareTo("0xff008000") == 0)
             {
  %>
         <td class=xl28 style='color:Green'><%=DomainServiceIriskingAttend.DepartDetailList[i].attend_signs[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.DepartDetailList[i].display_content_color[dayIndex].CompareTo("0xffff0000") == 0)
             {
  %>
         <td class=xl28 style='color:Red'><%=DomainServiceIriskingAttend.DepartDetailList[i].attend_signs[dayIndex]%></td>
  <% 
             }   
             else
             {
  %>
         <td class=xl28 style='color:Black'><%=DomainServiceIriskingAttend.DepartDetailList[i].attend_signs[dayIndex]%></td>
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
  <td height=18 class=xl28 style='height:13.5pt ' >时长</td>
  <%
         for (int dayIndex = 0; dayIndex < 31; dayIndex++)
         {
             if (DomainServiceIriskingAttend.DepartDetailList[i].display_content_color[dayIndex].CompareTo("0xffffa500") == 0)
             {
  %>
         <td class=xl28 style='color:Orange'><%=DomainServiceIriskingAttend.DepartDetailList[i].work_times[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.DepartDetailList[i].display_content_color[dayIndex].CompareTo("0xff008000") == 0)
             {
  %>
         <td class=xl28 style='color:Green'><%=DomainServiceIriskingAttend.DepartDetailList[i].work_times[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.DepartDetailList[i].display_content_color[dayIndex].CompareTo("0xffff0000") == 0)
             {
  %>
         <td class=xl28 style='color:Red'><%=DomainServiceIriskingAttend.DepartDetailList[i].work_times[dayIndex]%></td>
  <% 
             }   
             else
             {
  %>
         <td class=xl28 style='color:Black'><%=DomainServiceIriskingAttend.DepartDetailList[i].work_times[dayIndex]%></td>
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
  <td height=18 class=xl28 style='height:13.5pt'>时间</td>
  <%
         for (int dayIndex = 0; dayIndex < 31; dayIndex++)
         {
             if (DomainServiceIriskingAttend.DepartDetailList[i].display_content_color[dayIndex].CompareTo("0xffffa500") == 0)
             {
  %>
         <td class=xl28 style='color:Orange'><%=DomainServiceIriskingAttend.DepartDetailList[i].time_descriptions[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.DepartDetailList[i].display_content_color[dayIndex].CompareTo("0xff008000") == 0)
             {
  %>
         <td class=xl28 style='color:Green'><%=DomainServiceIriskingAttend.DepartDetailList[i].time_descriptions[dayIndex]%></td>
  <% 
             }
             else if (DomainServiceIriskingAttend.DepartDetailList[i].display_content_color[dayIndex].CompareTo("0xffff0000") == 0)
             {
  %>
         <td class=xl28 style='color:Red'><%=DomainServiceIriskingAttend.DepartDetailList[i].time_descriptions[dayIndex]%></td>
  <% 
             }   
             else
             {
  %>
         <td class=xl28 style='color:Black'><%=DomainServiceIriskingAttend.DepartDetailList[i].time_descriptions[dayIndex]%></td>
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
  <td rowspan=2 height=36 class=xl37 style='border-bottom:.5pt solid black;
  height:27.0pt;border-top:none'>　</td>
  <td colspan=2 rowspan=2 class=xl42 style='border-right:.5pt solid black;
  border-bottom:.5pt solid black'>合计</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'> </td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'> <%=string.Format("{0}:{1}",
            DomainServiceIriskingAttend.DepartDetailList[i].total_work_time / 60,
            DomainServiceIriskingAttend.DepartDetailList[i].total_work_time % 60)%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=string.Format("{0}:{1}",
            DomainServiceIriskingAttend.DepartDetailList[i].avg_work_time / 60,
            DomainServiceIriskingAttend.DepartDetailList[i].avg_work_time % 60)%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].total_times%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].valid_times%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].zao_times%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].zhong_times%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].wan_times%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].invalid_times%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].sum_0_2%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].sum_2_4%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].sum_4_8%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].sum_8_12%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'><%=DomainServiceIriskingAttend.DepartDetailList[i].sum_12%></td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'> </td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
  <td rowspan=2 class=xl37 style='border-bottom:.5pt solid black;border-top:
  none'>　</td>
 </tr>
 <tr height=18 style='mso-height-source:userset;height:13.5pt'>
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
  <td width=27 style='width:20pt'></td>
  <td width=39 style='width:29pt'></td>
  <td width=44 style='width:33pt'></td>
  <td width=80 style='width:60pt'></td>
  <td width=57 style='width:43pt'></td>
  <td width=52 style='width:39pt'></td>
  <td width=38 style='width:29pt'></td>
  <td width=29 style='width:22pt'></td>
  <td width=27 style='width:20pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=27 style='width:20pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=28 style='width:21pt'></td>
  <td width=33 style='width:25pt'></td>
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
