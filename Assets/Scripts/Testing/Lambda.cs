using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

namespace TBL.Testing
{
    public class Lambda : MonoBehaviour
    {
        // Start is called before the first frame update
        async void Start()
        {
            await Test(new Func<Task<int>>(async () =>
            {
                while (true)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                        break;

                    await Task.Yield();
                }
                return 100;
            }));
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {

            }
        }

        async Task Test(Func<Task<int>> i)
        {
            print(await i.Invoke());
        }
    }
}
