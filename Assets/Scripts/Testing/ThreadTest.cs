using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Threading;
using TBL.Util;

public class ThreadTest : MonoBehaviour
{
    Thread thread;
    Task task;
    CancellationTokenSource cs;
    Action<MyClass> myclassAction;
   public UnityEngine.UI.Button btn;

    private void Start()
    {
        cs = new CancellationTokenSource();
        // thread = new Thread(Test);
        // thread.Start();
        // task = Task.Run(
        //     async () =>
        //     {
        //         while (true)
        //         {
        //             await Task.Yield();
        //             Debug.Log("HI");
        //             // Print();
        //             if (cs.Token.IsCancellationRequested)
        //                 return;
        //         }
        //     }, cs.Token
        //  );
        MyClass mc = new MyClass();
        mc = new MyClass();
        mc.name = "Hello";
        myclassAction = (c) =>
        {
            c.name = "Nope";
        };
        btn.onClick.AddListener(() =>
        {
            myclassAction(mc);
            print(mc.name);
        });
        // MyAction(mc);
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        //     print()
            // cs.Cancel();
            // thread.Abort();
    }

    public void Test()
    {
        while (true)
        {
            Debug.Log("Hi");
            Thread.Yield();
        }
    }

    async Task MyAction(MyClass mc)
    {
        while (true)
        {
            Debug.Log(gameObject.name);
            await Task.Yield();
        }
    }


    void Print()
    {
        Debug.Log(gameObject.name);
    }

    public struct MyClass
    {
        public string name;
    }
}
