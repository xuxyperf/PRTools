CREATE TABLE PRDASD 
   (	
     LPARNAME NVARCHAR2(10), 
	   TIMEPOINT NVARCHAR2(16), 
	   SERVICEGROUP NVARCHAR2(10), 
	   VOLUMESERIAL NVARCHAR2(12), 
	   DEVICEACTIVITYRATE NUMBER(12,3), 
	   AVGRESPTIME NUMBER(12,3)
    ) 
    PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
    STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
    PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
    TABLESPACE XXXXXX ;
 
    CREATE INDEX PRT_LPTP_INDEX ON PRDASD (LPARNAME, TIMEPOINT) 
    PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
    STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
    PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
    TABLESPACE XXXXXX ;