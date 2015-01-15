--=============================================================================================
-- Table: 报表模板 
--=============================================================================================

create  table r_report_template
(
	r_report_template_id                  UUID   NOT NULL,
	parent_report_template_id             UUID   DEFAULT NULL,
	r_xml_data                            VARCHAR   NULL,                           --报表模板的序列化字符串
	
        CONSTRAINT r_report_template_pkey PRIMARY KEY (r_report_template_id)
);
    
         
      