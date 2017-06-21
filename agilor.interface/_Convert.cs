using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Agilor.Interface
{
    internal class _Convert
    {

        #region AGCN接口

        //参数：无
        //功能：本地服务的启动
        //返回值：成功0，失败-1
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Agcn_Startup();

        //参数：无
        //功能：本地服务的停止
        //返回值
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Agcn_Cleanup();

        /*参数：LPCTSTR szServerName	：输入参数，服务器名
        		LPCTSTR szServerAddr	：输入参数，服务器IP地址
        		LPCTSTR szUserName		：输入参数，用户名
        		LPCTSTR szPassword		：输入参数，用户密码
        		UINT nPort				：输入参数，通信端口
        功能描述：建立一个连接
        返回值：0： 	连接正常
        		-1： 	无效用户名或密码
        		-2： 	目标服务器已经连接
        		-4： 	服务器关闭或无效IP地址或连接超时
        		-99：	未知错误
		*/
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agcn_Connect(string szServerName, string szServerAddr, string szUserName, string szPassword, uint nPort = 900);

        //参数：LPCTSTR szServerName		：输入参数，服务器名
        //HWND hWnd				：输入参数，图形句柄
        //DWORD hThreadID			：输入参数，线程句柄
        //BOOL bAllowEvents			：是否允许事件
        //功能描述：创建一个连接节点
        //返回值：ture：创建成功    false：创建失败
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Agcn_CreateNode(string szServerName, IntPtr hWnd, int hThreadID, bool bAllowEvents = false);

        //参数：LPCTSTR szServerName		：输入参数，服务器名
        //功能：删除一个服务器节点
        //返回值：ture：删除成功     false：删除失败
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Agcn_RemoveNode(string szServerName);

        //参数：LPCTSTR szServerName		：输入参数，服务器名
        //功能描述：断开指定的服务器名的连接
        //返回值：>0 ：nServerID（注：没有出现这种情况，成功断开返回值为“0”）
        //<0 ：失败
        //-1 ：无效的服务器名或目标服务器尚未连接
        //-99：未知错误
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agcn_Disconnect(string szServerName);

        //参数：nServerID：输入/输出参数，第一次传送0，以后传送为获得服务器的节点号，返回服务器ID号
        //pSvrInfo：输出参数，返回连接服务器的信息
        //功能描述：读配置文件中所有服务器的节点
        //返回值：True：第一次返回第一个服务器节点信息，以后连接返回连接服务器的信息
        //False：连接不成功
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Agcn_EnumNodeInfo(long nServerID, ref struServer_Info pSvrInfo);
        #endregion AGCN

        #region AGPT接口
        //*** 常量定义
        public const int C_FULL_TAGNAME_LEN = 80;	//SERVERNAME.TAGNAME, sucha as LGCAG.ZL_AI1001
        public const int C_SERVERNAME_LEN = 16;
        public const int C_SERVERADDR_LEN = 16;
        public const int C_USERNAME_LEN = 32;
        public const int C_PASSWORD_LEN = 16;
        public const int C_TAGNAME_LEN = 64;	//maybe some tags on different server have the same name
        public const int C_TAGDESC_LEN = 32;
        public const int C_TAGUNIT_LEN = 16;
        public const int C_DEVICENAME_LEN = 32;
        public const int C_GROUPNAME_LEN = 32;
        public const int C_STRINGVALUE_LEN = 128;
        public const int C_SOURCETAG_LEN = 128;	//the physical tag on devices
        public const int C_ENUMDESC_LEN = 128;

        public const uint WM_USER = 0x0400;
        public const uint WM_SUBDATAARRIVAL = WM_USER + 101;

        //*** 报警状态位
        public const ushort ALARM_TYPE_HILIMIT_MASK = 0x0001;	//高报警 // for Long, Float
        public const ushort ALARM_TYPE_LOLIMIT_MASK = 0x0002;	//低报警 // for Long, Float
        public const ushort ALARM_TYPE_HIHILIMIT_MASK = 0x0004;	//高高报警//for Long, Float
        public const ushort ALARM_TYPE_LOLOLIMIT_MASK = 0x0008;	//低低报警//for Long, Float
        public const ushort ALARM_TYPE_SWITCHON_MASK = 0x0010;	//开报警 // for BOOL 
        public const ushort ALARM_TYPE_SWITCHOFF_MASK = 0x0020;	//关报警 // for BOOL
        //*** 启动数据采集标志
        public const byte SCAN_DISABLE = 0x80;	//禁止
        public const byte SCAN_INPUT = 0x01;	//输入
        public const byte SCAN_OUTPUT = 0x02;	//输出

        /// <summary>
        /// 登录数据库基本信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct struServer_Info              //服务器信息结构定义
        {
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)C_SERVERNAME_LEN)]
            //public char[] szServerName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_SERVERNAME_LEN)]
            public string szServerName;

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)C_SERVERADDR_LEN)]
            //public char[] szServerAddr;  //服务器地址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_SERVERADDR_LEN)]
            public string szServerAddr;  //服务器地址

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)C_USERNAME_LEN)]
            //public char[] szUserName;     //登陆用户名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_USERNAME_LEN)]
            public string szUserName;     //登陆用户名

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)C_PASSWORD_LEN)]
            //public char[] szPassword;      //用户密码
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_PASSWORD_LEN)]
            public string szPassword;      //用户密码

            public bool bIsConnected;      //服务器是否连接
        }        //SERVER_INFO;


        [StructLayout(LayoutKind.Sequential)]
        public struct struDevice_Info   //设备信息结构定义
        {
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)C_DEVICENAME_LEN)]
            //public char[] szDeviceName;   //设备名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_DEVICENAME_LEN)]
            public string szDeviceName;   //设备名

            public bool bIsOnline;                               //设备是否在线
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct struTagNode       //采集点
        {

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)C_TAGNAME_LEN)]
            //public char[] name;   //采集数据点名(必须)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_TAGNAME_LEN)]
            public string name;   //采集数据点名(必须)

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            //public char[] descriptor;   //采集数据点名(必须)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string descriptor;   //采集数据点名(必须)

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            //public char[] engunits;   //采集点数据单位
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string engunits;   //采集点数据单位

            public int pointid;		  //采集数据点编号
            public byte pointtype;    //采集点数据类型//(R浮点数/S字符串/B开关/L整形/E枚举)
            public byte scan;
            public ushort reserved1;    //保留值，暂未使用		        
            public Single typicalvalue;		//典型值 

            //public nodeunion mynodeunion; 
            //这是占数据位的
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 132)]
            private string zhanwei;

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)C_ENUMDESC_LEN)]
            //public char[] enumdesc;//枚举描述   eg. "2:1,2,on:0,3,off"  "枚举个数:标志1,枚举名1长度,枚举名1:标志2,枚举名2长度,枚举名2...   
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_ENUMDESC_LEN)]
            public string enumdesc;//枚举描述   eg. "2:1,2,on:0,3,off"  "枚举个数:标志1,枚举名1长度,枚举名1:标志2,枚举名2长度,枚举名2...   

            public int timedate;		//时间戳
            public int istat;			//点状态

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)C_DEVICENAME_LEN)]
            //public char[] sourceserver; //*采集点的数据源站(设备名)
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_DEVICENAME_LEN)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sourceserver; //*采集点的数据源站(设备名)


            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            //public char[] sourcegroup; //*采集点的数据源结点组
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sourcegroup; //*采集点的数据源结点组

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)C_SOURCETAG_LEN)]
            //public char[] sourcetag; //*采集点的数据源结点组
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_SOURCETAG_LEN)]
            public string sourcetag; //*采集点的数据源结点组

            public Single upperlimit;//数据上限
            public Single lowerlimit;//数据下限

            public ushort pushref1;		//实时推理规则标志
            public ushort ruleref1;		//实时推理规则标志	
            public int excmin;			//实时数据处理最短间隔（处理周期）
            public int excmax;			//实时数据处理最大间隔
            public Single excdev;			//实时数据处理灵敏度
            public ushort alarmtype;		//报警
            public ushort alarmstate;		//状态报警
            public Single alarmmax;		//上限报警
            public Single alarmmin;		//下限报警
            public Single alarmhihi;        //高高报警
            public Single alarmlolo;        //低低报警
            public ushort hipriority;			//报警优先级
            public ushort lopriority;            //低报警优先级
            public ushort hihipriority;          //高高报警优先级
            public ushort lolopriority;          //低低报警优先级
            public byte archiving;		//是否存储历史数据
            public byte compressing;		//是否进行历史压缩
            public byte step;			//历史数据的插值形式（0表示线形，1表示台阶）
            public byte reserved2;    //保留值，暂不使用
            public int hisidx;			//历史记录索引号
            public int compmin;		//压缩最断间隔
            public int compmax;		//压缩最长间隔
            public Single compdev;		    //压缩灵敏度
            public Single lastval;	     	//上次数据存档的值
            public int lasttime;		    //上次数据存档的时间
            public int creationdate;		//采集点创建日期
        }



        //参数：LPCTSTR szServerName：输入参数，服务器名称
        //功能：获得输入参数表示的服务器上的全部设备信息，并将这些信息放到一个结果集中
        //返回值：>=0 ：设备信息结果集句柄
        //-1   ：服务器断网或无效的服务器名
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agpt_QueryDeviceInfo(string szServerName);        //获取源设备站的结果集

        //功能：通过Agpt_QueryDeviceInfo得到的结果集句柄，获取设备信息（设备ID和设备描述等）
        //返回值：ture	：获得设备信息成功
        //false：获得设备信息失败
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Agpt_EnumDeviceInfo(int hRecordset, out int nDeviceID, ref struDevice_Info pDevInfo);

        //参数：LPCTSTR szServerName		：输入参数，服务器名
        //LPCTSTR szDeviceName		    ：输入参数，设备名
        //功能：获取一个实时数据库服务器上某个设备的点信息集句柄
        //返回值： >0 :  点信息结果集句柄
        //0:   设备上没有点或者服务器上没有该设备
        //-1 : 服务器断网或无效的服务器名
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agpt_QueryTagsbyDevice(string szServerName, string szDeviceName);

        //参数：LPCTSTR szServerName	      ：输入参数，数据库服务器名
        //LPCTSTR szTagNameMask	  ：输入参数，查询数据掩码
        //功能：通过掩码查询服务器上点信息
        //返回值：>0   点信息结果集句柄
        //0 :  没有与掩码匹配的点
        //-1:  服务器断网或无效的服务器名
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agpt_QueryTagsbyNameMask(string szServerName, string szTagNameMask);


        //参数：HRECORDSET hRecordset	：输入参数，点信息结果集句柄
        //long * nTagID				：输出参数，获取点ID
        //LPTSTR szTagName		    ：输出参数，获取点名
        //功能：以消耗方式获得点信息结果集中的一个点信息
        //返回值：true：获得信息成功
        //false：结果集已经为空
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Agpt_EnumTagName(int hRecordset, ref int nTagID, StringBuilder szTagName);  //or stringbuilder

        //参数：LPCTSTR szFullTagName	    ：输入参数，点名
        //TAG_INFO * pTagInfo		：输出参数，获取点信息
        //功能：通过点名获得该点的具体信息，并将它保存在输出参数中
        //返回值：0 ：成功
        //-1 ：无效的服务器名
        //-2 ：目标服务器已经断连
        //-3 ：等待超时
        //-4 ：等待错误
        //-5 ：无效点名
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agpt_GetTagInfo(string szFullTagName, ref struTagNode pTagInfo);

        //参数：long nTagID			：输入参数，点ID
        //LPTSTR szTagName	    ：输出参数，点名
        //功能：通过点ID得到点名
        //返回值：true	：获得成功
        //false	：获得失败

        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Agpt_GetTagNamebyID(int nTagID, StringBuilder szTagName);//or stringbuilder

        //参数：szServerName：输入参数，数据库服务器名
        //lTagID：输入参数，点的ID
        //功能：客户端发出请求，从服务器端删除一个点
        //返回值：0 ：成功
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agpt_RemoveTag(string szServerName, int lTagID);

        //参数：szServerName：输入参数，数据库服务器名
        //pTagNode：输入参数，添加的点信息
        //bOverwrite：输入参数，数据库中存在该点，是否被覆盖
        //功能：客户端发出请求，从服务器端添加一个点
        //返回值：0 ：成功
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agpt_AddNewTag(string szServerName, ref struTagNode pTagNode, bool bOverwrite);

        #endregion AGPT

        #region AGSN接口


        [StructLayout(LayoutKind.Sequential)]
        public struct struTagValue           //点信息：int、float、bool
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string TagName;
            public System.Int32 Timestamp;
            public System.Int32 TagState;
            public System.Int32 TagType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] value;
        };


        public struct TAGVALUE
        {
            public byte type;		// 类型
            public System.Int32 lTimestamp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] value;
        }


        //参数：LPCTSTR szServerName	    ：输入参数，点信息所在的服务器名
        //LPCTSTR szTagNames		：输入参数，需要获得信息的点的名称字串
        //UINT nTagCount			：输入参数，需要获得信息的点的个数
        //功能：截取参数标识的点的当前信息
        //返回值：>0 ：返回点信息的结果集句柄
        //-1 ：非法的服务器名
        //-2 ：服务器已经断连
        //-3 ：等待失败
        //-4 ：其他错误
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agda_QuerySnapshots(string szServerName, string szTagNames, uint nTagCount);


        //参数：HRECORDSET hRecordset	：输入参数，结果集句柄
        //TAGVAL * pTagVal		    ：输出参数，用来返回一个点信息
        //BOOL bRemoved			：输入参数，是否将所读出的结果记录从结果集中删除
        //功能：读取点信息结果集中的一条信息
        //返回值：true ：读取信息成功
        //False ：读取数据失败
        [DllImport("AgilorAPI.dll")]
        public static extern bool Agda_GetNextTagValue(int hRecordset, ref struTagValue pTagVal, bool bRemoved);

        //参数：LPCTSTR szServerName	    ：输入参数，订阅点所在的服务器名
        //LPCTSTR szTagNames		：输入参数，订阅点名的字符串
        //UINT nTagCount			：输入参数，订阅点的个数
        //功能：订阅点信息，利用该ACI可以异步地得到数据服务器上订阅点的动态更新信息
        //返回值：>=0 ：返回订阅点数目
        //-1  ：非法服务器状态
        //-2  ：客户端已经与服务器断连
        //-3  ：非法点名
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agda_SubscribeTags(string szServerName, string szTagNames, uint nTagCount);

        //[DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public static extern int Agda_SubscribeTags(string szServerName, IntPtr szTagNames, uint nTagCount);

        //参数：TAGVAL * pTagVal	：输出参数，用来返回一个更新的订阅点的信息
        //功能：获取订阅结果，每当新的数据到达, ACI通过自定义消息通知
        //返回值：ture ：信息读取成功
        //false ：信息读取失败
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Agda_GetSubTagValue(ref struTagValue pTagVal);


        //参数：LPCTSTR szServerName	    ：输入参数，被取消的订阅点所在的服务器名
        //LPCTSTR szTagNames		：输入参数，被取消的订阅点名的字符串
        //UINT nTagCount			：输入参数，被取消的订阅点的个数
        //功能：取消参数指定的一系列订阅点
        //返回值：>=0 ：返回取消的订阅点数目
        //-1  ：非法服务器状态
        //-2  ：客户端已经与服务器断连
        //-3  ：非法点名
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agda_UnSubscribeTags(string szServerName, string szTagNames, uint nTagCount);

        //参数：LPCTSTR szServerName	：输入参数，被取消的订阅点所在的服务器名
        //功能：取消目标服务器上所以的订阅点
        //返回值：0  ：服务器上所有订阅点成功被取消
        //-1 ：非法的服务器名
        //-2 ：服务器连接已经断开
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agda_UnSubscribeAll(string szServerName);

        //参数：LPCTSTR szFullTagName  	：输入参数，完全点名（服务器名.点名）
        //VALUE * pValue			：输入参数，点信息值
        //BOOL bManual			    ：输入参数，默认为false，表示设置数据库，true
        //LPCTSTR szComment		：输入参数，注释
        //功能：设置点值
        //返回值：0：正确执行
        //-1：非法的服务器名
        //-2：目标服务器已经断连
        //-3：非法点名
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agda_SetTagValue(string szFullTagName, ref TAGVALUE pValue, bool bManual = false, string szComment = null);


        #endregion

        #region AGAR接口


        enum Enum_Aggregate_Function
        {
            AF_SUMMARY,           //和
            AF_MINIMUM,           //最小值
            AF_MAXIMUM,          //最大值
            AF_AVERAGE,           //平均值
            AF_COUNT,             //数据数量
        }


        //参数：LPCTSTR szFullTagName	    ：输入参数，完全点名（服务器名.点名）
        //long nStartTime			    ：输入参数，起始时间
        //long nEndTime			    ：输入参数，终止时间
        //long nStep=0				：输入参数，步长（默认为“0”,表示获得时间段内所以的点）
        //功能：获得一个点在一个时间段的历史记录
        //返回值：>0 ：返回点的历史记录信息的结果集句柄
        //-1 ：非法的服务器名
        //-2 ：服务器已经断连
        //-3 ：等待超时
        //-4 ：等待失败
        //-5 ：非法点名
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agda_QueryTagHistory(string szFullTagName, int nStartTime, int nEndTime, int nStep = 0);

        //参数：HRECORDSET hRecordset	：输入参数，记录结果集句柄
        //TAGVAL * pTagVal		    ：输出参数，返回统计结果
        //long nAFunction			    ：输入参数，统计规则采用下面枚举类型中的一种规则；
        ///*enum Enum_Aggregate_Function    
        //{
        //    AF_SUMMARY,           //和
        //    AF_MINIMUM,           //最小值
        //    AF_MAXIMUM,          //最大值
        //    AF_AVERAGE,           //平均值
        //    AF_COUNT,             //数据数量
        //};*/
        //BOOL bRemoved			：输入参数，是否将统计点从结果集中删除
        //功能：根据统计规则，统计结果集
        //返回值： 0：正确执行
        //-1：无效的统计规则
        //-2：存在无效记录
        //-3：记录集无效
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agda_GetAggregateValue(int hRecordset, ref struTagValue pTagVal, int nAFunction, bool bRemoved = true);

        #endregion

        #region AGLG接口

        //参数：LPCTSTR szFullTagName	    ：输入参数，点名
        //long nTimeStamp			：输入参数，一个时间点
        //long nTagState				：输入参数，点的状态
        //LPCTSTR szComment		：输入参数，注释
        //功能：给一个点在特定时刻的特定状态添加注释
        //返回值：0：正确执行
        //-1：非法的服务器名
        //-2：目标服务器已经断连
        //-3：非法点名
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agda_AddTagComment(string szFullTagName, int nTimeStamp, int nTagState, string szComment);

        ////参数：LPCTSTR szFullTagName	    ：输入参数，完全点名（服务器名.点名）
        ////VALUE * pValue			：输入参数，点信息值
        ////BOOL bManual			：输入参数，默认为false，表示设置数据库，true
        ////LPCTSTR szComment	：输入参数，注释
        ////功能：设置点值
        ////返回值：0：正确执行
        ////-1：非法的服务器名
        ////-2：目标服务器已经断连
        ////-3：非法点名
        //[DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public static extern int Agda_SetTagValue(string szFullTagName, ref struValue pValue, bool bManual = false, string szComment = null);

        #endregion

        #region AGTM接口
        //参数：int hour	    ：输入参数，时
        //int min			：输入参数，分
        //int sec			：输入参数，秒
        //int year		：输入参数，年（1900年为0）
        //int mon		：输入参数，月（1月为0）
        //int mday		：输入参数，日
        //功能：将一个YYYY-MM-DD HH:MM:SS格式的时间转换成表示时间的long类型数值
        //返回值：返回表示时间的long类型数值
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agtm_DateTime2Long(int hour, int min, int sec, int year, int mon, int mday);

        //参数：BOOL bLocal		：输入参数
        //功能：读取系统时间，当输入参数为true，表示获取本地时间；当输入参数为false，表示获取远程时间
        //返回值：返回表示时间的long类型数值
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agtm_GetCurrTime(bool bLocal = true);

        //参数：long nDateTime		    ：输入参数，表示时间的long类型数值
        //LPTSTR szDateString	：输出参数，YYYY-MM-DD HH:MM:SS格式字串
        //功能：将一个表示时间的long类型数值转换成YYYY-MM-DD HH:MM:SS格式的时间
        //返回值：1 ：转换成功
        [DllImport("AgilorAPI.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Agtm_Long2DateString(int nDateTime, string szDateString);

        #endregion




        #region 数采接口

        public struct TAG_VALUE_LOCAL
        {
            public int lLocalID;			// 本地重新分配的ID
            public int lTagID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTagSource;
            public byte cTagType;			// char		//(R 浮点数/S字符串/B开关/L整形)
            public int nTagState;			// 增加数据质量，作为RTDB中State中Byte2,Byte1
            public int lTimeStamp;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] value;
          
            //public strcurrvalue value;
        }

        public struct TAG_NODE{
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_TAGNAME_LEN)]
            public string name;   //采集数据点名(必须)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string descriptor;   //采集数据点名(必须)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string engunits;   //采集点数据单位

            public int pointid;		  //采集数据点编号
            public byte pointtype;    //采集点数据类型//(R浮点数/S字符串/B开关/L整形/E枚举)
            public byte scan;
            public ushort reserved1;    //保留值，暂未使用		        
            public Single typicalvalue;		//典型值 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 132)]
            private string zhanwei;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_ENUMDESC_LEN)]
            public string enumdesc;//枚举描述   eg. "2:1,2,on:0,3,off"  "枚举个数:标志1,枚举名1长度,枚举名1:标志2,枚举名2长度,枚举名2...   

            public int timedate;		//时间戳
            public int istat;			//点状态
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sourceserver; //*采集点的数据源站(设备名)
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sourcegroup; //*采集点的数据源结点组
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)C_SOURCETAG_LEN)]
            public string sourcetag; //*采集点的数据源结点组

            public Single upperlimit;//数据上限
            public Single lowerlimit;//数据下限

            public ushort pushref1;		//实时推理规则标志
            public ushort ruleref1;		//实时推理规则标志	
            public int excmin;			//实时数据处理最短间隔（处理周期）
            public int excmax;			//实时数据处理最大间隔
            public Single excdev;			//实时数据处理灵敏度
            public ushort alarmtype;		//报警
            public ushort alarmstate;		//状态报警
            public Single alarmmax;		//上限报警
            public Single alarmmin;		//下限报警
            public Single alarmhihi;        //高高报警
            public Single alarmlolo;        //低低报警
            public ushort hipriority;			//报警优先级
            public ushort lopriority;            //低报警优先级
            public ushort hihipriority;          //高高报警优先级
            public ushort lolopriority;          //低低报警优先级
            public byte archiving;		//是否存储历史数据
            public byte compressing;		//是否进行历史压缩
            public byte step;			//历史数据的插值形式（0表示线形，1表示台阶）
            public byte reserved2;    //保留值，暂不使用
            public int hisidx;			//历史记录索引号
            public int compmin;		//压缩最断间隔
            public int compmax;		//压缩最长间隔
            public Single compdev;		    //压缩灵敏度
            public Single lastval;	     	//上次数据存档的值
            public int lasttime;		    //上次数据存档的时间
            public int creationdate;		//采集点创建日期
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate void TARGETHANDLER(ref TAG_NODE node);
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate void TARGETVALHANDLER(ref TAG_VALUE_LOCAL val);




        [DllImport("DeviceRtdb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int DRTDB_RegisterDevice(string server, int port, string device);

        [DllImport("DeviceRtdb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void DRTDB_SetCallBackFunction(TARGETHANDLER target_arrived, TARGETHANDLER target_removed, TARGETVALHANDLER value_seted, TARGETHANDLER value_geted);

        [DllImport("DeviceRtdb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int DRTDB_MD_UnregisterDevice(string sDeviceName, string sCause, int lErrCode = 0);
        [DllImport("DeviceRtdb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int DRTDB_MD_Flush(string sDeviceName);
        //int DRTDB_MD_FindTagBySource(string sDeviceName, string sourcename, TAG_NODE tagnode);
        
        //public static extern int DRTDB_MD_GetCurrentState(string sDeviceName);
        //int DRTDB_MD_GetNextTagNode(string sDeviceName, int lTagID, TAG_NODE TagNode);
        //public static extern void DRTDB_MD_GetParameter(string sDeviceName, bool isDataBuf, bool isBufSend);
        //public static extern int DRTDB_MD_GetTagCount(string sDeviceName, int lTagCount);
        [DllImport("DeviceRtdb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int DRTDB_MD_SendNewValue(string sDeviceName,ref TAG_VALUE_LOCAL TagValue, bool bImmediate = true);

        [DllImport("DeviceRtdb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int DRTDB_MD_SendNewValue_EX(string sDeviceName, ref TAG_VALUE_LOCAL TagValue, bool bImmediate, bool bFiltered);

        [DllImport("DeviceRtdb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int DRTDB_MD_GetTagCount(string sDeviceName,ref int bImmediate);
        [DllImport("DeviceRtdb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int DRTDB_MD_GetCurrentState(string sDeviceName);
        [DllImport("DeviceRtdb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern void DRTDB_MD_SetCallBackFunction(string sDeviceName, TARGETHANDLER target_arrived, TARGETHANDLER target_removed, TARGETVALHANDLER value_seted, TARGETHANDLER value_geted);


        //public static extern int DRTDB_MD_SendNewValues(string sDeviceName, TAG_VALUE_LOCAL[] lpTagValues, int lTagCount);

        //public static extern int DRTDB_MD_SendHisValuesUsingTagID(string sDeviceName, TAG_VALUE_LOCAL[] lpHisTagValues, int lCount);


        #endregion

        #region WINDOWS API 

        public const int PM_NOREMOVE = 0x0000;
        public const int PM_REMOVE = 0x0001;
        public const int WM_QUIT = 0x0012;
        public struct POINTAPI
        {
            public Int32 x;
            public Int32 y;
        }
        public struct MSG
        {
            public Int32 hwnd;
            public Int32 message;
            public Int32 wParam;
            public Int32 lParam;
            public Int32 time;
            public POINTAPI pt;
        }
        [DllImport("Coredll.dll", SetLastError = true)]
        public static extern bool PeekMessage(ref MSG lpMsg, Int32 hwnd, Int32 wMsgFilterMin, Int32 wMsgFilterMax, Int32 wRemoveMsg);

        [DllImport("Coredll.dll", SetLastError = true)]
        public static extern Int32 DispatchMessage(ref MSG lpMsg);

        [DllImport("Coredll.dll", SetLastError = true)]
        public static extern Int32 TranslateMessage(ref MSG lpMsg);

        [DllImport("user32")]
        public static extern int GetMessage(ref MSG lpMsg,int hwnd,int wMsgFilterMin,int wMsgFilterMax);
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        [DllImport("user32.dll")]
        public static extern int PostThreadMessage(int threadId, //线程标识
                          uint msg, //消息标识符

                          int wParam, //具体由消息决定

                          int lParam); //具体由消息决定

        #endregion

    }

}