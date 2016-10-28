using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;


namespace Sensip
{
    class DataProcessing<T>
    {
        

        private static Queue<T> sharedQueue = new Queue<T>();
        
        Thread thProducer;
        Thread thConsumer;
        public bool _bStop = false;

        public void StartProcessing()
        {
            _bStop = false;
            thProducer = new Thread(Producer);
            thConsumer = new Thread(Consumer);
            thProducer.Start();
            thConsumer.Start();

        }

        public void StopProcessing()
        {
            _bStop = true;
            //thConsumer.Join();
            thConsumer.Abort();
            //thProducer.Join();
            thConsumer.Abort();
        }


        public virtual void Producer()
        {
            return;
        }


        public virtual void Consumer()
        {
            return;
        }




        public void EndProcessing()
        {
            // Join on them:
            thProducer.Join();
            thConsumer.Join();
        }

        protected void ProducerData(T var)
        {
            lock (sharedQueue)
            {
                sharedQueue.Enqueue(var);
                Monitor.Pulse(sharedQueue);
            }
        }

        protected T ConsumerData()
        {
            lock (sharedQueue)
            {
                while (sharedQueue.Count == 0)
                    Monitor.Wait(sharedQueue);
                return (sharedQueue.Dequeue());
            }
        }
    }
}
