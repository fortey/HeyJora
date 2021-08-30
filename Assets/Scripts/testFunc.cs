using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.CodeDom;
using System.CodeDom.Compiler;
using System;
using UnityEngine.UI;
using Jint;

public class testFunc : MonoBehaviour
{
    public Text text;

    private AppDomain domain;

    private Engine engine;

    // Start is called before the first frame update
    void Start()
    {
        engine = new Engine();
        engine.SetValue("log", new Action<object>(msg => Debug.Log(msg)));

        engine.Execute(@"
        var myVariable = 108;
        log('Hello from Javascript! myVariable = '+myVariable);
      ");
   
        
//           var parameters = new CompilerParameters(); //Кстати я так понял эта строка вроде и вызывает ошибку во случае с IL2CPP
//        parameters.GenerateExecutable = false;
//        parameters.GenerateInMemory = true;
//        //parameters.OutputAssembly = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Code/Test.dll");

//        parameters.ReferencedAssemblies.Add("System.dll");
//        //parameters.ReferencedAssemblies.Add(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Unity/UnityEngine.dll"));
//        string _sourceCode = @"
//using System;

//    public class TestClass
//    {
//        public static string TestMethod(string msg, string msg2)
//        {
//            return msg+msg2;
//        }
//                                    }";
//		var provider = CodeDomProvider.CreateProvider("CSharp");
//        var cr = provider.CompileAssemblyFromSource(parameters, _sourceCode);

//        if (domain != null)
//        {
//            AppDomain.Unload(domain);
//        }

//        var info = new AppDomainSetup()
//        {
//            ShadowCopyFiles = "true"
//        };

//        if (cr.Errors.Count <= 0)
//        {
//            domain = AppDomain.CreateDomain("additional", new System.Security.Policy.Evidence(), info);

//            //domain.Load(cr.CompiledAssembly.GetName());

//            Debug.Log(cr.PathToAssembly);

//            var testType = cr.CompiledAssembly.GetType("TestClass");
//            var gethw = testType.GetMethod("TestMethod");

//            string[] arrParams = { "Test message.", "Second test message." };
//            string resultMsg =  gethw.Invoke(null, arrParams).ToString();
//            Debug.Log(resultMsg);
//            text.text = resultMsg;
//        }
//        else
//        {
//            foreach (CompilerError error in cr.Errors)
//            {
//                Debug.LogError(error.ErrorText);
//            }
//        }
    

//    //CompilerOptions = "/t:library",
//    var compiler = CodeDomProvider.CreateProvider("CSharp");
//        var parameters = new CompilerParameters
//        {
            
//    GenerateInMemory = true,
//    IncludeDebugInformation = false
//};
//        string _sourceCode = @"
//using System;

//    public class TestClass
//    {
//        public static void TestMethod()
//        {
//            Debug.Log(""Hello, World!"");
//        }

//}";

//        CompilerResults results = compiler.CompileAssemblyFromSource(parameters, _sourceCode);
//        //var instance = results.CompiledAssembly.CreateInstance("Test.TestClass");
//        results.CompiledAssembly.GetType("TestClass").GetMethod("TestMethod").Invoke(null, null);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
