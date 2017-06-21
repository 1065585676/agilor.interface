//using Agilor.Interface;
//using System;
////using Agilor.Interface;
////using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace agilor.@interface
//{


//    class Program
//    {




//        static void Main(string[] args)
//        {
//            ACI aci = ACI.Instance("TEST2", "11.0.0.91");//连接 测试通过
//            aci.Close();









//            //            //addTarget 测试通过
//            //            //TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
//            //            //Target node = new Target();
//            //            //node.Name = "test" + Convert.ToInt64(ts.TotalSeconds).ToString();
//            //            //node.Scan = Target.Status.IN;
//            //            //node.Device = "TEST_DEVICE";
//            //            //node.Descriptor = " this is descriptor";
//            //            //node.SourceName = node.Name;
//            //            //node.Type = Value.Types.FLOAT;
//            //            //aci.addTarget(node);


//            //            //QuerySnapshots测试通过
//            //            //List<Value> values = aci.QuerySnapshots(new string[] { "NCS", "NCS2" });

//            //            //foreach(Value val in values)
//            //            //    Console.WriteLine("name:{0},value:{1}",val.Name,(float)val.Val);

//            //            //QueryTagHistory测试通过
//            //            //DateTime start = new DateTime(2015,12,4,23,44,35);
//            //            //DateTime end  = new DateTime(2015,12,4,23,44,42);
//            //            //values = aci.QueryTagHistory("NCS",start,end);
//            //            //foreach (Value val in values)
//            //            //    Console.WriteLine("name:{0},value:{1},time:{2}", val.Name, (float)val.Val, val.Time.ToString("yyyy-MM-dd hh:mm::ss"));


//            //            //GetTarget测试通过
//            //            //Target target = aci.GetTarget(node.Name);

//            //            //removeTarget测试通过
//            //            // aci.removeTarget(target.Id);

//            //            //aci.close();//关闭连接 测试通过


//            //RTDB rtdb = RTDB.Instance("NCS", "192.168.178.128", 700);
//            //rtdb.WriteValue(new Value("NCS", 18.3f));
//            //rtdb.Flush();

//            //rtdb.close();

//            Console.Read();

//        }
//    }
//}
